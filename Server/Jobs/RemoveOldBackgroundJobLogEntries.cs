using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Database;

namespace WebApp.Server.Jobs;

public sealed class RemoveOldBackgroundJobLogEntries
{
    private readonly WebAppDbContext _dbContext;

    public RemoveOldBackgroundJobLogEntries(WebAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Process(CancellationToken token)
    {
        await _dbContext.BackgroundJobLogs
            .Where(x => x.Started < DateTimeOffset.Now.AddDays(-30))
            .ExecuteDeleteAsync(token);
    }
}
