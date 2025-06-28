#nullable enable

using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Infrastructure;
using WebApp.Server.Infrastructure.ProblemDetailsModels;

namespace WebApp.Server.Features.Season;

using Result = OneOf<Unit, ValidationProblemDetails, NotFoundProblemDetails>;

public static class UpdateWeekForSeason
{
    public sealed record Command : IRequest<Result>
    {
        public int? SeasonId { get; init; }
        public int? SeasonWeekId { get; init; }
        public DateOnly? WeekStart { get; init; }
        public DateOnly? WeekEnd { get; init; }
    }

    public sealed class UpdateWeekForSeasonValidator : AbstractValidator<Command>
    {
        private readonly WebAppDbContext _dbContext;

        public UpdateWeekForSeasonValidator(WebAppDbContext dbContext) 
        {
            _dbContext = dbContext;

            RuleFor(x => x.SeasonId)
                .NotEmpty()
                .GreaterThan(0)
                .MustAsync(async (seasonId, cancellationToken) =>
                {
                    return await _dbContext.Seasons.FindAsync([seasonId], cancellationToken) is not null;
                })
                .WithMessage(x => $"The season with the id of {x.SeasonId} does not exist");

            RuleFor(x => x.SeasonWeekId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.WeekStart)
                .NotEmpty()
                .LessThan(x => x.WeekEnd);

            RuleFor(x => x.WeekEnd)
                .NotEmpty()
                .GreaterThan(x => x.WeekStart);
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

            Debug.Assert(cmd.WeekStart is not null);
            Debug.Assert(cmd.WeekEnd is not null);

            var week = await _dbContext.SeasonWeeks.FirstOrDefaultAsync
            (
                x => x.SeasonId == cmd.SeasonId && x.SeasonWeekId == cmd.SeasonWeekId, 
                cancellationToken
            );

            if (week is null)
            {
                return new NotFoundProblemDetails($"Season week with SeasonId {cmd.SeasonId} and SeasonWeekId {cmd.SeasonWeekId} was not found.");
            }

            week.WeekStart = cmd.WeekStart.Value;
            week.WeekEnd = cmd.WeekEnd.Value;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
