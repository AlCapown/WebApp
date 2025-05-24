#nullable enable

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Server.Infrastructure;
using WebApp.Server.Services.AccountService.Query;

namespace WebApp.Server.Services.GamePredictionService;

using Result = OneOf<GetGamePredictionResponse, ValidationProblemDetails, NotFoundProblemDetails>;

public static class GetGamePrediction
{
    public sealed record Query : IRequest<Result>
    {
        /// <summary>
        /// Unique identifier for the game prediction.
        /// </summary>
        public int GamePredictionId { get; init; }
    }

    public sealed class GetGamePredictionValidator : AbstractValidator<Query>
    {
        public GetGamePredictionValidator()
        {
            RuleFor(x => x.GamePredictionId)
                .GreaterThan(0);
        }
    }

    public sealed class Handler : IRequestHandler<Query, Result>
    {
        private readonly IValidator<Query> _validator;
        private readonly WebAppDbContext _dbContext;
        private readonly IMediator _mediator;

        public Handler(IValidator<Query> validator, WebAppDbContext dbContext, IMediator mediator)
        {
            _validator = validator;
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<Result> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = _validator.ValidateRequest(query);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            var currentUser = await _mediator.Send(new GetCurrentAppUser.Query(), token);

            var dbResult = await _dbContext.GamePredictions
                .AsNoTracking()
                .Where(x => x.GamePredictionId == query.GamePredictionId)
                .Select(x => new GamePrediction
                {
                    GamePredictionId = x.GamePredictionId,
                    SeasonId = x.Game.SeasonWeek.SeasonId,
                    GameId = x.GameId,
                    UserId = x.UserId,
                    IsCurrentUser = x.UserId == currentUser.UserId,
                    FirstName = x.AppUser.FirstName,
                    LastName = x.AppUser.LastName,
                    HomeTeamId = x.Game.HomeTeamId,
                    PredictedHomeTeamScore = x.HomeTeamScore,
                    AwayTeamId = x.Game.AwayTeamId,
                    PredictedAwayTeamScore = x.AwayTeamScore
                })
                .FirstOrDefaultAsync(token);

            if (dbResult is null)
            {
                return new NotFoundProblemDetails($"Game prediction with ID of {query.GamePredictionId} was not found.");
            }

            return new GetGamePredictionResponse
            {
                GamePrediction = dbResult
            };
        }
    }
}
