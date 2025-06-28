#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Infrastructure;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Features.Season;

using Result = OneOf<Unit, ValidationProblemDetails, NotFoundProblemDetails>;

public static class UpdateSeason
{
    public sealed record Command : IRequest<Result>
    {
        public int? SeasonId { get; init; }
        public string? Description { get; init; }
        public DateOnly? SeasonStart { get; init; }
        public DateOnly? SeasonEnd { get; init; }
    }

    public sealed class UpdateSeasonValidator : AbstractValidator<Command>
    {
        public UpdateSeasonValidator()
        {

            RuleFor(x => x.SeasonId)
                .NotEmpty();

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.SeasonStart)
                .LessThan(x => x.SeasonEnd)
                .When(x => x.SeasonEnd.HasValue);

            RuleFor(x => x.SeasonEnd)
                .GreaterThan(x => x.SeasonStart)
                .When(x => x.SeasonStart.HasValue);
        }
    }

    public sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly WebAppDbContext _dbContext;
        private readonly IValidator<Command> _validator;

        public Handler(WebAppDbContext dbContext, IValidator<Command> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async ValueTask<Result> Handle(Command cmd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ValidationProblemDetails? problemDetails = _validator.ValidateRequest(cmd);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            var season = await _dbContext.Seasons.FindAsync([cmd.SeasonId], cancellationToken);

            if (season is null)
            {
                return new NotFoundProblemDetails($"The Season with an Id of {cmd.SeasonId} does not exist.");
            }

            season.Description = cmd.Description ?? season.Description;
            season.SeasonStart = cmd.SeasonStart ?? season.SeasonStart;
            season.SeasonEnd = cmd.SeasonEnd ?? season.SeasonEnd;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
