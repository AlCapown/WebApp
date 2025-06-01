using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Database;

namespace WebApp.Server.Features.SeasonService.Query;

public class GetSeasonWeek
{
    public class Query : IRequest<SeasonWeek>
    {
        public int SeasonId { get; set; }
        public int SeasonWeekId { get; set; }
    }

    public class Handler : IRequestHandler<Query, SeasonWeek>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<SeasonWeek> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var weekQuery =
                from sw in _dbContext.SeasonWeeks.AsNoTracking()
                where sw.SeasonId == query.SeasonId &&
                      sw.SeasonWeekId == query.SeasonWeekId
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
