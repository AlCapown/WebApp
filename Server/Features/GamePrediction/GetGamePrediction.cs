#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Extensions;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Features.GamePrediction;

using Result = OneOf<GetGamePredictionResponse, ValidationProblemDetails, NotFoundProblemDetails>;

public static class GetGamePrediction
{
    public sealed record Query : IRequest<Result>
    {
        /// <summary>
        /// Unique identifier for the game prediction.
        /// </summary>
        public int GamePredictionId { get; init; }
    }

    public sealed class GetGamePredictionValidator : AbstractValidator<Query>
    {
        public GetGamePredictionValidator()
        {
            RuleFor(x => x.GamePredictionId)
                .GreaterThan(0);
        }
    }

    public sealed class Handler : IRequestHandler<Query, Result>
    {
        private readonly IValidator<Query> _validator;
        private readonly WebAppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public Handler(
            IValidator<Query> validator, 
            WebAppDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _validator = validator;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<Result> Handle(Query query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = _validator.ValidateRequest(query);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            // Should never be calling this command from an unauthenticated context.
            string userId = _httpContextAccessor.HttpContext?.User.Claims.GetUserId()
                ?? throw new InvalidOperationException("Current logged in user could not be found.");

            var dbResult = await _dbContext.GamePredictions
                .AsNoTracking()
                .Where(x => x.GamePredictionId == query.GamePredictionId)
                .Select(x => new Common.Models.GamePrediction
                {
                    GamePredictionId = x.GamePredictionId,
                    SeasonId = x.Game.SeasonWeek.SeasonId,
                    GameId = x.GameId,
                    UserId = x.UserId,
                    IsCurrentUser = x.UserId == userId,
                    FirstName = x.AppUser.FirstName,
                    LastName = x.AppUser.LastName,
                    HomeTeamId = x.Game.HomeTeamId,
                    PredictedHomeTeamScore = x.HomeTeamScore,
                    AwayTeamId = x.Game.AwayTeamId,
                    PredictedAwayTeamScore = x.AwayTeamScore
                })
                .FirstOrDefaultAsync(token);

            if (dbResult is null)
            {
                return new NotFoundProblemDetails($"Game prediction with ID of {query.GamePredictionId} was not found.");
            }

            return new GetGamePredictionResponse
            {
                GamePrediction = dbResult
            };
        }
    }
}
