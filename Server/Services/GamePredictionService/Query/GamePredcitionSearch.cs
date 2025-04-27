using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Extensions;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Server.Services.AccountService.Query;

namespace WebApp.Server.Services.GamePredictionService.Query;

public class GamePredictionSearch
{
    public class Query : IRequest<Result>
    {
        public int? SeasonId { get; set; }
        public int? GameId { get; set; }
        public string UserId { get; set; }
        public bool? LimitToCurrentUser { get; set; }
        public int? TeamId { get; set; }
    }

    public class Result
    {
        public GamePrediction[] GamePredictions { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly WebAppDbContext _dbContext;
        private readonly IMediator _mediator;

        public Handler(WebAppDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<Result> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

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

            gamePredictionQuery = AddFilters(query, gamePredictionQuery);

            var dbResult = await gamePredictionQuery.ToArrayAsync(token);

            return new Result
            {
                GamePredictions = dbResult,
            };
        }

        private static IQueryable<GamePrediction> AddFilters(Query query, IQueryable<GamePrediction> gamePredictionQuery)
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
