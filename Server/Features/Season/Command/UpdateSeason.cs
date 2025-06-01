using Mediator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Infrastructure.Exceptions;

namespace WebApp.Server.Features.SeasonService.Command;

public class UpdateSeason
{
    public class Command : IRequest<Unit>
    {
        public int SeasonId { get; set; }
        public string Description { get; set; }
        public DateOnly? SeasonStart { get; set; }
        public DateOnly? SeasonEnd { get; set; }
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<Unit> Handle(Command cmd, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var season = await _dbContext.Seasons
                .FirstOrDefaultAsync(x => x.SeasonId == cmd.SeasonId, token);

            if (season == null)
            {
                throw new WebAppValidationException(nameof(Command.SeasonId), $"The {cmd.SeasonId} season does not exist.");
            }

            season.Description = cmd.Description ?? season.Description;
            season.SeasonStart = cmd.SeasonStart ?? season.SeasonStart;
            season.SeasonEnd = cmd.SeasonEnd ?? season.SeasonEnd;

            await _dbContext.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
