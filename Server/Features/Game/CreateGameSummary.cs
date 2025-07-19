#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using OneOf;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Infrastructure;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Features.Game;

using Result = OneOf<Unit, ValidationProblemDetails, ConflictProblemDetails>;

public static class CreateGameSummary
{
    public sealed record Command : IRequest<Result>
    {
        /// <summary>
        /// The identifier for the game to retrieve AI summary for.
        /// </summary>
        public int GameId { get; init; }

        /// <summary>
        /// Summary of the game, including key details and statistics.
        /// </summary>
        public required string Summary { get; init; }
    }

    public sealed class CreateGameSummaryValidator : AbstractValidator<Command>
    {
        private readonly WebAppDbContext _dbContext;

        public CreateGameSummaryValidator(WebAppDbContext dbContext)
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

            RuleFor(x => x.Summary)
                .NotEmpty();

            RuleFor(x => x.Summary)
                .MaximumLength(int.MaxValue);
        }
    }

    public sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly IValidator<Command> _validator;
        private readonly WebAppDbContext _dbContext;

        public Handler(IValidator<Command> validator, WebAppDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async ValueTask<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = await _validator.ValidateRequestAsync(command, cancellationToken);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            bool conflict = await _dbContext.GameSummaries
                .AsNoTracking()
                .Where(x => x.GameId == command.GameId)
                .FirstOrDefaultAsync(cancellationToken) is not null;

            if (conflict)
            {
                return new ConflictProblemDetails($"Game Summary already exits for game with ID {command.GameId}");
            }

            var gameSummary = new Database.Tables.GameSummary
            {
                GameId = command.GameId,
                Summary = command.Summary
            };

            _dbContext.GameSummaries.Add(gameSummary);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
