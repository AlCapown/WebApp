using Hangfire;
using System.Threading;
using WebApp.Server.Jobs;

namespace WebApp.Server.Infrastructure;

public static class ReoccurringJobsScheduler
{
    public static void Schedule()
    {
        BackgroundJob.Enqueue<UpdateScheduleAndGameScores>(job => job.Process(true, CancellationToken.None));
        
        RecurringJob.AddOrUpdate<UpdateScheduleAndGameScores>("UpdateGameScores", 
            job => job.Process(false, CancellationToken.None), "*/5 * * * *"); // Every 5 Min

        RecurringJob.AddOrUpdate<RemoveOldBackgroundJobLogEntries>("RemoveOldBackgroundJobLogEntries",
            job => job.Process(CancellationToken.None), "0 0 * * *"); // Daily at 12:00AM
    }
}
