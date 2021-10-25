namespace Kruchy.Plugin.Akcje.Utils
{
    public static class StringExtensions
    {
        public static string NullIfEmptyStringElseValue(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            else
                return value;
        }

        public static string EndLinesToN(this string value)
        {
            return value.Replace("\r\n", "\n");
        }
    }
}
