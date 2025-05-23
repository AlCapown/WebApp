#nullable enable

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Client.Common.Extensions;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Server.Infrastructure;
using WebApp.Server.Services.GamePredictionService;

namespace WebApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ValidateAntiForgeryToken]
public sealed class GamePredictionController : ControllerBase
{
    private readonly IMediator _mediator;

    public GamePredictionController(IMediator mediator)
    {
        _mediator = mediator;
    }

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


    [HttpPost("{userId}")]
    [Authorize(Roles = AppRole.ADMIN)]
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
