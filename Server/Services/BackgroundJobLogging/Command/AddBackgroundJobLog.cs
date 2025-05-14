using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;

namespace WebApp.Server.Services.BackgroundJobLogging.Command;

public class AddBackgroundJobLog
{
    public class Command : IRequest<Unit>
    {
        public string BackgroundJobName { get; set; }
        public DateTimeOffset? Started { get; set; }
        public DateTimeOffset? Ended { get; set; }
        public List<Error> Errors { get; set; } = [];

        public class Error
        {
            public string Message { get; set; }
            public IDictionary<string, string[]> ValidationErrors { get; set; }
            public string StackTrace { get; set; }
        }
    }

    public class Handler : IRequestHandler<Command, Unit>
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

        public async Task<Unit> Handle (Command command, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            string errorsJson = null;
            bool isSuccess = true;
            
            if (command.Errors is not null && command.Errors.Count != 0)
            {
                errorsJson = JsonSerializer.Serialize(command.Errors, _jsonSerializerOptions);
                isSuccess = false;
            }

            var log = new Database.Tables.BackgroundJobLog
            {
                BackgroundJobName = command.BackgroundJobName,
                IsSuccess = isSuccess,
                Started = command.Started,
                Ended = command.Ended,
                ErrorsJson = errorsJson
            };

            _dbContext.Add(log);

            await _dbContext.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
