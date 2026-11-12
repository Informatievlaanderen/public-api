namespace Common.Infrastructure
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using StackExchange.Redis;
    using ZstdSharp;

    /// <summary>
    /// Decodes the value of a record cached by the redis-populator, according to its compression field.
    /// </summary>
    /// <remarks>
    /// Records written before the compression field existed do not have it, they are plain text.
    /// </remarks>
    public static class RedisValueDecoder
    {
        public const string CompressionKey = "compression";
        public const string CompressionNone = "none";
        public const string CompressionZstd = "zstd";

        /// <returns>False when the compression is unknown, so the caller can treat the record as a cache miss.</returns>
        public static bool TryDecode(RedisValue value, RedisValue compression, [NotNullWhen(true)] out string? content)
        {
            if (compression.IsNullOrEmpty || compression == CompressionNone)
            {
                content = value.ToString();
                return true;
            }

            if (compression == CompressionZstd)
            {
                using var decompressor = new Decompressor();
                content = Encoding.UTF8.GetString(decompressor.Unwrap((byte[])value!));
                return true;
            }

            content = null;
            return false;
        }
    }
}
