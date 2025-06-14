using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Enums;
using WebApp.Common.Models;
using WebApp.Server.Features.Season;
using WebApp.Server.Features.SeasonService.Command;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[ValidateAntiForgeryToken]
public sealed class SeasonController : ControllerBase
{
    private readonly IMediator _mediator;

    public SeasonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("")]
    [Authorize]
    [ProducesResponseType(typeof(GetSeasonListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSeasonList()
    {
        var result = await _mediator.Send(new GetSeasonList.Query(), HttpContext.RequestAborted);
        return Ok(result);
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [Authorize(Roles = AppRole.ADMIN)]
    public async Task<IActionResult> CreateSeason([FromBody] CreateSeasonRequest body)
    {
        var result = await _mediator.Send(new CreateSeason.Command
        {
            SeasonId = body.SeasonId.Value,
            Description = body.Description
        }, HttpContext.RequestAborted);

        return CreatedAtAction(nameof(GetSeason), new { seasonId = result }, new CreateSeasonResponse { SeasonId = result });
    }

    [HttpGet("{seasonId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(Season), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSeason([FromRoute] int seasonId)
    {
        var result = await _mediator.Send(new GetSeasonById.Query
        {
            SeasonId = seasonId,
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            notFound => NotFound(notFound)
        );
    }

    [HttpPut("{seasonId:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [Authorize(Roles = AppRole.ADMIN)]
    public async Task<IActionResult> UpdateSeason([FromRoute] int seasonId, [FromBody] UpdateSeasonRequest body)
    {
        await _mediator.Send(new UpdateSeason.Command
        {
            SeasonId = seasonId,
            Description = body.Description
        }, HttpContext.RequestAborted);

        return NoContent();
    }

    [HttpGet("{seasonId:int}/week")]
    [Authorize]
    [ProducesResponseType(typeof(GetSeasonWeekListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetWeekList([FromRoute] int seasonId, [FromQuery] WeekType? weekType)
    {
        var result = await _mediator.Send(new SeasonWeekSearch.Query
        {
            SeasonId = seasonId,
            WeekType = weekType
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Ok(success),
            validationProblem => BadRequest(validationProblem)
        );
    }

    [HttpGet("{seasonId:int}/week/{SeasonWeekId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(SeasonWeek), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWeek([FromRoute] int seasonId, [FromRoute] int seasonWeekId)
    {
        var result = await _mediator.Send(new SeasonWeekSearch.Query
        {
            SeasonId = seasonId,
            SeasonWeekId = seasonWeekId
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success =>
            {
                if(success is null)
                {
                    return NotFound(new NotFoundProblemDetails(""));
                }

                return Ok(success);
            },
            badRequest => BadRequest(badRequest)
        );
    }

    [HttpPut("{seasonId:int}/week/{SeasonWeekId:int}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [Authorize(Roles = AppRole.ADMIN)]
    public async Task<IActionResult> UpdateWeek([FromRoute] int seasonId, [FromRoute] int seasonWeekId, [FromBody] UpdateSeasonWeekRequest request)
    {
        await _mediator.Send(new UpdateWeekForSeason.Command
        {
            SeasonId = seasonId,
            SeasonWeekId = seasonWeekId,
            WeekStart = request.WeekStart.Value,
            WeekEnd = request.WeekEnd.Value
        }, HttpContext.RequestAborted);

        return NoContent();
    }
}
