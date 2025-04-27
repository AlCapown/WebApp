using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;

namespace WebApp.Server.Services.InviteCodeService.Query;

public class InviteCodeIsValid
{
    public class Query : IRequest<Result>
    {
        public string InviteCode { get; set; }
    }

    public class Result
    {
        public bool IsValid { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly WebAppDbContext _dbContext;

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Query query, CancellationToken token)
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
