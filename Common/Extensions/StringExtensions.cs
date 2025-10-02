#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace WebApp.Common.Extensions;

public static class StringExtensions
{
    public static string? ToTitleCase(this string? value)
    {
        return value.IsNullOrWhiteSpace() 
            ? value
            : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
    }

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static DayOfWeek DayAbbreviationToDayOfWeek(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.ToUpper() switch
        {
            "SUN" => DayOfWeek.Sunday,
            "MON" => DayOfWeek.Monday,
            "TUE" => DayOfWeek.Tuesday,
            "WED" => DayOfWeek.Wednesday,
            "THUR" => DayOfWeek.Thursday,
            "FRI" => DayOfWeek.Friday,
            "SAT" => DayOfWeek.Saturday,
            _ => throw new ArgumentException($"The day abbreviation {value} was not recognized."),
        };
    }
}
