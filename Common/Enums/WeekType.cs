namespace WebApp.Common.Enums;

public enum WeekType
{
    Preseason,
    RegularSeason,
    PostSeason,
    OffSeason
}

public static class WeekTypeExtensions
{
    public static string ToString(this WeekType w) =>
        w switch
        {
            WeekType.Preseason => "Preseason",
            WeekType.RegularSeason => "RegularSeason",
            WeekType.PostSeason => "PostSeason",
            WeekType.OffSeason => "OffSeason",
            _ => null
        };

    public static string ToSlugString(this WeekType w) =>
        w switch
        {
            WeekType.Preseason => "Pre",
            WeekType.RegularSeason => "Reg",
            WeekType.PostSeason => "Post",
            WeekType.OffSeason => "Off",
            _ => null
        };

    public static string ToFriendlyNameString(this WeekType w) =>
        w switch
        {
            WeekType.Preseason => "Pre Season",
            WeekType.RegularSeason => "Reg Season",
            WeekType.PostSeason => "Post Season",
            WeekType.OffSeason => "Off Season",
            _ => null
        };

    public static WeekType? ToWeekTypeEnum(this string s) =>
        s.ToLower() switch
        {
            "preseason" => WeekType.Preseason,
            "regularseason" => WeekType.RegularSeason,
            "postseason" => WeekType.PostSeason,
            "offseason" => WeekType.OffSeason,
            _ => null
        };
}

