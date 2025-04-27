using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Database.Tables;

namespace WebApp.Server.Services.AccountService.Query;

public class GetUser
{
    public class Query : IRequest<User> 
    {
        public string UserId { get; set; }
    }


    public class Handler : IRequestHandler<Query, User>
    {
        private readonly UserManager<AppUser> _userManager;

        public Handler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(query.UserId) 
                ?? throw new InvalidOperationException("User Not Found");

            return new User
            {
                UserId = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }
    }
}
