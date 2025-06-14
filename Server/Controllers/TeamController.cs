using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Server.Features.Team;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[ValidateAntiForgeryToken]
public sealed class TeamController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a list of all NFL teams.
    /// </summary>
    /// <returns>
    /// Returns a 200 OK response with a <see cref="TeamSearchResponse"/> containing all teams.
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the request is invalid.
    /// </returns>
    [HttpGet("")]
    [Authorize]
    [ProducesResponseType(typeof(TeamSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTeamList()
    {
        var result = await _mediator.Send(new TeamSearch.Query(), HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem)
        );
    }

    /// <summary>
    /// Retrieves details for a specific NFL team by its ID.
    /// </summary>
    /// <param name="teamId">The unique identifier of the team.</param>
    /// <returns>
    /// Returns a 200 OK response with a <see cref="Team"/> if the team is found.
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the request is invalid.
    /// Returns a 404 Not Found response with a <see cref="NotFoundProblemDetails"/> if the team does not exist.
    /// </returns>
    [HttpGet("{teamId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeam([FromRoute] int teamId)
    {
        var result = await _mediator.Send(new TeamSearch.Query() 
        { 
            TeamId = teamId 
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success =>
            {
                var team = success.Teams.FirstOrDefault(t => t.TeamId == teamId);
                if (team is null)
                {
                    return NotFound(new NotFoundProblemDetails($"Team with ID {teamId} was not found."));
                }

                return Ok(team);
            },
            validationProblem => BadRequest(validationProblem)
        );
    }
}
