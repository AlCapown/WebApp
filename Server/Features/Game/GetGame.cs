#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Infrastructure;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Features.Game;

using Result = OneOf<Common.Models.Game, ValidationProblemDetails, NotFoundProblemDetails>;

public static class GetGame
{
    public sealed record Query : IRequest<Result>
    {
        /// <summary>
        /// The identifier for the game to retrieve AI summary for.
        /// </summary>
        public int GameId { get; init; }
    }

    public sealed class GetGameValidator : AbstractValidator<Query>
    {
        public GetGameValidator()
        {
            RuleFor(x => x.GameId)
                .GreaterThan(0)
                .WithMessage("GameId must be greater than 0.");
        }
    }

    public sealed class Handler : IRequestHandler<Query, Result>
    {
        private readonly IValidator<Query> _validator;
        private readonly WebAppDbContext _dbContext;

        public Handler(IValidator<Query> validator, WebAppDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async ValueTask<Result> Handle(Query query, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = _validator.ValidateRequest(query);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            var game = await _dbContext.Games
                .AsNoTracking()
                .Where(x => x.GameId == query.GameId)
                .Select(g => new Common.Models.Game
                {
                    GameId = g.GameId,
                    SeasonWeekId = g.SeasonWeekId,
                    SeasonId = g.SeasonWeek.SeasonId,
                    Week = g.SeasonWeek.Week,
                    SeasonWeekTypeName = g.SeasonWeek.SeasonWeekTypeName,
                    GameStartsOn = g.StartsOn,
                    HomeTeamId = g.HomeTeamId,
                    HomeTeamName = g.HomeTeam.TeamName,
                    HomeTeamScore = g.HomeTeamScore,
                    AwayTeamId = g.AwayTeamId,
                    AwayTeamName = g.AwayTeam.TeamName,
                    AwayTeamScore = g.AwayTeamScore,
                    ClockTime = g.ClockTime,
                    Quarter = g.Quarter,
                    IsComplete = g.IsComplete
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (game is null)
            {
                return new NotFoundProblemDetails($"Game with ID {query.GameId} not found.");
            }

            return game;
        }
    }
}
