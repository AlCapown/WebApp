using Microsoft.AspNetCore.Components;
using System;
using System.Timers;

namespace WebApp.Client.Components.Common.CountdownTimer;

/// <summary>
/// A component that provides countdown timer functionality.
/// </summary>
public partial class CountdownTimer : ComponentBase, IDisposable
{
    /// <summary>
    /// Gets the remaining time until the countdown ends.
    /// </summary>
    public TimeSpan RemainingTime { get; private set; }

    /// <summary>
    /// Gets or sets the end time for the countdown.
    /// </summary>
    private DateTime EndTime { get; set; }

    /// <summary>
    /// Gets or sets the timer used to trigger countdown ticks.
    /// </summary>
    private Timer Timer { get; set; }

    /// <summary>
    /// Gets or sets the action to invoke on each timer tick.
    /// </summary>
    private Action OnTick { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CountdownTimer"/> class.
    /// </summary>
    public CountdownTimer()
    {
        RemainingTime = TimeSpan.Zero;

        Timer = new Timer(1000)
        {
            AutoReset = true,
        };

        Timer.Elapsed += OnTimedEvent;
    }

    /// <summary>
    /// Starts the countdown timer with the specified end time and tick callback.
    /// </summary>
    /// <param name="endTime">The time at which the countdown should end.</param>
    /// <param name="onTick">The action to invoke on each tick.</param>
    public void Start(DateTime endTime, Action onTick)
    {
        EndTime = endTime;
        RemainingTime = endTime.Subtract(DateTime.Now);
        OnTick = onTick;
        Timer.Start();
    }

    /// <summary>
    /// Stops the countdown timer.
    /// </summary>
    public void Stop()
    {
        Timer.Stop();
    }

    /// <summary>
    /// Gets a value indicating whether the timer is currently running.
    /// </summary>
    public bool Started => Timer.Enabled;

    /// <summary>
    /// Gets a value indicating whether the timer is currently stopped.
    /// </summary>
    public bool Stopped => !Timer.Enabled;

    /// <summary>
    /// Handles the timer's Elapsed event, updates the remaining time, and triggers callbacks.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The elapsed event arguments.</param>
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

    /// <summary>
    /// Releases all resources used by the <see cref="CountdownTimer"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="CountdownTimer"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Timer.Elapsed -= OnTimedEvent;
            Timer.Dispose();
        }
    }
}
