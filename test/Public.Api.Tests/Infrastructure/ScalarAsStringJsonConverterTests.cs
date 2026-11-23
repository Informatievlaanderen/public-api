namespace Public.Api.Tests.Infrastructure
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;
    using FluentAssertions;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A value of the wrong type is a value, and it is the registry behind the public API that answers for it. It used
    /// to bind as null instead, because of the string converter registered for every string this api binds, and a
    /// required parameter holding null blew up when the body was serialized for that registry: a 500 with an empty
    /// detail, with the request never leaving the public API (GAWR-7870).
    /// </summary>
    /// <remarks>
    /// Through wijzig geometriemethode, which is where this was reported, but the converter is registered for every
    /// string the public API binds and nothing in these tests is particular to road segments.
    /// </remarks>
    [Collection(PublicApiCollection.Name)]
    public class ScalarAsStringJsonConverterTests
    {
        private const string Route = "/v3/wegsegmenten/acties/wijzigen/geometriemethode";

        private readonly PublicApiTestHost _host;

        public ScalarAsStringJsonConverterTests(PublicApiTestHost host)
        {
            _host = host;
            _host.RoadRegistry.Reset();
        }

        [Theory]
        [InlineData("123", "123")]
        [InlineData("1.5", "1.5")]
        [InlineData("true", "true")]
        public async Task WhenAParameterIsNotAString_ThenItIsPassedOnAsTheTextItWasSentAs(string jsonValue, string expected)
        {
            var forwarded = AcceptAndCaptureTheForwardedBody();

            using var response = await ChangeGeometryDrawMethod($"[{{\"wegsegmenten\":[1],\"geometriemethode\":{jsonValue}}}]");

            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
            GeometryDrawMethodIn(forwarded).Should().Be(expected);
        }

        [Fact]
        public async Task WhenAParameterIsAString_ThenTheConverterItWasPutInFrontOfStillTrimsIt()
        {
            var forwarded = AcceptAndCaptureTheForwardedBody();

            using var response = await ChangeGeometryDrawMethod("[{\"wegsegmenten\":[1],\"geometriemethode\":\"  ingemeten  \"}]");

            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
            GeometryDrawMethodIn(forwarded).Should().Be("ingemeten");
        }

        [Theory]
        [InlineData("{\"naam\":\"ingemeten\"}")]
        [InlineData("[\"ingemeten\"]")]
        public async Task WhenAParameterIsNoValueAtAll_ThenBadRequest(string jsonValue)
        {
            _host.RoadRegistry.RespondWith(() => new HttpResponseMessage(HttpStatusCode.Accepted));

            using var response = await ChangeGeometryDrawMethod($"[{{\"wegsegmenten\":[1],\"geometriemethode\":{jsonValue}}}]");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _host.RoadRegistry.Requests.Should().BeEmpty("a body that cannot be read is never forwarded");
        }

        /// <summary>
        /// Answers 202 and keeps the body the public API sent, read while the request is on its way: the content of a
        /// request that has been sent is disposed of by the time the call returns.
        /// </summary>
        private List<string> AcceptAndCaptureTheForwardedBody()
        {
            var bodies = new List<string>();

            _host.RoadRegistry.RespondWith(() =>
            {
                var content = _host.RoadRegistry.Requests[^1].Content!;
                bodies.Add(content.ReadAsStringAsync().GetAwaiter().GetResult());

                return new HttpResponseMessage(HttpStatusCode.Accepted);
            });

            return bodies;
        }

        private static string? GeometryDrawMethodIn(List<string> forwardedBodies)
        {
            var body = JArray.Parse(forwardedBodies.Should().ContainSingle().Subject);

            return body[0]["geometriemethode"]!.Value<string>();
        }

        private async Task<HttpResponseMessage> ChangeGeometryDrawMethod(string body)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, Route)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _host.CreateClient().SendAsync(request);
        }
    }
}
