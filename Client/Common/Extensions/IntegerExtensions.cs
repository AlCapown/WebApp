namespace WebApp.Client.Common.Extensions;

public static class IntegerExtensions
{
    public static string ToOrdinalNumberString(this int number)
    {
        string num = number.ToString();

        if (number <= 0)        return num;
        if (num.EndsWith("11")) return num + "th";
        if (num.EndsWith("12")) return num + "th";
        if (num.EndsWith("13")) return num + "th";
        if (num.EndsWith('1'))  return num + "st";
        if (num.EndsWith('2'))  return num + "nd";
        if (num.EndsWith('3'))  return num + "rd";

        return num + "th";
    }
}
