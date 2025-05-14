#nullable enable

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Services.GameService;

using Result = OneOf<SearchGamesResponse, ValidationProblemDetails>;

public sealed class GameSearch
{
    public sealed record Query : IRequest<Result>
    {
        /// <summary>
        /// The identifier for the specific season week to filter games.
        /// Optional. If provided, only games from the specified season week will be included.
        /// </summary>
        public int? SeasonWeekId { get; init; }

        /// <summary>
        /// The identifier for the season to filter games.
        /// Optional. If provided, only games from the specified season will be included.
        /// </summary>
        public int? SeasonId { get; init; }

        /// <summary>
        /// The identifier for the team to filter games.
        /// Optional. If provided, only games involving the specified team (as home or away) will be included.
        /// </summary>
        public int? TeamId { get; init; }

        /// <summary>
        /// The minimum start date and time for games to include in the results.
        /// Optional. If provided, only games starting after this date and time will be included.
        /// </summary>
        public DateTimeOffset? GameStartsOnMin { get; init; }

        /// <summary>
        /// The maximum start date and time for games to include in the results.
        /// Optional. If provided, only games starting before this date and time will be included.
        /// </summary>
        public DateTimeOffset? GameStartsOnMax { get; init; }

        /// <summary>
        /// Indicates whether to filter games based on their completion status.
        /// Optional. If provided, only games matching the specified completion status will be included.
        /// </summary>
        public bool? IsGameComplete { get; init; }
    }

    public sealed class SearchGameValidator : AbstractValidator<Query> 
    {
        public SearchGameValidator()
        {
            RuleFor(x => x.SeasonWeekId)
                .GreaterThan(0)
                .When(x => x.SeasonWeekId.HasValue);

            RuleFor(x => x.SeasonId)
                .GreaterThan(0)
                .When(x => x.SeasonId.HasValue);

            RuleFor(x => x.TeamId)
                .GreaterThan(0)
                .When(x => x.TeamId.HasValue);

            RuleFor(x => x.GameStartsOnMin)
                .LessThan(x => x.GameStartsOnMax)
                .When(x => x.GameStartsOnMin.HasValue && x.GameStartsOnMax.HasValue);
        }
    }

    public sealed class Handler : IRequestHandler<Query, Result>
    {
        private readonly IValidator<Query> _validator;
        private readonly WebAppDbContext _dbContext;

        public Handler(IValidator<Query> validator, WebAppDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = _validator.ValidateRequest(query);
            if (problemDetails is not null)
            {
                return problemDetails;
            }   

            var gameQuery =
                from g in _dbContext.Games.AsNoTracking()
                orderby g.StartsOn
                select new Game
                {
                    GameId = g.GameId,
                    SeasonWeekId = g.SeasonWeekId,
                    SeasonId = g.SeasonWeek.SeasonId,
                    Week = g.SeasonWeek.Week,
                    SeasonWeekTypeName = g.SeasonWeek.SeasonWeekTypeName,
                    GameStartsOn = g.StartsOn,
                    HomeTeamId = g.HomeTeamId,
                    HomeTeamName = g.HomeTeam.TeamName,
                    HomeTeamScore = g.HomeTeamScore,
                    AwayTeamId = g.AwayTeamId,
                    AwayTeamName = g.AwayTeam.TeamName,
                    AwayTeamScore = g.AwayTeamScore,
                    ClockTime = g.ClockTime,
                    Quarter = g.Quarter,
                    IsComplete = g.IsComplete
                };

            gameQuery = AddFilters(query, gameQuery);

            var games = await gameQuery.ToArrayAsync(token);

            return new SearchGamesResponse
            {
                Games = games
            };
        }

        private static IQueryable<Game> AddFilters(Query query, IQueryable<Game> gameQuery)
        {
            if (query.SeasonWeekId.HasValue)
            {
                gameQuery = gameQuery.Where(x => x.SeasonWeekId == query.SeasonWeekId.Value);
            }

            if (query.SeasonId.HasValue)
            {
                gameQuery = gameQuery.Where(x => x.SeasonId == query.SeasonId.Value);
            }

            if (query.TeamId.HasValue)
            {
                gameQuery = gameQuery.Where(x => x.HomeTeamId == query.TeamId.Value || x.AwayTeamId == query.TeamId.Value);
            }

            if (query.GameStartsOnMin.HasValue)
            {
                gameQuery = gameQuery.Where(x => x.GameStartsOn > query.GameStartsOnMin.Value);
            }

            if (query.GameStartsOnMax.HasValue)
            {
                gameQuery = gameQuery.Where(x => x.GameStartsOn < query.GameStartsOnMax.Value);
            }

            if (query.IsGameComplete.HasValue)
            {
                gameQuery = gameQuery.Where(x => x.IsComplete == query.IsGameComplete.Value);
            }

            return gameQuery;
        }
    }
}

