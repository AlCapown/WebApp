using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Server.Infrastructure;
using WebApp.Server.Services.GamePredictionService;
using WebApp.Server.Services.GamePredictionService.Command;

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
    [ProducesResponseType(typeof(CreateGamePredictionResponse), StatusCodes.Status201Created)]
    [Authorize]
    public async Task<IActionResult> CreateGamePrediction([FromBody] CreateGamePredictionRequest body)
    {
        var result = await _mediator.Send(new UpsertGamePrediction.Command
        {
            GameId = body.GameId.Value,
            HomeTeamScore = body.HomeTeamScore.Value,
            AwayTeamScore = body.AwayTeamScore.Value,
            UserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
        }, HttpContext.RequestAborted);

        return CreatedAtAction(
            nameof(GetGamePrediction), 
            new 
            { 
                result.GamePredictionId
            }, 
            new CreateGamePredictionResponse 
            { 
                GamePredictionId = result.GamePredictionId 
            });
    }

    [HttpPost("{userId}")]
    [ProducesResponseType(typeof(CreateGamePredictionResponse), StatusCodes.Status201Created)]
    [Authorize(Roles = AppRole.ADMIN)]
    public async Task<IActionResult> CreateGamePredictionForUser([FromRoute] string userId, [FromBody] CreateGamePredictionRequest body)
    {
        var result = await _mediator.Send(new UpsertGamePrediction.Command
        {
            GameId = body.GameId.Value,
            HomeTeamScore = body.HomeTeamScore.Value,
            AwayTeamScore = body.AwayTeamScore.Value,
            UserId = userId,
            BypassGameStartTimeValidation = true
        }, HttpContext.RequestAborted);

        return CreatedAtAction(
            nameof(GetGamePrediction),
            new
            {
                result.GamePredictionId
            },
            new CreateGamePredictionResponse
            {
                GamePredictionId = result.GamePredictionId
            });
    }
}
