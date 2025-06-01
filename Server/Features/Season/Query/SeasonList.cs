using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Database;

namespace WebApp.Server.Features.SeasonService.Query;

public class SeasonList
{
    public class Query : IRequest<GetSeasonListResponse> { }

    public class Handler : IRequestHandler<Query, GetSeasonListResponse>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetSeasonListResponse> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var seasons = await _dbContext.Seasons
                .AsNoTracking()
                .Select(x => new Season
                {
                    SeasonId = x.SeasonId,
                    Description = x.Description,
                    IsCurrent = x.SeasonId == SeasonConstants.CURRENT_SEASON_ID,
                })
                .ToArrayAsync(token);

            return new GetSeasonListResponse
            {
                Seasons = seasons
            };
        }
    }
}
