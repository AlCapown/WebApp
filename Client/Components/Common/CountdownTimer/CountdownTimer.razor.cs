using Microsoft.AspNetCore.Components;
using System;
using System.Timers;

namespace WebApp.Client.Components.Common.CountdownTimer;

public partial class CountdownTimer : ComponentBase, IDisposable
{
    public TimeSpan RemainingTime { get; private set; }
    private DateTime EndTime { get; set; }
    private Timer Timer { get; set; }
    private Action OnTick { get; set; }

    public CountdownTimer()
    {
        RemainingTime = TimeSpan.Zero;

        Timer = new Timer(1000)
        {
            AutoReset = true,
        };

        Timer.Elapsed += OnTimedEvent;
    }

    public void Start(DateTime endTime, Action onTick)
    {
        EndTime = endTime;
        RemainingTime = endTime.Subtract(DateTime.Now);
        OnTick = onTick;
        Timer.Start();
    }

    public bool Started() => Timer.Enabled;

    public void Stop()
    {
        Timer.Stop();
    }

    public bool Stopped() => !Timer.Enabled;

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        RemainingTime = EndTime.Subtract(e.SignalTime);

        if (RemainingTime.TotalSeconds < 1)
        {
            Stop();
            RemainingTime = TimeSpan.Zero;
        }

        OnTick?.Invoke();
        StateHasChanged();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Timer.Elapsed -= OnTimedEvent;
            Timer.Dispose();
        }
    }
}
