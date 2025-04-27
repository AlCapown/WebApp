using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Database;

namespace WebApp.Server.Services.TeamService.Query;

public class GetTeamList
{
    public class Query : IRequest<GetTeamsResponse> { }

    public class Handler : IRequestHandler<Query, GetTeamsResponse>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetTeamsResponse> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var teams = await _dbContext.Teams
                .AsNoTracking()
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
                .ToArrayAsync(token);

            return new GetTeamsResponse
            {
                Teams = teams
            };
        }
    }
}
