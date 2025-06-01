using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Infrastructure.Exceptions;

namespace WebApp.Server.Features.SeasonService.Command;

public class UpdateWeekForSeason
{
    public class Command : IRequest<Unit>
    {
        public int SeasonId { get; set; }
        public int SeasonWeekId { get; set; }
        public DateOnly WeekStart { get; set; }
        public DateOnly WeekEnd { get; set; }
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

            // TODO validate week overlap

            var week = await _dbContext.SeasonWeeks
                .FirstOrDefaultAsync(x => x.SeasonId == cmd.SeasonId && x.SeasonWeekId == cmd.SeasonWeekId, token);

            if (week == null)
            {
                throw new WebAppValidationException(nameof(Command.SeasonWeekId), $"SeasonWeekId {cmd.SeasonWeekId} for {cmd.SeasonId} season does not exist.");
            }

            week.WeekStart = cmd.WeekStart;
            week.WeekEnd = cmd.WeekEnd;

            await _dbContext.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
