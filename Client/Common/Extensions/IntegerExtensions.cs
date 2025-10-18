#nullable enable

namespace WebApp.Client.Common.Extensions;

public static class IntegerExtensions
{
    public static string ToOrdinalNumberString(this int number)
    {
        string num = number.ToString();

        return number <= 0 ? num : num + GetOrdinalSuffix(num);
        
        static string GetOrdinalSuffix(string numStr) => numStr switch
        {
            var s when s.EndsWith("11") || s.EndsWith("12") || s.EndsWith("13") => "th",
            var s when s.EndsWith('1') => "st",
            var s when s.EndsWith('2') => "nd", 
            var s when s.EndsWith('3') => "rd",
            _ => "th"
        };
    }
}
