using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Models;
using WebApp.Server.Services.GamePredictionService.Command;
using WebApp.Server.Services.GamePredictionService.Query;

namespace WebApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
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
    [Authorize]
    public async Task<IActionResult> GetGamePrediction([FromRoute] int gamePredictionId)
    {
        var result = await _mediator.Send(new GetGamePrediction.Query
        {
            GamePredictionId = gamePredictionId
        }, HttpContext.RequestAborted);

        return Ok(new GetGamePredictionResponse
        {
            GamePrediction = result.GamePrediction
        });
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(GamePredictionSearchResponse), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetGamePredictionSearch(
        [FromQuery] int? seasonId,
        [FromQuery] int? gameId,
        [FromQuery] string userId,
        [FromQuery] bool? limitToCurrentUser,
        [FromQuery] int? teamId)
    { 
        var result = await _mediator.Send(new GamePredictionSearch.Query
        {
            SeasonId = seasonId,
            GameId = gameId,
            UserId = userId,
            LimitToCurrentUser = limitToCurrentUser,
            TeamId = teamId
        }, HttpContext.RequestAborted);

        return Ok(result);
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
