#nullable enable

using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Database;

namespace WebApp.Server.Features.Season;

public static class GetSeasonList
{
    public sealed record Query : IRequest<GetSeasonListResponse> { }

    public sealed class Handler : IRequestHandler<Query, GetSeasonListResponse>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<GetSeasonListResponse> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var seasons = await _dbContext.Seasons
                .AsNoTracking()
                .Select(x => new Common.Models.Season
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
