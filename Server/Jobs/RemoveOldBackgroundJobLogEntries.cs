using Mediator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;
using WebApp.Server.Features.BackgroundJobLogging;

namespace WebApp.Server.Jobs;

public sealed class RemoveOldBackgroundJobLogEntries
{
    private readonly WebAppDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly CreateBackgroundJobLog.Command _logCommand;

    public RemoveOldBackgroundJobLogEntries(WebAppDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _logCommand = new CreateBackgroundJobLog.Command
        {
            BackgroundJobName = nameof(RemoveOldBackgroundJobLogEntries),
            Started = DateTimeOffset.Now
        };
    }

    public async Task Process(CancellationToken token)
    {
        try
        {
            await _dbContext.BackgroundJobLogs
                .Where(x => x.Started < DateTimeOffset.Now.AddDays(-30))
                .ExecuteDeleteAsync(token);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logCommand.Errors.Add(new CreateBackgroundJobLog.Command.Error
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace
            });
        }

        await _mediator.Send(_logCommand, CancellationToken.None);
    }
}
