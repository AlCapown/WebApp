using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Server.Services.AccountService.Query;

namespace WebApp.Server.Services.GamePredictionService.Query;

public class GetGamePrediction
{
    public class Query : IRequest<Result>
    {
        public int GamePredictionId { get; set; }
    }

    public class Result
    {
        public GamePrediction GamePrediction { get; set; }
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

            return new Result
            {
                GamePrediction = dbResult
            };
        }
    }
}
