using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Enums;
using WebApp.Common.Models;
using WebApp.Database;

namespace WebApp.Server.Services.SeasonService.Query;

public class SeasonWeekList
{
    public class Query : IRequest<GetSeasonWeekListResponse>
    {
        public int SeasonId { get; set; }
        public WeekType? WeekType { get; set; }
    }

    public class Handler : IRequestHandler<Query, GetSeasonWeekListResponse>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetSeasonWeekListResponse> Handle(Query query, CancellationToken token)
        {
            var weekQuery =
                from sw in _dbContext.SeasonWeeks.AsNoTracking()
                where sw.SeasonId == query.SeasonId
                select new SeasonWeek
                {
                    SeasonWeekId = sw.SeasonWeekId,
                    SeasonId = sw.SeasonId,
                    Week = sw.Week,
                    WeekType = sw.SeasonWeekTypeName,
                    Description = sw.Description,
                    WeekStart = sw.WeekStart,
                    WeekEnd = sw.WeekEnd
                };

            weekQuery = AddFilters(query, weekQuery);

            var seasonWeeks = await weekQuery.ToArrayAsync(token);

            return new GetSeasonWeekListResponse
            {
                SeasonWeeks = seasonWeeks,
            };
        }

        private static IQueryable<SeasonWeek> AddFilters(Query query, IQueryable<SeasonWeek> seasonWeekQuery)
        {
            if (query.WeekType != null)
            {
                seasonWeekQuery = seasonWeekQuery.Where(x => x.WeekType == query.WeekType);
            }

            return seasonWeekQuery;
        }
    }
}
