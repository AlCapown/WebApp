using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Server.Features.TeamService.Query;

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
    /// Get all NFL teams
    /// </summary>
    /// <returns></returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(GetTeamsResponse), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetTeamList()
    {
        var result = await _mediator.Send(new GetTeamList.Query(), HttpContext.RequestAborted);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific NFL team
    /// </summary>
    /// <param name="teamId">Id of the team</param>
    /// <returns></returns>
    [HttpGet("{teamId:int}")]
    [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetTeam([FromRoute] int teamId)
    {
        var result = await _mediator.Send(new GetTeamById.Query
        {
            TeamId = teamId
        }, HttpContext.RequestAborted);

        return Ok(result);
    }
}
