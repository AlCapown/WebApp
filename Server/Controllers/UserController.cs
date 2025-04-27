#nullable enable

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Server.Services.AccountService.Query;

namespace WebApp.Server.Controllers;


[Route("api/[controller]")]
[ApiController]
[ValidateAntiForgeryToken]
public sealed class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CurrentUserInfoResponse), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public IActionResult GetCurrentUserInfo()
    {
        if (User?.Identity?.IsAuthenticated is null || !User.Identity.IsAuthenticated)
        {
            return Ok(new CurrentUserInfoResponse
            {
                IsAuthenticated = false
            });
        }

        string nameClaimType = ClaimTypes.Name;
        string roleClaimType = ClaimTypes.Role;

        if (User?.Identity is ClaimsIdentity claimsIdentity)
        {
            nameClaimType = claimsIdentity.NameClaimType;
            roleClaimType = claimsIdentity.RoleClaimType;
        }

        return Ok(new CurrentUserInfoResponse
        {
            IsAuthenticated = true,
            NameClaimType = nameClaimType,
            RoleClaimType = roleClaimType,
            Claims = User?.Claims.Select(x => new ClaimValue(x.Type, x.Value)).ToArray()
        });
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(GetUserByIdResponse), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetUserById([FromRoute] string userId)
    {
        var user = await _mediator.Send(new GetUser.Query
        {
            UserId = userId
        }, HttpContext.RequestAborted);

        return Ok(new GetUserByIdResponse
        {
            User = user
        });
    }
}
