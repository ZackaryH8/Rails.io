using System.Text.RegularExpressions;

namespace RailsIO.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceX(this string text, string regex, string replacement)
        {
            return Regex.Replace(text, regex, replacement);
        }
    }
}
