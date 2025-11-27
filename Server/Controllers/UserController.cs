#nullable enable

using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Server.Features.Account;
using WebApp.Server.Infrastructure;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Policy.User)]
[ValidateAntiForgeryToken]
public sealed class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets information about the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Returns authentication status, claim types, and claims for the current user.
    /// If the user is not authenticated, returns IsAuthenticated = false.
    /// </remarks>
    /// <returns>
    /// <see cref="CurrentUserInfoResponse"/> containing user authentication and claim information.
    /// </returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CurrentUserInfoResponse), StatusCodes.Status200OK)]
    public IActionResult GetCurrentUserInfo()
    {
        if (User?.Identity is not ClaimsIdentity { IsAuthenticated: true } claimsIdentity)
        {
            return Ok(new CurrentUserInfoResponse
            {
                IsAuthenticated = false,
            });
        }

        return Ok(new CurrentUserInfoResponse
        {
            IsAuthenticated = true,
            NameClaimType = claimsIdentity.NameClaimType,
            RoleClaimType = claimsIdentity.RoleClaimType,
            Claims = [.. claimsIdentity.Claims
                .Where(x => x.Type != "AspNet.Identity.SecurityStamp")
                .Select(x => new ClaimValue 
                {
                    Type = x.Type,
                    Value = x.Value
                })]
        });
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to retrieve.</param>
    /// <returns>
    /// Returns <see cref="User"/> if found.<br/>
    /// Returns <see cref="ValidationProblemDetails"/> if the request is invalid.<br/>
    /// Returns <see cref="NotFoundProblemDetails"/> if the user does not exist.
    /// </returns>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById([FromRoute] string userId)
    {
        var result = await _mediator.Send(new GetUserById.Query
        {
            UserId = userId
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem),
            notFound => NotFound(notFound)
        );
    }

    /// <summary>
    /// User Search
    /// </summary>
    /// <param name="query">The search criteria for filtering users.</param>
    /// <returns>
    /// Returns <see cref="SearchUsersResponse"/> on success <br/>
    /// Returns <see cref="ValidationProblemDetails"/> if the request is invalid.<br/>
    /// </returns>
    [HttpGet("Search")]
    [Authorize(Policy = Policy.Admin)]
    [ProducesResponseType(typeof(SearchUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserSearch([FromQuery] UserSearch.Query query)
    {
        var result = await _mediator.Send(query, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem)
        );
    }
}
