using Hangfire;
using System.Threading;
using WebApp.Server.Jobs;

namespace WebApp.Server.Infrastructure;

public static class ReoccurringJobsScheduler
{
    public static void Schedule()
    {
        BackgroundJob.Enqueue<UpdateScheduleAndGameScores>(job => job.Process(true, CancellationToken.None));

        //BackgroundJob.Enqueue<SummarizeLastWeeksResults>(job => job.Process(CancellationToken.None));

        RecurringJob.AddOrUpdate<UpdateScheduleAndGameScores>("UpdateGameScores", 
            job => job.Process(false, CancellationToken.None), "*/5 * * * *"); // Every 5 Min

        RecurringJob.AddOrUpdate<RemoveOldBackgroundJobLogEntries>("RemoveOldBackgroundJobLogEntries",
            job => job.Process(CancellationToken.None), "0 1 * * *"); // Daily at 1:00AM
    }
}
