#nullable enable

using Mediator;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;

namespace WebApp.Server.Features.BackgroundJobLogging;

public static class CreateBackgroundJobLog
{
    public sealed record Command : IRequest<Unit>
    {
        public required string BackgroundJobName { get; init; }
        public DateTimeOffset? Started { get; init; }
        public List<Error> Errors { get; init; } = [];

        public sealed record Error
        {
            public string? Message { get; init; }
            public IDictionary<string, string[]>? ValidationErrors { get; init; }
            public string? StackTrace { get; init; }
        }
    }

    public sealed class Handler : IRequestHandler<Command, Unit>
    {
        private readonly WebAppDbContext _dbContext;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public Handler(WebAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<Unit> Handle(Command command, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            string? errorsJson = null;
            bool isSuccess = true;

            if (command.Errors is { Count: > 0 })
            {
                errorsJson = JsonSerializer.Serialize(command.Errors, _jsonSerializerOptions);
                isSuccess = false;
            }

            var log = new Database.Tables.BackgroundJobLog
            {
                BackgroundJobName = command.BackgroundJobName,
                IsSuccess = isSuccess,
                Started = command.Started,
                Ended = DateTimeOffset.Now,
                ErrorsJson = errorsJson
            };

            _dbContext.Add(log);

            await _dbContext.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
