using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Enums;
using WebApp.Database;
using WebApp.Server.Infrastructure.Exceptions;

namespace WebApp.Server.Features.SeasonService.Command;

public class UpsertWeekForSeason
{
    public class Command : IRequest<Unit>
    {
        public int SeasonId { get; set; }
        public int Week { get; set; }
        public WeekType WeekType { get; set; }
        public DateOnly WeekStart { get; set; }
        public DateOnly WeekEnd { get; set; }
        public string Description { get; set; }
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

            var season = await _dbContext.Seasons
                .FirstOrDefaultAsync(x => x.SeasonId == cmd.SeasonId, token);

            if (season == null)
            {
                throw new WebAppValidationException(nameof(Command.SeasonId), $"The {cmd.SeasonId} season does not exist.");
            }

            var week = await _dbContext.SeasonWeeks
                .FirstOrDefaultAsync(x =>
                    x.SeasonId == cmd.SeasonId &&
                    x.Week == cmd.Week &&
                    x.SeasonWeekTypeName == cmd.WeekType, token);

            if (week == null)
            {
                var seasonWeek = new Database.Tables.SeasonWeek
                {
                    SeasonId = cmd.SeasonId,
                    Week = cmd.Week,
                    SeasonWeekTypeName = cmd.WeekType,
                    WeekStart = cmd.WeekStart,
                    WeekEnd = cmd.WeekEnd,
                    Description = cmd.Description,
                };

                _dbContext.SeasonWeeks.Add(seasonWeek);
            }
            else
            {
                week.WeekStart = cmd.WeekStart;
                week.WeekEnd = cmd.WeekEnd;
                week.Description = cmd.Description;
            }

            await _dbContext.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
