using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Server.Services.GameService.Query;

namespace WebApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[ValidateAntiForgeryToken]
public sealed class GameController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(SearchGamesResponse), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> SearchGames(
        [FromQuery] int? seasonWeekId,
        [FromQuery] int? seasonId,
        [FromQuery] int? teamId,
        [FromQuery] DateTimeOffset? gameStartsOnMin,
        [FromQuery] DateTimeOffset? gameStartsOnMax,
        [FromQuery] bool? isGameComplete)
    {      
        var result = await _mediator.Send(new SearchGames.Query
        {
            SeasonWeekId = seasonWeekId,
            SeasonId = seasonId,
            TeamId = teamId,
            GameStartsOnMin = gameStartsOnMin,
            GameStartsOnMax = gameStartsOnMax,
            IsGameComplete = isGameComplete
        }, HttpContext.RequestAborted);

        return Ok(result);
    }
}
