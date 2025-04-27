using System;

namespace WebApp.ExternalIntegrations.NFL.Service
{
    public static class Helpers
    {
        public static DateTimeOffset ParseGameStartDateTime(string eid, string time, TimeZoneInfo tz)
        {
            ReadOnlySpan<char> eidAsSpan = eid;
            ReadOnlySpan<char> timeAsSpan = time;

            // The eid acts as the unique identifier but we can extract information about the game date from it
            // by looking at the first 8 digits. Not ideal but it works.
            // Example: 2019101000
            int year = int.Parse(eidAsSpan.Slice(0, 4));
            int month = int.Parse(eidAsSpan.Slice(4, 2));
            int day = int.Parse(eidAsSpan.Slice(6, 2));

            // Time format is always specified as hh:mm
            // Example: 8:20
            int hour = timeAsSpan.Length == 4
                ? int.Parse(timeAsSpan.Slice(0, 1))
                : int.Parse(timeAsSpan.Slice(0, 2));

            int min = timeAsSpan.Length == 4
                ? int.Parse(timeAsSpan.Slice(2, 2))
                : int.Parse(timeAsSpan.Slice(3, 2));

            // Note always assume PM times so we are adding 12 hours since the time string doesn't specify.  
            // TODO: There are games played in London that are acutally played in the AM (Eastern TZ). Reference
            // 2019 Week 6 Buccaneers vs Panthers
            var dt = new DateTime(year, month, day, hour + 12, min, 0, DateTimeKind.Unspecified);

            return new DateTimeOffset(dt, tz.GetUtcOffset(dt));
        }
    }
}
