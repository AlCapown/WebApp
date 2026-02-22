# nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using OneOf;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Enums;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Features.Season;

using Result = OneOf<SeasonWeekSearchResponse, ValidationProblemDetails>;

public static class SeasonWeekSearch
{
    public sealed record Query : IRequest<Result>
    {
        public int SeasonId { get; init; }
        public int? SeasonWeekId { get; init; }
        public int? Week { get; init; }
        public WeekType? WeekType { get; init; }
    }

    public sealed class SeasonWeekListValidator : AbstractValidator<Query>
    {
        public SeasonWeekListValidator()
        {
            RuleFor(x => x.SeasonId)
                .GreaterThan(0);

            RuleFor(x => x.SeasonWeekId)
                .GreaterThan(0)
                .When(x => x.SeasonWeekId.HasValue);

            RuleFor(x => x.Week)
                .GreaterThan(0)
                .When(x => x.Week.HasValue);

            RuleFor(x => x.WeekType)
                .IsInEnum()
                .When(x => x.WeekType.HasValue);
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
            cancellationToken.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = _validator.ValidateRequest(query);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            var seasonWeeks = await _cache.GetOrCreateAsync
            (
                key: $"{nameof(SeasonWeekSearch)}_{query.SeasonId}",
                state: (DbContext: _dbContext, query.SeasonId),
                factory: static async (state, cancellationToken) => await GetSeasonWeeksAsync(state.DbContext, state.SeasonId, cancellationToken),
                options: CacheOptions.STANDARD_L20_D60,
                tags: null,
                cancellationToken: cancellationToken
            );

            return new SeasonWeekSearchResponse
            {
                SeasonWeeks = [.. Filter(seasonWeeks, query)]
            };
        }

        private static async Task<SeasonWeek[]> GetSeasonWeeksAsync(WebAppDbContext dbContext, int seasonId,  CancellationToken cancellationToken)
        {
            return await dbContext.SeasonWeeks
                .AsNoTracking()
                .Where(x => x.SeasonId == seasonId)
                .Select(x => new SeasonWeek
                {
                    SeasonWeekId = x.SeasonWeekId,
                    SeasonId = x.SeasonId,
                    Week = x.Week,
                    WeekType = x.SeasonWeekTypeName,
                    Description = x.Description,
                    WeekStart = x.WeekStart,
                    WeekEnd = x.WeekEnd
                })
                .ToArrayAsync(cancellationToken);
        }

        private static IEnumerable<SeasonWeek> Filter(IEnumerable<SeasonWeek> seasonWeekQuery, Query query)
        {
            if(query.SeasonWeekId.HasValue)
            {
                seasonWeekQuery = seasonWeekQuery.Where(x => x.SeasonWeekId == query.SeasonWeekId);
            }

            if (query.Week.HasValue)
            {
                seasonWeekQuery = seasonWeekQuery.Where(x => x.Week == query.Week);
            }

            if (query.WeekType.HasValue)
            {
                seasonWeekQuery = seasonWeekQuery.Where(x => x.WeekType == query.WeekType);
            }

            return seasonWeekQuery;
        }
    }
}
