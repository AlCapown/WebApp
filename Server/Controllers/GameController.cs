#nullable enable

using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Server.Features.Game;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Policy.USER)]
[ValidateAntiForgeryToken]
public sealed class GameController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Searches for games based on specified criteria.
    /// </summary>
    /// <param name="query">The search query parameters including optional filters for season, team, date range, and game status.</param>
    /// <returns>
    /// Returns a 200 OK response with a <see cref="GameSearchResponse"/> containing the list of matching games. <br/>
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the query is invalid.
    /// </returns>
    [HttpGet("Search")]
    [ProducesResponseType(typeof(GameSearchResponse), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Retrieves a specific game by its ID.
    /// </summary>
    /// <param name="gameId">The unique identifier of the game.</param>
    /// <returns>
    /// Returns a 200 OK response with the <see cref="Game"/> if found. <br/>
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the request is invalid. <br/>
    /// Returns a 404 Not Found response with a <see cref="NotFoundProblemDetails"/> if the game does not exist.
    /// </returns>
    [HttpGet("{GameId:int}")]
    [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGame([FromRoute] int gameId)
    {
        var result = await _mediator.Send(new GetGame.Query
        {
            GameId = gameId
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem),
            notFoundProblem => NotFound(notFoundProblem)
        );
    }

    /// <summary>
    /// Retrieves the game summary for a specific game.
    /// </summary>
    /// <param name="gameId">The unique identifier of the game.</param>
    /// <returns>
    /// Returns a 200 OK response with a <see cref="GameSummary"/> containing the game summary if found. <br/>
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the request is invalid. <br/>
    /// Returns a 404 Not Found response with a <see cref="NotFoundProblemDetails"/> if no summary exists for the specified game.
    /// </returns>
    [HttpGet("{GameId:int}/Summary")]
    [ProducesResponseType(typeof(GameSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameSummary([FromRoute] int gameId)
    {
        var result = await _mediator.Send(new GetGameSummary.Query
        {
            GameId = gameId
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem),
            notFoundProblem => NotFound(notFoundProblem)
        );
    }
}
