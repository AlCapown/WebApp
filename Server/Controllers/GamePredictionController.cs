#nullable enable

using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Client.Common.Extensions;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Server.Features.GamePrediction;
using WebApp.Server.Infrastructure;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Policy.User)]
[ValidateAntiForgeryToken]
public sealed class GamePredictionController : ControllerBase
{
    private readonly IMediator _mediator;

    public GamePredictionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a specific game prediction by its unique identifier.
    /// </summary>
    /// <param name="gamePredictionId">The unique identifier of the game prediction.</param>
    /// <returns>
    /// 200 OK with the game prediction if found.<br/>
    /// 400 Bad Request if the request is invalid.<br/>
    /// 404 Not Found if the game prediction does not exist.
    /// </returns>
    [HttpGet("{gamePredictionId:int}")]
    [ProducesResponseType(typeof(GetGamePredictionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGamePrediction([FromRoute] int gamePredictionId)
    {
        var result = await _mediator.Send(new GetGamePrediction.Query
        {
            GamePredictionId = gamePredictionId
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem),
            notFound => NotFound(notFound)
        );
    }

    /// <summary>
    /// Searches for user game predictions based on the provided query parameters.
    /// </summary>
    /// <param name="query">The search criteria for filtering game predictions.</param>
    /// <returns>
    /// 200 OK with the search results.<br/>
    /// 400 Bad Request if the query is invalid.<br/>
    /// 404 Not Found if no predictions match the criteria.
    /// </returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(GamePredictionSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGamePredictionSearch([FromQuery] GamePredictionSearch.Query query)
    { 
        var result = await _mediator.Send(query, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem),
            notFound => NotFound(notFound)
        );
    }

    /// <summary>
    /// Creates or updates a game prediction for the current authenticated user.
    /// </summary>
    /// <param name="body">The prediction details to create or update.</param>
    /// <returns>
    /// 200 OK with the created or updated prediction.<br/>
    /// 400 Bad Request if the request is invalid.<br/>
    /// 403 Forbidden if the operation is not allowed.
    /// </returns>
    [HttpPost("")]
    [ProducesResponseType(typeof(CreateGamePredictionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ForbiddenProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateGamePrediction([FromBody] CreateGamePredictionRequest body)
    {
        var result = await _mediator.Send(new UpsertGamePrediction.Command
        {
            GameId = body.GameId,
            HomeTeamScore = body.HomeTeamScore,
            AwayTeamScore = body.AwayTeamScore,
            UserId = HttpContext.User.GetUserId(),
            BypassGameStartTimeValidation = false
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem),
            forbiddenProblem => StatusCode(StatusCodes.Status403Forbidden, forbiddenProblem)
        );
    }

    /// <summary>
    /// Creates or updates a game prediction for a specified user. Only accessible by administrators.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom the prediction is being created or updated.</param>
    /// <param name="body">The prediction details to create or update.</param>
    /// <returns>
    /// 200 OK with the created or updated prediction.<br/>
    /// 400 Bad Request if the request is invalid.<br/>
    /// 403 Forbidden if the operation is not allowed.
    /// </returns>
    [HttpPost("{userId}")]
    [Authorize(Policy = Policy.Admin)]
    [ProducesResponseType(typeof(CreateGamePredictionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ForbiddenProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateGamePredictionForUser([FromRoute] string userId, [FromBody] CreateGamePredictionRequest body)
    {
        var result = await _mediator.Send(new UpsertGamePrediction.Command
        {
            GameId = body.GameId,
            HomeTeamScore = body.HomeTeamScore,
            AwayTeamScore = body.AwayTeamScore,
            UserId = userId,
            BypassGameStartTimeValidation = true
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem),
            forbiddenProblem => StatusCode(StatusCodes.Status403Forbidden, forbiddenProblem)
        );
    }
}
