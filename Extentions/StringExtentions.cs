namespace IgTradingApp.Extentions
{
    public static class StringExtentions
    {
        public static string Truncate(this string source, int length)
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length);
            }
            return source;
        }
    }
}