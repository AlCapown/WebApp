#nullable enable

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneOf;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Extensions;
using WebApp.Database.Tables;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Services.AccountService.Command;

using Result = OneOf<AppUser, ValidationProblemDetails, ForbiddenProblemDetails>;

public sealed class CreateAppUser
{
    public record Command : IRequest<Result>
    {
        public required ExternalLoginInfo ExternalLoginInfo { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
    }

    public sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<CreateAppUser> _logger;

        public Handler(UserManager<AppUser> userManager, ILogger<CreateAppUser> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result> Handle(Command command, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var appUser = new AppUser
            {
                UserName = command.ExternalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                Email = command.ExternalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                FirstName = command.FirstName ?? command.ExternalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName)?.ToTitleCase(),
                LastName = command.LastName ?? command.ExternalLoginInfo.Principal.FindFirstValue(ClaimTypes.Surname)?.ToTitleCase()
            };

            // Creates the AspNetUsers profile without a password
            var createResult = await _userManager.CreateAsync(appUser);
            if (!createResult.Succeeded)
            {
                _logger.LogError("Failed to create new user profile for {UserName}", appUser.UserName);
                throw new Exception("Failed to create user");
            }

            // Registers the external login provider as a login method
            var createLoginResult = await _userManager.AddLoginAsync(appUser, command.ExternalLoginInfo);
            if (!createLoginResult.Succeeded)
            {
                _logger.LogError("Failed to create login provider for {UserName}", appUser.UserName);
                throw new Exception("Failed to create user");
            }

            // Adds default user role
            var addRoleResult = await _userManager.AddToRoleAsync(appUser, AppRole.USER);
            if (!addRoleResult.Succeeded)
            {
                _logger.LogError("Failed to create a default app role for {UserName}", appUser.UserName);
                throw new Exception("Failed to create user");
            }

            _logger.LogInformation("Successfully created new account for {UserName}", appUser.UserName);

            // User manager fills in some of the missing properties that weren't specified in the original create
            // so we want to do a re-fetch to return the most up to date result. 
            return await _userManager.FindByIdAsync(appUser.Id);
        }
    }
}
