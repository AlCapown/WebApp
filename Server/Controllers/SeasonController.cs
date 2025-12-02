#nullable enable

using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Enums;
using WebApp.Common.Models;
using WebApp.Server.Features.Season;
using WebApp.Server.Infrastructure;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Policy.USER)]
[ValidateAntiForgeryToken]
public sealed class SeasonController : ControllerBase
{
    private readonly IMediator _mediator;

    public SeasonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a list of all seasons.
    /// </summary>
    /// <returns>
    /// Returns a 200 OK response with a <see cref="GetSeasonListResponse"/> containing the list of seasons.
    /// </returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(GetSeasonListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSeasonList()
    {
        var result = await _mediator.Send(new GetSeasonList.Query(), HttpContext.RequestAborted);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new season.
    /// </summary>
    /// <param name="body">The season creation request payload.</param>
    /// <returns>
    /// Returns a 201 Created response on success. <br/>
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the request is invalid.
    /// </returns>
    [HttpPost("")]
    [Authorize(Policy = Policy.ADMIN)]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSeason([FromBody] CreateSeasonRequest body)
    {
        var result = await _mediator.Send(new CreateSeason.Command
        {
            SeasonId = body.SeasonId,
            Description = body.Description
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => Created(),
            validation => BadRequest(validation)
        );
    }

    /// <summary>
    /// Retrieves a specific season by its ID.
    /// </summary>
    /// <param name="seasonId">The unique identifier of the season.</param>
    /// <returns>
    /// Returns a 200 OK response with the <see cref="Season"/> if found. <br/>
    /// Returns a 404 Not Found response with a <see cref="NotFoundProblemDetails"/> if the season does not exist.
    /// </returns>
    [HttpGet("{seasonId:int}")]
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

    /// <summary>
    /// Updates the details of a specific season.
    /// </summary>
    /// <param name="seasonId">The unique identifier of the season to update.</param>
    /// <param name="body">The updated season data.</param>
    /// <returns>
    /// Returns a 204 No Content response on success. <br/>
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the request is invalid. <br/>
    /// Returns a 404 Not Found response with a <see cref="NotFoundProblemDetails"/> if the season does not exist.
    /// </returns>
    [HttpPut("{seasonId:int}")]
    [Authorize(Policy = Policy.ADMIN)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSeason([FromRoute] int seasonId, [FromBody] UpdateSeasonRequest body)
    {
        var result = await _mediator.Send(new UpdateSeason.Command
        {
            SeasonId = seasonId,
            Description = body.Description
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => NoContent(),
            validationProblem => BadRequest(validationProblem),
            notFound => NotFound(notFound)
        );
    }

    /// <summary>
    /// Retrieves a list of weeks for a specific season, optionally filtered by week type.
    /// </summary>
    /// <param name="seasonId">The unique identifier of the season.</param>
    /// <param name="weekType">Optional week type filter.</param>
    /// <returns>
    /// Returns a 200 OK response with a <see cref="SeasonWeekSearchResponse"/> containing the list of weeks. <br/>
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the query is invalid.
    /// </returns>
    [HttpGet("{seasonId:int}/week")]
    [ProducesResponseType(typeof(SeasonWeekSearchResponse), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Retrieves a specific week for a season by its ID.
    /// </summary>
    /// <param name="seasonId">The unique identifier of the season.</param>
    /// <param name="seasonWeekId">The unique identifier of the season week.</param>
    /// <returns>
    /// Returns a 200 OK response with the <see cref="SeasonWeek"/> if found. <br/>
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the update is invalid. <br/>
    /// Returns a 404 Not Found response with a <see cref="NotFoundProblemDetails"/> if the week does not exist.
    /// </returns>
    [HttpGet("{seasonId:int}/week/{SeasonWeekId:int}")]
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
                    return NotFound(new NotFoundProblemDetails($"The season with the ID {seasonId} does not have a week with the ID {seasonWeekId}."));
                }

                return Ok(success);
            },
            badRequest => BadRequest(badRequest)
        );
    }

    /// <summary>
    /// Updates the details of a specific week for a season.
    /// </summary>
    /// <param name="seasonId">The unique identifier of the season.</param>
    /// <param name="seasonWeekId">The unique identifier of the season week to update.</param>
    /// <param name="request">The updated week data.</param>
    /// <returns>
    /// Returns a 204 No Content response on success. <br/>
    /// Returns a 400 Bad Request response with a <see cref="ValidationProblemDetails"/> if the request is invalid. <br/>
    /// Returns a 404 Not Found response with a <see cref="NotFoundProblemDetails"/> if the week does not exist.
    /// </returns>
    [HttpPut("{seasonId:int}/week/{SeasonWeekId:int}")]
    [Authorize(Policy = Policy.ADMIN)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWeek([FromRoute] int seasonId, [FromRoute] int seasonWeekId, [FromBody] UpdateSeasonWeekRequest request)
    {
        var result = await _mediator.Send(new UpdateWeekForSeason.Command
        {
            SeasonId = seasonId,
            SeasonWeekId = seasonWeekId,
            WeekStart = request.WeekStart,
            WeekEnd = request.WeekEnd
        }, HttpContext.RequestAborted);

        return result.Match<IActionResult>
        (
            success => NoContent(),
            badRequest => BadRequest(badRequest),
            notFound => NotFound(notFound)
        );
    }
}
