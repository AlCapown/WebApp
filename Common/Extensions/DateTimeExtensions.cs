using System;

namespace WebApp.Common.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Returns the nearest day of the week based on the date.
    /// </summary>
    /// <remarks>
    /// If the date already lands of the day of the week, then the date is returned. 
    /// </remarks>
    /// <param name="date"></param>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static DateTime NearestDayOfWeek(this DateTime date, DayOfWeek dayOfWeek)
    {
        int daysUntil = ((int)dayOfWeek - (int)date.DayOfWeek + 7) % 7;
        return date.AddDays(daysUntil);
    }
}
