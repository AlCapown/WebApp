using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Enums;
using WebApp.Common.Models;
using WebApp.Database;

namespace WebApp.Server.Features.SeasonService.Query;

public class GetSeasonWeekByWeekAndWeekType
{
    public class Query : IRequest<SeasonWeek>
    {
        public int SeasonId { get; set; }
        public int Week { get; set; }
        public WeekType WeekType { get; set; }
    }

    public class Handler : IRequestHandler<Query, SeasonWeek>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SeasonWeek> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var weekQuery =
                from sw in _dbContext.SeasonWeeks.AsNoTracking()
                where sw.SeasonId == query.SeasonId &&
                      sw.Week == query.Week &&
                      sw.SeasonWeekTypeName == query.WeekType
                select new SeasonWeek
                {
                    SeasonWeekId = sw.SeasonWeekId,
                    SeasonId = sw.SeasonId,
                    Week = sw.Week,
                    WeekType = sw.SeasonWeekTypeName,
                    WeekStart = sw.WeekStart,
                    WeekEnd = sw.WeekEnd
                };

            return await weekQuery.FirstOrDefaultAsync(token);
        }
    }
}
