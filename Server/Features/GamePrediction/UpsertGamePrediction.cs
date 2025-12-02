#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Server.Infrastructure;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Features.GamePrediction;

using Result = OneOf<CreateGamePredictionResponse, ValidationProblemDetails, ForbiddenProblemDetails>;

public static class UpsertGamePrediction
{
    public sealed record Command : IRequest<Result>
    {
        /// <summary>
        /// The unique identifier of the game for which the prediction is being made.
        /// </summary>
        public int? GameId { get; init; }

        /// <summary>
        /// The predicted score for the home team.
        /// </summary>
        public int? HomeTeamScore { get; init; }

        /// <summary>
        /// The predicted score for the away team.
        /// </summary>
        public int? AwayTeamScore { get; init; }

        /// <summary>
        /// The unique identifier of the user making the prediction.
        /// </summary>
        public string? UserId { get; init; }

        /// <summary>
        /// If true, bypasses the validation that prevents predictions after the game has started.
        /// Used for administrative overrides.
        /// </summary>
        public bool BypassGameStartTimeValidation { get; init; }
    }

    public sealed class UpsertGamePredictionValidator : AbstractValidator<Command>
    {
        private readonly WebAppDbContext _dbContext;

        public UpsertGamePredictionValidator(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.GameId)
                .NotEmpty()
                .GreaterThan(0)
                .MustAsync(async (gameId, cancellation) =>
                {
                    return await _dbContext.Games.FindAsync([gameId], cancellation) is not null;
                })
                .WithMessage($"Invalid {nameof(Command.GameId)}");

            RuleFor(x => x.HomeTeamScore)
                .NotEmpty()
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.AwayTeamScore)
                .NotEmpty()
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.UserId)
                .NotEmpty()
                .MaximumLength(450)
                .MustAsync(async (userId, cancellation) =>
                {
                    return await _dbContext.AppUsers.FindAsync([userId], cancellation) is not null;
                })
                .WithMessage($"Invalid {nameof(Command.UserId)}");
        }
    }

    public sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly WebAppDbContext _dbContext;
        private readonly IValidator<Command> _validator;

        public Handler(WebAppDbContext dbContext, IValidator<Command> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async ValueTask<Result> Handle(Command cmd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = await _validator.ValidateRequestAsync(cmd, cancellationToken);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            var game = await _dbContext.Games.FindAsync([cmd.GameId], cancellationToken);

            Debug.Assert(game is not null, "Should not be null becasue of validation.");

            if (!cmd.BypassGameStartTimeValidation && game.StartsOn.HasValue && game.StartsOn.Value < DateTimeOffset.Now)
            {
                return new ForbiddenProblemDetails("You cannot modify or add a prediction for this game since it has already started.");
            }

            if (game!.HomeTeamId != SeasonConstants.CURRENT_TEAM_ID && game.AwayTeamId != SeasonConstants.CURRENT_TEAM_ID)
            {
                return new ForbiddenProblemDetails("This is not a team you can make a prediction for.");
            }

            var prediction = await _dbContext.GamePredictions.FirstOrDefaultAsync
            (
                x => x.GameId == cmd.GameId && x.UserId == cmd.UserId,
                cancellationToken
            );

            int? gamePredictionId;

            if (prediction is null)
            {
                var gamePredictionDb = new Database.Tables.GamePrediction
                {
                    GameId = cmd.GameId!.Value,
                    UserId = cmd.UserId,
                    HomeTeamScore = cmd.HomeTeamScore!.Value,
                    AwayTeamScore = cmd.AwayTeamScore!.Value,
                    DateCreated = DateTimeOffset.Now,
                };

                _dbContext.GamePredictions.Add(gamePredictionDb);

                await _dbContext.SaveChangesAsync(cancellationToken);

                gamePredictionId = gamePredictionDb.GamePredictionId;
            }
            else
            {
                prediction.HomeTeamScore = cmd.HomeTeamScore!.Value;
                prediction.AwayTeamScore = cmd.AwayTeamScore!.Value;
                prediction.DateCreated = DateTimeOffset.Now;

                await _dbContext.SaveChangesAsync(cancellationToken);

                gamePredictionId = prediction.GamePredictionId;
            }

            return new CreateGamePredictionResponse
            {
                GamePredictionId = gamePredictionId.Value
            };
        }
    }
}
