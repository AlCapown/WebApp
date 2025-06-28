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
using WebApp.Common.Enums;
using WebApp.Database;
using WebApp.Server.Infrastructure;

namespace WebApp.Server.Features.Season;

using Result = OneOf<Unit, ValidationProblemDetails>;

public static class UpsertWeekForSeason
{
    public record Command : IRequest<Result>
    {
        public int? SeasonId { get; init; }
        public int? Week { get; init; }
        public WeekType? WeekType { get; init; }
        public DateOnly? WeekStart { get; init; }
        public DateOnly? WeekEnd { get; init; }
        public string? Description { get; init; }
    }

    public sealed class UpsertWeekForSeasonValidator : AbstractValidator<Command>
    {
        private readonly WebAppDbContext _dbContext;

        public UpsertWeekForSeasonValidator(WebAppDbContext dbContext)
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

            RuleFor(x => x.Week)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.WeekType)
                .NotEmpty()
                .IsInEnum();

            RuleFor(x => x.WeekStart)
                .NotEmpty()
                .LessThan(x => x.WeekEnd);

            RuleFor(x => x.WeekEnd)
                .NotEmpty()
                .GreaterThan(x => x.WeekStart);

            RuleFor(x => x.Description)
                .MaximumLength(1000);
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

            var week = await _dbContext.SeasonWeeks.FirstOrDefaultAsync
            (
                x => x.SeasonId == cmd.SeasonId && x.Week == cmd.Week && x.SeasonWeekTypeName == cmd.WeekType, 
                cancellationToken
            );

            if (week is null)
            {
                var seasonWeek = new Database.Tables.SeasonWeek
                {
                    SeasonId = cmd.SeasonId!.Value,
                    Week = cmd.Week!.Value,
                    SeasonWeekTypeName = cmd.WeekType!.Value,
                    WeekStart = cmd.WeekStart!.Value,
                    WeekEnd = cmd.WeekEnd!.Value,
                    Description = cmd.Description,
                };

                _dbContext.SeasonWeeks.Add(seasonWeek);
            }
            else
            {
                week.WeekStart = cmd.WeekStart!.Value;
                week.WeekEnd = cmd.WeekEnd!.Value;
                week.Description = cmd.Description;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
