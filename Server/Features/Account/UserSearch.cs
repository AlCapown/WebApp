#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Database.Tables;
using WebApp.Server.Infrastructure;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Features.Account;

using Result = OneOf<UserSearch.Response, ValidationProblemDetails>;

public static class UserSearch
{
    public sealed record Query : IRequest<Result>
    {
        public string? UserId { get; init; }
    }
    
    public sealed record Response
    {
        public required User[] Users { get; init; }
    }

    public sealed class UserSearchValidator : AbstractValidator<Query>
    {
        public UserSearchValidator()
        {
        }
    }

    public sealed class Handler : IRequestHandler<Query, Result>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IValidator<Query> _validator;
        private readonly WebAppDbContext _dbContext;

        public Handler(UserManager<AppUser> userManager, IValidator<Query> validator, WebAppDbContext dbContext)
        {
            _userManager = userManager;
            _validator = validator;
            _dbContext = dbContext;

        }

        public async ValueTask<Result> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = _validator.ValidateRequest(query);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            // WIP
            var dbQuery = _dbContext.Users.AsNoTracking()
                .Select(user => new User
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsAdmin = false
                });

            var queryResult = await dbQuery.ToArrayAsync(token);

            return new Response
            {
                Users = queryResult
            };
        }
    }
}
