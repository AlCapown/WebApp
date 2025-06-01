using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Database;

namespace WebApp.Server.Features.TeamService.Query;

public class GetTeamByAbbreviation
{
    public class Query : IRequest<Team>
    {
        public string Abbreviation { get; set; }
    }

    public class Handler : IRequestHandler<Query, Team>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<Team> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var team = await _dbContext.Teams
                .AsNoTracking()
                .Where(x => x.Abbreviation == query.Abbreviation)
                .Select(x => new Team
                {
                    TeamId = x.TeamId,
                    TeamFullName = x.TeamFullName,
                    TeamName = x.TeamName,
                    Abbreviation = x.Abbreviation,
                    BrandingLogo = x.BrandingLogo,
                    Division = x.Division.DivisionName,
                    Conference = x.Division.Conference.ConferenceName
                })
                .FirstOrDefaultAsync(cancellationToken: token);

            return team;
        }
    }
}
