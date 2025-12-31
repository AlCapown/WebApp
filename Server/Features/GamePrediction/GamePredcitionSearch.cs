#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;
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
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Features.GamePrediction;

using Result = OneOf<GamePredictionSearchResponse, ValidationProblemDetails, ForbiddenProblemDetails>;

public static class GamePredictionSearch
{
    public sealed record Query : IRequest<Result>
    {
        /// <summary>
        /// The identifier for the season to filter game predictions.
        /// Optional. If provided, only predictions for games in the specified season will be included.
        /// </summary>
        public int? SeasonId { get; init; }

        /// <summary>
        /// The identifier for the season week to filter game predictions.
        /// </summary>
        public int? SeasonWeekId { get; init; }

        /// <summary>
        /// The identifier for a specific game to filter predictions.
        /// Optional. If provided, only predictions for the specified game will be included.
        /// </summary>
        public int? GameId { get; init; }

        /// <summary>
        /// The identifier of the user to filter predictions.
        /// Optional. If provided, only predictions made by this user will be included.
        /// </summary>
        public string? UserId { get; init; }

        /// <summary>
        /// If true, limits the results to predictions made by the current authenticated user.
        /// Optional. If not set or false, predictions from all users may be included.
        /// </summary>
        public bool? LimitToCurrentUser { get; init; }

        /// <summary>
        /// The identifier for a team to filter predictions.
        /// Optional. If provided, only predictions for games involving this team (as home or away) will be included.
        /// </summary>
        public int? TeamId { get; init; }
    }

    public sealed class GamePredictionSearchValidator : AbstractValidator<Query>
    {
        public GamePredictionSearchValidator()
        {
            RuleFor(x => x.SeasonId)
                .GreaterThan(0)
                .When(x => x.SeasonId.HasValue);

            RuleFor(x => x.SeasonWeekId)
                .GreaterThan(0)
                .When(x => x.SeasonWeekId.HasValue);

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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            IValidator<Query> validator, 
            WebAppDbContext dbContext, 
            IHttpContextAccessor httpContextAccessor)
        {
            _validator = validator;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<Result> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = _validator.ValidateRequest(query);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            string? userId = _httpContextAccessor.HttpContext?.User.Claims.GetUserId();

            var gamePredictionQuery = _dbContext.GamePredictions
                .AsNoTracking()
                .Select(x => new Common.Models.GamePrediction
                {
                    GamePredictionId = x.GamePredictionId,
                    SeasonId = x.Game.SeasonWeek.SeasonId,
                    SeasonWeekId = x.Game.SeasonWeekId,
                    GameId = x.GameId,
                    UserId = x.UserId,
                    IsCurrentUser = x.UserId == userId,
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

        private static IQueryable<Common.Models.GamePrediction> AddFilters(IQueryable<Common.Models.GamePrediction> gamePredictionQuery, Query query)
        {
            if (query.SeasonId.HasValue)
            {
                gamePredictionQuery = gamePredictionQuery.Where(x => x.SeasonId == query.SeasonId.Value);
            }

            if (query.SeasonWeekId.HasValue)
            {
                gamePredictionQuery = gamePredictionQuery.Where(x => x.SeasonWeekId == query.SeasonWeekId.Value);
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
