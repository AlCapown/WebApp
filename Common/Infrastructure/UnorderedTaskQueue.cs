using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Common.Infrastructure;

/// <summary>
/// Simple queue to process tasks one at a time.
/// </summary>
/// <remarks>
/// There is no guarantee that the tasks will be processed in order of being queued. 
/// </remarks>
public sealed class UnorderedTaskQueue : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private bool _disposed;

    public UnorderedTaskQueue()
    {
        _semaphore = new SemaphoreSlim(1);
    }

    public async Task Enqueue(Func<Task> task, CancellationToken token)
    {
        await _semaphore.WaitAsync(token);

        try
        {
            await task();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        if(!_disposed)
        {
            _disposed = true;
            _semaphore.Dispose();
        }
    }
}
