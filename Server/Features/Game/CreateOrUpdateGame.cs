#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Features.Game;

using Result = OneOf<Unit, ValidationProblemDetails>;

public static partial class CreateOrUpdateGame
{
    public sealed record Command : IRequest<Result>
    {
        /// <summary>
        /// The identifier of the season week in which the game is scheduled.
        /// </summary>
        public int SeasonWeekId { get; init; }

        /// <summary>
        /// The date and time when the game is scheduled to start.
        /// </summary>
        public DateTimeOffset StartsOn { get; init; }

        /// <summary>
        /// The identifier of the home team participating in the game.
        /// </summary>
        public int HomeTeamId { get; init; }

        /// <summary>
        /// The score of the home team.
        /// </summary>
        public int HomeTeamScore { get; init; }

        /// <summary>
        /// The identifier of the away team participating in the game.
        /// </summary>
        public int AwayTeamId { get; init; }

        /// <summary>
        /// The score of the away team.
        /// </summary>
        public int AwayTeamScore { get; init; }

        /// <summary>
        /// The current quarter of the game (e.g., 1 for first quarter).
        /// </summary>
        public int Quarter { get; init; }

        /// <summary>
        /// The current clock time in the game, formatted as MM:SS. Null if not applicable.
        /// </summary>
        public string? ClockTime { get; init; }

        /// <summary>
        /// Indicates whether the game is complete.
        /// </summary>
        public bool IsComplete { get; init; }
    }


    [GeneratedRegex(@"^\d{1,2}:\d{2}$")]
    private static partial Regex ClockTimeRegex();

    public sealed class CreateOrUpdateGameValidator : AbstractValidator<Command>
    {
        private readonly WebAppDbContext _dbContext;

        public CreateOrUpdateGameValidator(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.SeasonWeekId)
                .GreaterThan(0)
                .MustAsync(async (seasonWeekId, cancellation) =>
                {
                    return await _dbContext.SeasonWeeks.FindAsync([seasonWeekId], cancellation) is not null;
                })
                .WithMessage($"Invalid {nameof(Command.SeasonWeekId)}");

            RuleFor(x => x.StartsOn)
                .NotEmpty();

            RuleFor(x => x.HomeTeamId)
                .GreaterThan(0)
                .MustAsync(async (homeTeamId, cancellation) =>
                {
                    return await _dbContext.Teams.FindAsync([homeTeamId], cancellation) is not null;
                })
                .WithMessage($"Invalid {nameof(Command.HomeTeamId)}");

            RuleFor(x => x.HomeTeamScore)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.AwayTeamId)
                .NotEmpty()
                .MustAsync(async (awayTeamId, cancellation) =>
                {
                    return await _dbContext.Teams.FindAsync([awayTeamId], cancellation) is not null;
                })
                .WithMessage($"Invalid {nameof(Command.AwayTeamId)}");

            RuleFor(x => x.AwayTeamScore)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Quarter)
                .GreaterThan(0);

            RuleFor(x => x.ClockTime)
                .Matches(ClockTimeRegex())
                .WithMessage($"{nameof(Command.ClockTime)} must be in the format MM:SS.");
        }
    }

    public sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly IValidator<Command> _validator;
        private readonly WebAppDbContext _dbContext;

        public Handler(IValidator<Command> validator, WebAppDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async ValueTask<Result> Handle(Command cmd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = await _validator.ValidateRequestAsync(cmd, cancellationToken);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            var game = await _dbContext.Games
                .Where(x => x.SeasonWeekId == cmd.SeasonWeekId)
                .Where(x => x.HomeTeamId == cmd.HomeTeamId)
                .Where(x => x.AwayTeamId == cmd.AwayTeamId)
                .FirstOrDefaultAsync(cancellationToken);

            if (game is null)
            {
                var gameDb = new Database.Tables.Game
                {
                    SeasonWeekId = cmd.SeasonWeekId,
                    StartsOn = cmd.StartsOn,
                    HomeTeamId = cmd.HomeTeamId,
                    HomeTeamScore = cmd.HomeTeamScore,
                    AwayTeamId = cmd.AwayTeamId,
                    AwayTeamScore = cmd.AwayTeamScore,
                    Quarter = cmd.Quarter,
                    ClockTime = cmd.IsComplete ? "0:00" : cmd.ClockTime,
                    IsComplete = cmd.IsComplete,
                };

                _dbContext.Games.Add(gameDb);
            }
            else
            {
                game.StartsOn = cmd.StartsOn;
                game.HomeTeamScore = cmd.HomeTeamScore;
                game.AwayTeamScore = cmd.AwayTeamScore;
                game.Quarter = cmd.Quarter;
                game.ClockTime = cmd.IsComplete ? "0:00" : cmd.ClockTime;
                game.IsComplete = cmd.IsComplete;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

