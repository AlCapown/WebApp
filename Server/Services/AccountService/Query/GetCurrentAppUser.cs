using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Database.Tables;

namespace WebApp.Server.Services.AccountService.Query;

public class GetCurrentAppUser
{
    public class Query : IRequest<Response> { }

    public class Response
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class Handler : IRequestHandler<Query, Response>
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
                ?? throw new InvalidOperationException("No user information in the current HttpContext.");

            var user = await _userManager.GetUserAsync(claimsPrincipal)
                ?? throw new InvalidOperationException("No information about the current user could be found.");

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
