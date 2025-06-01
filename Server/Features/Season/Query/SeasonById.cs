using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Database;

namespace WebApp.Server.Features.SeasonService.Query;

public class SeasonById
{
    public class Query : IRequest<Season>
    {
        public int SeasonId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Season>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<Season> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var season = await _dbContext.Seasons
                .AsNoTracking()
                .Where(x => x.SeasonId == query.SeasonId)
                .Select(x => new Season
                {
                    SeasonId = x.SeasonId,
                    Description = x.Description,
                    IsCurrent = x.SeasonId == SeasonConstants.CURRENT_SEASON_ID,
                })
                .FirstOrDefaultAsync(token);

            return season;
        }
    }
}
