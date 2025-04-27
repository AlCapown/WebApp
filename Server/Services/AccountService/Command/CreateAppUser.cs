using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Extensions;
using WebApp.Database.Tables;

namespace WebApp.Server.Services.AccountService.Command;

public class CreateAppUser
{
    public class Command : IRequest<AppUser>
    {
        public ExternalLoginInfo ExternalLoginInfo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Handler : IRequestHandler<Command, AppUser>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<CreateAppUser> _logger;

        public Handler(UserManager<AppUser> userManager, ILogger<CreateAppUser> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<AppUser> Handle(Command command, CancellationToken token)
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
