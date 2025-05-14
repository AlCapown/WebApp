#nullable enable

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Server.Services.GameService;

namespace WebApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ValidateAntiForgeryToken]
public sealed class GameController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Searches for games based on the provided query parameters.
    /// </summary>
    /// <param name="query">The search criteria, including optional filters such as SeasonId, SeasonWeekId, TeamId, and date range.</param>
    /// <returns>
    /// Returns a 200 OK response with a <see cref="SearchGamesResponse"/> containing the list of games that match the search criteria.
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the query parameters are invalid.
    /// </returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(SearchGamesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchGames([FromQuery] GameSearch.Query query)
    {
        var result = await _mediator.Send(query, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem)
        );
    }
}
