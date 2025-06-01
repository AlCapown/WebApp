#nullable enable

using Mediator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;

namespace WebApp.Server.Features.InviteCode;

public static class InviteCodeIsValid
{
    public sealed record Query : IRequest<Result>
    {
        public required string InviteCode { get; init; }
    }

    public sealed record Result
    {
        public bool IsValid { get; init; }
    }

    public sealed class Handler : IRequestHandler<Query, Result>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<Result> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var isValid = await _dbContext.InviteCodes.AsNoTracking()
                .Where(x => x.Code == query.InviteCode)
                .Where(x => x.Expires > DateTimeOffset.Now)
                .AnyAsync(token);

            return new Result
            {
                IsValid = isValid
            };
        }
    }
}
