using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;

namespace WebApp.Server.Services.GameService;

public class UpsertGame
{
    public class Command : IRequest<Unit>
    {
        public int SeasonWeekId { get; set; }
        public DateTimeOffset StartsOn { get; set; }
        public int HomeTeamId { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamId { get; set; }
        public int AwayTeamScore { get; set; }
        public int Quarter { get; set; }
        public string ClockTime { get; set; }
        public bool IsComplete { get; set; }
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(Command cmd, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var game = await _dbContext.Games
                .Where(x => x.SeasonWeekId == cmd.SeasonWeekId)
                .Where(x => x.HomeTeamId == cmd.HomeTeamId)
                .Where(x => x.AwayTeamId == cmd.AwayTeamId)
                .FirstOrDefaultAsync(token);

            if (game == null)
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

            await _dbContext.SaveChangesAsync(token);
            return Unit.Value;
        }
    }
}
