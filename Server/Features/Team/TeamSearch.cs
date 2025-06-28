#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Extensions;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Features.Team;

using Result = OneOf<TeamSearchResponse, ValidationProblemDetails>;

public static class TeamSearch
{
    public sealed record Query : IRequest<Result> 
    {
        public int? TeamId { get; init; }
        public string? Abbreviation { get; init; }
    }

    public sealed class TeamSearchValidator : AbstractValidator<Query>
    {
        public TeamSearchValidator()
        {
            RuleFor(x => x.TeamId)
                .GreaterThan(0)
                .When(x => x.TeamId.HasValue);

            RuleFor(x => x.Abbreviation)
                .MaximumLength(3)
                .When(x => !x.Abbreviation.IsNullOrWhiteSpace());
        }
    }

    public sealed class Handler : IRequestHandler<Query, Result>
    {
        private readonly WebAppDbContext _dbContext;
        private readonly HybridCache _cache;
        private readonly IValidator<Query> _validator;

        public Handler(WebAppDbContext dbContext, HybridCache cache, IValidator<Query> validator)
        {
            _dbContext = dbContext;
            _cache = cache;
            _validator = validator;
        }

        public async ValueTask<Result> Handle(Query query, CancellationToken cancellationToken)
        {
            ValidationProblemDetails? problemDetails = _validator.ValidateRequest(query);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            var teams = await _cache.GetOrCreateAsync
            (
                key: nameof(TeamSearch),
                state: (_dbContext),
                factory: static async (dbContext, token) => await GetTeamListAsync(dbContext, token),
                options: new HybridCacheEntryOptions
                {
                    LocalCacheExpiration = TimeSpan.FromMinutes(20),
                    Expiration = TimeSpan.FromMinutes(60),
                },
                tags: null,
                cancellationToken: cancellationToken
            );

            return new TeamSearchResponse
            {
                Teams = [.. AddFilters(teams, query)]
            };
        }

        private static async Task<Common.Models.Team[]> GetTeamListAsync(WebAppDbContext dbContext, CancellationToken token)
        {
            return await dbContext.Teams
                .AsNoTracking()
                .Select(x => new Common.Models.Team
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
        }

        private static IEnumerable<Common.Models.Team> AddFilters(IEnumerable<Common.Models.Team> teams, Query query)
        {
            if (query.TeamId.HasValue)
            {
                teams = teams.Where(x => x.TeamId == query.TeamId.Value);
            }

            if (!query.Abbreviation.IsNullOrWhiteSpace())
            {
                teams = teams.Where(x => x.Abbreviation.Equals(query.Abbreviation, StringComparison.OrdinalIgnoreCase));
            }

            return teams;
        }
    }
}
