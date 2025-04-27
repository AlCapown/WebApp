using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Database;
using WebApp.Server.Infrastructure.Exceptions;

namespace WebApp.Server.Services.GamePredictionService.Command;

public class UpsertGamePrediction
{
    public class Command : IRequest<Result>
    {
        public int GameId { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public string UserId { get; set; }
        public bool BypassGameStartTimeValidation { get; set; }
    }

    public class Result
    {
        public int GamePredictionId { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly IMediator _mediator;
        private readonly WebAppDbContext _dbContext;

        public Handler(IMediator mediator, WebAppDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Command cmd, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var game = await _dbContext.Games
                .AsNoTracking()
                .Where(x => x.GameId == cmd.GameId)
                .FirstOrDefaultAsync(token);

            if (game == null)
            {
                throw new WebAppValidationException(nameof(Command.GameId), "Game does not exist or is invalid.");
            }

            if (cmd.BypassGameStartTimeValidation || (game.StartsOn.HasValue && game.StartsOn.Value < DateTimeOffset.Now))
            {
                throw new WebAppValidationException(nameof(Command.GameId), "You cannot modify or add a prediction for this game since it has already started.");
            }

            if (game.HomeTeamId != SeasonConstants.CURRENT_TEAM_ID && game.AwayTeamId != SeasonConstants.CURRENT_TEAM_ID)
            {
                throw new WebAppValidationException(nameof(Command.GameId), "This is not a team you can make a prediction for.");
            }

            var prediction = await _dbContext.GamePredictions
                .Where(x => x.GameId == cmd.GameId)
                .Where(x => x.UserId == cmd.UserId)
                .FirstOrDefaultAsync(token);

            int gamePredictionId;

            if (prediction == null)
            {
                var gamePredictionDb = new Database.Tables.GamePrediction
                {
                    GameId = cmd.GameId,
                    UserId = cmd.UserId,
                    HomeTeamScore = cmd.HomeTeamScore,
                    AwayTeamScore = cmd.AwayTeamScore,
                    DateCreated = DateTimeOffset.Now,
                };

                _dbContext.GamePredictions.Add(gamePredictionDb);

                await _dbContext.SaveChangesAsync(token);

                gamePredictionId = gamePredictionDb.GamePredictionId;
            }
            else
            {
                prediction.HomeTeamScore = cmd.HomeTeamScore;
                prediction.AwayTeamScore = cmd.AwayTeamScore;
                prediction.DateCreated = DateTimeOffset.Now;

                await _dbContext.SaveChangesAsync(token);

                gamePredictionId = prediction.GamePredictionId;
            }

            return new Result
            {
                GamePredictionId = gamePredictionId
            };
        }
    }
}
