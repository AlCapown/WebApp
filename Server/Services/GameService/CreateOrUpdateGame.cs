#nullable enable

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Services.GameService;

using Result = OneOf<Unit, ValidationProblemDetails>;

public sealed class CreateOrUpdateGame
{
    public record Command : IRequest<Result>
    {
        public int SeasonWeekId { get; init; }
        public DateTimeOffset StartsOn { get; init; }
        public int HomeTeamId { get; init; }
        public int HomeTeamScore { get; init; }
        public int AwayTeamId { get; init; }
        public int AwayTeamScore { get; init; }
        public int Quarter { get; init; }
        public string? ClockTime { get; init; }
        public bool IsComplete { get; init; }
    }

    public sealed class CreateOrUpdateGameValidator : AbstractValidator<Command>
    {
        private readonly WebAppDbContext _dbContext;

        public CreateOrUpdateGameValidator(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.SeasonWeekId)
                .NotEmpty()
                .MustAsync(async (seasonWeekId, cancellation) =>
                {
                    return await _dbContext.SeasonWeeks
                        .AsNoTracking()
                        .AnyAsync(x => x.SeasonWeekId == seasonWeekId, cancellation);
                }).WithMessage("Season week not found.");

            RuleFor(x => x.StartsOn)
                .NotEmpty();

            RuleFor(x => x.HomeTeamId)
                .NotEmpty()
                .MustAsync(async (homeTeamId, cancellation) =>
                {
                    return await _dbContext.Teams
                        .AsNoTracking()
                        .AnyAsync(x => x.TeamId == homeTeamId, cancellation);
                }).WithMessage("Home team not found.");

            RuleFor(x => x.HomeTeamScore)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.AwayTeamId)
                .NotEmpty()
                .MustAsync(async (awayTeamId, cancellation) =>
                {
                    return await _dbContext.Teams
                        .AsNoTracking()
                        .AnyAsync(x => x.TeamId == awayTeamId, cancellation);
                }).WithMessage("Away team not found.");

            RuleFor(x => x.AwayTeamScore)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.ClockTime)
                .Matches(@"^\d{1,2}:\d{2}$")
                .WithMessage("Clock time must be in the format MM:SS.");
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

        public async Task<Result> Handle(Command cmd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = await _validator.ValidateRequestAsync(cmd, cancellationToken);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            var game = await _dbContext.Games
                .Where(x => x.SeasonWeekId == cmd.SeasonWeekId)
                .Where(x => x.HomeTeamId == cmd.HomeTeamId)
                .Where(x => x.AwayTeamId == cmd.AwayTeamId)
                .FirstOrDefaultAsync(cancellationToken);

            if (game is null)
            {
                var gameDb = new Database.Tables.Game
                {
                    SeasonWeekId = cmd.SeasonWeekId,
                    StartsOn = cmd.StartsOn,
                    HomeTeamId = cmd.HomeTeamId,
                    HomeTeamScore = cmd.HomeTeamScore,
                    AwayTeamId = cmd.AwayTeamId,
                    AwayTeamScore = cmd.AwayTeamScore,
                    Quarter = cmd.Quarter,
                    ClockTime = cmd.IsComplete ? "0:00" : cmd.ClockTime,
                    IsComplete = cmd.IsComplete,
                };

                _dbContext.Games.Add(gameDb);
            }
            else
            {
                game.StartsOn = cmd.StartsOn;
                game.HomeTeamScore = cmd.HomeTeamScore;
                game.AwayTeamScore = cmd.AwayTeamScore;
                game.Quarter = cmd.Quarter;
                game.ClockTime = cmd.IsComplete ? "0:00" : cmd.ClockTime;
                game.IsComplete = cmd.IsComplete;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
