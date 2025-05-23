#nullable enable

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OneOf;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Database.Tables;

namespace WebApp.Server.Services.AccountService.Query;


public static class GetCurrentAppUser
{
    public sealed record Query : IRequest<Response> { }

    public record Response
    {
        public required string UserId { get; init; }
        public string? UserName { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public string? Email { get; init; }
        public bool IsAdmin { get; init; }
    }

    public sealed class Handler : IRequestHandler<Query, Response>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public Handler(IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task<Response> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var claimsPrincipal = _contextAccessor.HttpContext?.User
                ?? throw new InvalidOperationException("Current User Not Found.");

            var user = await _userManager.GetUserAsync(claimsPrincipal)
                ?? throw new InvalidOperationException("Current User Not Found.");

            return new Response
            {
                UserId = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsAdmin = claimsPrincipal.IsInRole(AppRole.ADMIN)
            };
        }
    }
}
