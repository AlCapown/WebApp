
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
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Features.Account;

using Result = OneOf<SearchUsersResponse, ValidationProblemDetails>;

public static class UserSearch
{
    public sealed record Query : IRequest<Result>
    {
        /// <summary>
        /// Optional search paramter to search against the unique identifier of the user.
        /// </summary>
        public string? UserId { get; init; }
    }
    
    public sealed class UserSearchValidator : AbstractValidator<Query>
    {
        public UserSearchValidator()
        {
            RuleFor(x => x.UserId)
                .MaximumLength(450)
                .When(x => x.UserId is not null);
        }
    }

    public sealed class Handler : IRequestHandler<Query, Result>
    {
        private readonly IValidator<Query> _validator;
        private readonly WebAppDbContext _dbContext;

        public Handler(IValidator<Query> validator, WebAppDbContext dbContext)
        {
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

            var userQuery = _dbContext.Users.AsNoTracking()
                .Where(x => x.UserName != null)
                .Where(x => x.Email != null)
                .Select(user => new User
                {
                    UserId = user.Id,
                    UserName = user.UserName!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    IsAdmin = user.UserRoles.Any(ur => ur.RoleId == AppRole.ADMIN)
                });

            userQuery = AddFilters(userQuery, query);

            var users = await userQuery.ToArrayAsync(token);

            return new SearchUsersResponse
            {  
                Users = users
            };
        }
    }

    private static IQueryable<User> AddFilters(IQueryable<User> userQuery, Query query)
    {
        if (query.UserId is not null)
        {
            userQuery = userQuery.Where(x => x.UserId == query.UserId);
        }

        return userQuery;
    }
}
