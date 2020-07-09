namespace Common.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string Format(this string template, params object[] values)
            => string.Format(template, values);
    }
}
