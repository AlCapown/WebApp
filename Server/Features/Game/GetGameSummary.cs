#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Database;
using WebApp.Server.Infrastructure;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Features.Game;

using Result = OneOf<GameSummary, ValidationProblemDetails, NotFoundProblemDetails>;

public static class GetGameSummary
{
    public sealed record Query : IRequest<Result>
    {
        /// <summary>
        /// The identifier for the game to retrieve summary for.
        /// </summary>
        public int GameId { get; init; }
    }

    public sealed class GetGameSummaryValidator : AbstractValidator<Query>
    {
        private readonly WebAppDbContext _dbContext;

        public GetGameSummaryValidator(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.GameId)
                .GreaterThan(0)
                .WithMessage("GameId must be greater than 0.");

            RuleFor(x => x.GameId)
                .MustAsync(async (gameId, cancellationToken) =>
                {
                    return await _dbContext.Games.FindAsync([gameId], cancellationToken) is not null;
                })
                .WithMessage("Game with the specified GameId does not exist.");
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

            ValidationProblemDetails? problemDetails = await _validator.ValidateRequestAsync(query, cancellationToken);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            var gameSummary = await _dbContext.GameSummaries
                .AsNoTracking()
                .Where(x => x.GameId == query.GameId)
                .Select(x => new GameSummary
                {
                    GameSummaryId = x.GameSummaryId,
                    GameId = x.GameId,
                    Summary = x.Summary
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (gameSummary is null)
            {
                return new NotFoundProblemDetails($"No summary found for game with ID {query.GameId}.");
            }

            return gameSummary;
        }
    }
}
