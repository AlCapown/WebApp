using System;

namespace WebApp.Common.Infrastructure;

public sealed class FrozenTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _frozenTime;

    public FrozenTimeProvider(DateTimeOffset frozenTime)
    {
        _frozenTime = frozenTime;
    }

    public override DateTimeOffset GetUtcNow() => _frozenTime;
}
