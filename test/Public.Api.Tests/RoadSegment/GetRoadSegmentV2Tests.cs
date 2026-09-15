namespace Public.Api.Tests.RoadSegment
{
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.OpenApi;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A road segment whose inwinning is complete is no longer served by v2 of the road registry: it answers 404 with a
    /// Link header to the road segment in v3. The public API has to pass that header on, with the problem details any
    /// other not found road segment gets.
    /// </summary>
    [Collection(PublicApiCollection.Name)]
    public class GetRoadSegmentV2Tests
    {
        private const string SuccessorVersionLink = "<https://api.basisregisters.vlaanderen.be/v3/wegsegmenten/51613>; rel=\"successor-version\"";

        private readonly PublicApiTestHost _host;

        public GetRoadSegmentV2Tests(PublicApiTestHost host)
        {
            _host = host;
            _host.RoadRegistry.Reset();
        }

        [Fact]
        public async Task WhenTheBackendAnswersNotFoundWithALink_ThenNotFoundWithThatLink()
        {
            _host.RoadRegistry.RespondWith(() => NotFound(withLink: true));

            using var response = await GetRoadSegment();

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Headers.TryGetValues("Link", out var links).Should().BeTrue();
            links.Should().Equal(SuccessorVersionLink);
            await ShouldBeTheNotFoundProblemDetails(response);
        }

        [Fact]
        public async Task WhenTheBackendAnswersNotFoundWithALink_ThenTheSameProblemDetailsAsWithout()
        {
            _host.RoadRegistry.RespondWith(() => NotFound(withLink: true));
            using var withLink = await GetRoadSegment();

            _host.RoadRegistry.RespondWith(() => NotFound(withLink: false));
            using var withoutLink = await GetRoadSegment();

            var problemWithLink = JObject.Parse(await withLink.Content.ReadAsStringAsync());
            var problemWithoutLink = JObject.Parse(await withoutLink.Content.ReadAsStringAsync());

            problemWithLink.Properties().Select(x => x.Name).Should().BeEquivalentTo(problemWithoutLink.Properties().Select(x => x.Name));
            problemWithLink["type"]!.Value<string>().Should().Be(problemWithoutLink["type"]!.Value<string>());
            problemWithLink["detail"]!.Value<string>().Should().Be(problemWithoutLink["detail"]!.Value<string>());
            withLink.Content.Headers.ContentType!.MediaType.Should().Be(withoutLink.Content.Headers.ContentType!.MediaType);
        }

        [Fact]
        public async Task WhenTheBackendAnswersNotFoundWithoutALink_ThenNotFoundWithoutALink()
        {
            _host.RoadRegistry.RespondWith(() => NotFound(withLink: false));

            using var response = await GetRoadSegment();

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Headers.Contains("Link").Should().BeFalse();
            await ShouldBeTheNotFoundProblemDetails(response);
        }

        [Fact]
        public async Task WhenTheBackendAnswersOk_ThenOkWithItsContent()
        {
            const string roadSegment = "{\"identificator\":{\"objectId\":\"51613\"}}";
            _host.RoadRegistry.RespondWith(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(roadSegment, Encoding.UTF8, "application/json")
            });

            using var response = await GetRoadSegment();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Headers.Contains("Link").Should().BeFalse();
            JToken.DeepEquals(JObject.Parse(await response.Content.ReadAsStringAsync()), JObject.Parse(roadSegment)).Should().BeTrue();
            _host.RoadRegistry.Requests.Should().ContainSingle(request => request.RequestUri!.AbsolutePath == "/v1/wegsegmenten/51613");
        }

        [Fact]
        public void TheLinkOfANotFoundIsDocumented()
        {
            var notFoundResponses = SwaggerDocuments.Names(_host)
                .Select(documentName => SwaggerDocuments.Build(_host, documentName))
                .SelectMany(document => document.Paths.Values)
                .SelectMany(path => path.Operations?.Values ?? Enumerable.Empty<OpenApiOperation>())
                .Where(operation => operation.OperationId == "GetRoadSegmentV2")
                .Select(operation => operation.Responses?["404"])
                .ToList();

            // In every document the operation is part of.
            notFoundResponses.Should().NotBeEmpty()
                .And.OnlyContain(response => response != null && response.Headers != null && response.Headers.ContainsKey("Link"));
        }

        private async Task<HttpResponseMessage> GetRoadSegment()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/v2/wegsegmenten/51613");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _host.CreateClient().SendAsync(request);
        }

        private static async Task ShouldBeTheNotFoundProblemDetails(HttpResponseMessage response)
        {
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

            var problem = JObject.Parse(await response.Content.ReadAsStringAsync());
            problem["type"]!.Value<string>().Should().Be("urn:be.vlaanderen.basisregisters.api:roadsegmentv2:not-found");
            problem["status"]!.Value<int>().Should().Be(404);
            problem["detail"]!.Value<string>().Should().Be("Onbestaand wegsegment.");
        }

        private static HttpResponseMessage NotFound(bool withLink)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(
                    "{\"type\":\"urn:be.vlaanderen.basisregisters.api:roadsegment:not-found\",\"title\":\"Er heeft zich een fout voorgedaan!\",\"detail\":\"Onbestaand wegsegment.\",\"status\":404}",
                    Encoding.UTF8,
                    "application/problem+json")
            };

            if (withLink)
            {
                response.Headers.TryAddWithoutValidation("Link", SuccessorVersionLink);
            }

            return response;
        }
    }
}
