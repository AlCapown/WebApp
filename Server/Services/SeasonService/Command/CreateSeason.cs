using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Infrastructure.Exceptions;

namespace WebApp.Server.Services.SeasonService.Command;

public class CreateSeason
{
    public class Command : IRequest<int>
    {
        public int SeasonId { get; set; }
        public string Description { get; set; }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Handle(Command cmd, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var alreadyExists =
                await _dbContext.Seasons
                    .AsNoTracking()
                    .AnyAsync(x => x.SeasonId == cmd.SeasonId, token);

            if (alreadyExists)
            {
                throw new WebAppValidationException(nameof(cmd.SeasonId), $"The {cmd.SeasonId} season already exists.");
            }

            var season = new Database.Tables.Season
            {
                SeasonId = cmd.SeasonId,
                Description = cmd.Description
            };

            await _dbContext.Seasons.AddAsync(season, token);
            await _dbContext.SaveChangesAsync(token);

            return season.SeasonId;
        }
    }
}
