#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Features.Season;

using Result = OneOf<Unit, ValidationProblemDetails>;

public static class CreateSeason    
{
    public sealed record Command : IRequest<Result>
    {
        public int? SeasonId { get; init; }
        public string? Description { get; init; }
    }

    public sealed class CreateSeasonValidator : AbstractValidator<Command>
    {
        public readonly WebAppDbContext _dbContext;

        public CreateSeasonValidator(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.SeasonId)
                .NotEmpty()
                .MustAsync(async (seasonId, cancellationToken) =>
                {
                    return await _dbContext.Seasons.FindAsync([seasonId], cancellationToken) is null;
                })
                .WithMessage("Season already exits.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(255);
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

            ValidationProblemDetails? problemDetails = await _validator.ValidateRequestAsync(cmd, cancellationToken);
            if (problemDetails is not null)
            {
                return problemDetails;
            }

            Debug.Assert(cmd.SeasonId is not null);

            var season = new Database.Tables.Season
            {
                SeasonId = cmd.SeasonId.Value,
                Description = cmd.Description
            };

            _dbContext.Seasons.Add(season);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
