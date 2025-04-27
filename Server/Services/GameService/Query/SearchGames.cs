using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Database;

namespace WebApp.Server.Services.GameService.Query;

public class SearchGames
{
    public class Query : IRequest<SearchGamesResponse>
    {
        public int? SeasonWeekId { get; set; }
        public int? SeasonId { get; set; }
        public int? TeamId { get; set; }
        public DateTimeOffset? GameStartsOnMin { get; set; }
        public DateTimeOffset? GameStartsOnMax { get; set; }
        public bool? IsGameComplete { get; set; }
    }

    public class Handler : IRequestHandler<Query, SearchGamesResponse>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SearchGamesResponse> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

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
