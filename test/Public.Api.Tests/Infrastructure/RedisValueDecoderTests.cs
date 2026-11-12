namespace Public.Api.Tests.Infrastructure
{
    using System.Text;
    using Common.Infrastructure;
    using FluentAssertions;
    using StackExchange.Redis;
    using ZstdSharp;

    /// <summary>
    /// The redis-populator marks how it stored the value of a record in its compression field. Records written before
    /// that field existed do not have it and are plain text.
    /// </summary>
    public class RedisValueDecoderTests
    {
        private const string Content = "{\"identificator\":{\"id\":\"https://data.vlaanderen.be/id/adres/1\"}}";

        [Fact]
        public void WhenThereIsNoCompressionField_ThenTheValueIsPlainText()
        {
            RedisValueDecoder.TryDecode(Content, RedisValue.Null, out var content).Should().BeTrue();

            content.Should().Be(Content);
        }

        [Fact]
        public void WhenTheCompressionIsNone_ThenTheValueIsPlainText()
        {
            RedisValueDecoder.TryDecode(Content, RedisValueDecoder.CompressionNone, out var content).Should().BeTrue();

            content.Should().Be(Content);
        }

        [Fact]
        public void WhenTheCompressionIsZstd_ThenTheValueIsDecompressed()
        {
            using var compressor = new Compressor();
            var compressed = compressor.Wrap(Encoding.UTF8.GetBytes(Content)).ToArray();

            RedisValueDecoder.TryDecode(compressed, RedisValueDecoder.CompressionZstd, out var content).Should().BeTrue();

            content.Should().Be(Content);
        }

        [Fact]
        public void WhenTheCompressionIsUnknown_ThenTheValueIsNotDecoded()
        {
            RedisValueDecoder.TryDecode(Content, "brotli", out var content).Should().BeFalse();

            content.Should().BeNull();
        }
    }
}
