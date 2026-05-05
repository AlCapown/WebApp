using System;

namespace WebApp.Common.Infrastructure;

public sealed class CountdownTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _startTime;
    private readonly long _startTimestamp;

    public CountdownTimeProvider(DateTimeOffset startTime)
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
