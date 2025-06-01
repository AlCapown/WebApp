#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Database.Tables;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Features.Account;

using Result = OneOf<User, ValidationProblemDetails, NotFoundProblemDetails>;

public static class GetUserById
{
    public sealed record Query : IRequest<Result> 
    {
        public string? UserId { get; init; }
    }

    public sealed class GetUserValidator : AbstractValidator<Query>
    {
        public GetUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Query, Result>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IValidator<Query> _validator;

        public Handler(UserManager<AppUser> userManager, IValidator<Query> validator)
        {
            _userManager = userManager;
            _validator = validator;
        }

        public async ValueTask<Result> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = _validator.ValidateRequest(query);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            var appUser = await _userManager.FindByIdAsync(query.UserId!);

            if (appUser is null)
            {
                return new NotFoundProblemDetails("User not found.");
            }

            var isAdmin = await _userManager.IsInRoleAsync(appUser, AppRole.ADMIN);

            return new User
            {
                UserId = appUser.Id,
                UserName = appUser.UserName,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                IsAdmin = isAdmin
            };
        }
    }
}
