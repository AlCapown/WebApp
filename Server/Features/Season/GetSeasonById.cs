#nullable enable

using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Database;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Features.Season;

using Result = OneOf<Common.Models.Season, NotFoundProblemDetails>;

public static class GetSeasonById
{
    public sealed record Query : IRequest<Result>
    {
        public int SeasonId { get; init; }
    }

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<Result> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var season = await _dbContext.Seasons
                .AsNoTracking()
                .Where(x => x.SeasonId == query.SeasonId)
                .Select(x => new Common.Models.Season
                {
                    SeasonId = x.SeasonId,
                    Description = x.Description,
                    IsCurrent = x.SeasonId == SeasonConstants.CURRENT_SEASON_ID,
                })
                .FirstOrDefaultAsync(token);

            if (season is null)
            {
               return new NotFoundProblemDetails($"Season with ID {query.SeasonId} not found.");
            }

            return season;
        }
    }
}
