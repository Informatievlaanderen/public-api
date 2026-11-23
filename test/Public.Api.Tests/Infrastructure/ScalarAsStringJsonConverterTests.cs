namespace Public.Api.Tests.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private const string ChangeGeometryDrawMethodRoute = "/v3/wegsegmenten/acties/wijzigen/geometriemethode";

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
        /// The converter is in front of every string the public API binds, so what it must not do is disturb a request
        /// that is right: a whole body has to arrive at the registry as it was sent. Through wijzig attribuutwaarden,
        /// the richest body v3 takes - several groups, nested arrays, positions along the segment, sides, identifiers
        /// and seven kinds of enumeration name, with the optional parts left out of one group and filled in on the
        /// other.
        /// </summary>
        [Fact]
        public async Task WhenAComplexRequestIsValid_ThenTheWholeBodyIsPassedOnUnchanged()
        {
            const string body = """
                                [
                                  {
                                    "wegsegmenten": [481110, 481111],
                                    "morfologie": [{ "morfologie": "weg bestaande uit 1 rijbaan" }],
                                    "wegcategorie": [{ "wegcategorie": "lokale ontsluitingsweg" }]
                                  },
                                  {
                                    "wegsegmenten": [481112],
                                    "morfologie": [
                                      { "vanPositie": 0.0, "totPositie": 12.5, "morfologie": "weg bestaande uit 1 rijbaan" },
                                      { "vanPositie": 12.5, "totPositie": 30.0, "morfologie": "weg bestaande uit 2 rijbanen" }
                                    ],
                                    "wegverharding": [{ "vanPositie": 0.0, "totPositie": 30.0, "wegverharding": "halfverhard" }],
                                    "toegang": [{ "vanPositie": 0.0, "totPositie": 30.0, "toegang": "openbare weg" }],
                                    "straatnaam": [{ "kant": "links", "identificator": "https://data.vlaanderen.be/id/straatnaam/14291" }],
                                    "wegbeheerder": [{ "kant": "rechts", "wegbeheerder": "AWV114" }],
                                    "verkeerstypeAuto": [{ "vanPositie": 0.0, "totPositie": 30.0, "richting": "heen" }],
                                    "verkeerstypeFiets": [{ "vanPositie": 0.0, "totPositie": 30.0, "richting": "beide" }],
                                    "verkeerstypeVoetganger": [{ "vanPositie": 0.0, "totPositie": 30.0, "richting": "niet gekend" }]
                                  }
                                ]
                                """;

            var forwarded = AcceptAndCaptureTheForwardedBody();

            using var response = await ChangeAttributes(body);

            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
            response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);

            // The parameters that were left out are serialized as null on the way to the registry, which says the same
            // thing as leaving them out. Everything else has to be there, value for value.
            var sent = JArray.Parse(body);
            var arrived = WithoutNulls(JArray.Parse(forwarded.Should().ContainSingle().Subject));

            JToken.DeepEquals(arrived, sent).Should().BeTrue($"what arrived at the registry was{Environment.NewLine}{arrived}");
        }

        private static JToken WithoutNulls(JToken token)
        {
            switch (token)
            {
                case JArray array:
                    return new JArray(array.Select(WithoutNulls));

                case JObject o:
                    return new JObject(o.Properties()
                        .Where(property => property.Value.Type != JTokenType.Null)
                        .Select(property => new JProperty(property.Name, WithoutNulls(property.Value))));

                default:
                    return token;
            }
        }

        private async Task<HttpResponseMessage> ChangeAttributes(string body)
        {
            return await Post("/v3/wegsegmenten/acties/wijzigen/attributen", body);
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
            return await Post(ChangeGeometryDrawMethodRoute, body);
        }

        private async Task<HttpResponseMessage> Post(string route, string body)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, route)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _host.CreateClient().SendAsync(request);
        }
    }
}
