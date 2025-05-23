#nullable enable

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Extensions;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Server.Infrastructure;
using WebApp.Server.Services.AccountService.Query;

namespace WebApp.Server.Services.GamePredictionService;

using Result = OneOf<GamePredictionSearchResponse, ValidationProblemDetails, ForbiddenProblemDetails>;

public sealed class GamePredictionSearch
{
    public sealed record Query : IRequest<Result>
    {
        public int? SeasonId { get; set; }
        public int? GameId { get; set; }
        public string? UserId { get; set; }
        public bool? LimitToCurrentUser { get; set; }
        public int? TeamId { get; set; }
    }

    public sealed class GamePredictionSearchValidator : AbstractValidator<Query>
    {
        public GamePredictionSearchValidator()
        {
            RuleFor(x => x.SeasonId)
                .GreaterThan(0)
                .When(x => x.SeasonId.HasValue);

            RuleFor(x => x.GameId)
                .GreaterThan(0)
                .When(x => x.GameId.HasValue);

            RuleFor(x => x.TeamId)
                .GreaterThan(0)
                .When(x => x.TeamId.HasValue);
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

            var gamePredictionQuery = _dbContext.GamePredictions
                .AsNoTracking()
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
                });

            gamePredictionQuery = AddFilters(gamePredictionQuery, query);

            var gamePredictions = await gamePredictionQuery.ToArrayAsync(token);

            return new GamePredictionSearchResponse
            {
                GamePredictions = gamePredictions
            };
        }

        private static IQueryable<GamePrediction> AddFilters(IQueryable<GamePrediction> gamePredictionQuery, Query query)
        {
            if (query.SeasonId.HasValue)
            {
                gamePredictionQuery = gamePredictionQuery.Where(x => x.SeasonId == query.SeasonId.Value);
            }

            if (query.GameId.HasValue)
            {
                gamePredictionQuery = gamePredictionQuery.Where(x => x.GameId == query.GameId.Value);
            }

            if (!query.UserId.IsNullOrWhiteSpace())
            {
                gamePredictionQuery = gamePredictionQuery.Where(x => x.UserId == query.UserId);
            }

            if (query.LimitToCurrentUser == true)
            {
                gamePredictionQuery = gamePredictionQuery.Where(x => x.IsCurrentUser);
            }

            if (query.TeamId.HasValue)
            {
                gamePredictionQuery = gamePredictionQuery.Where(x => x.HomeTeamId == query.TeamId.Value || x.AwayTeamId == query.TeamId.Value);
            }

            return gamePredictionQuery;
        }
    }
}
