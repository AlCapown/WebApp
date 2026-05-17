using System;

namespace WebApp.Common.Infrastructure;

public sealed class SimulatedTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _startTime;
    private readonly long _startTimestamp;

    public SimulatedTimeProvider(DateTimeOffset startTime)
    {
        _startTime = startTime;
        _startTimestamp = GetTimestamp();
    }

    public override DateTimeOffset GetUtcNow()
    {
        var elapsed = GetElapsedTime(_startTimestamp);
        return _startTime + elapsed;
    }
}
