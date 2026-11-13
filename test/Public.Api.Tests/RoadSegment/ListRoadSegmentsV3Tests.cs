namespace Public.Api.Tests.RoadSegment
{
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;
    using FluentAssertions;
    using Infrastructure;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The list of road segments passes its filters and paging on to the road registry in the X-Filtering and X-Pagination
    /// headers, and adds the filters to the link to the next page the road registry answers with.
    /// </summary>
    [Collection(PublicApiCollection.Name)]
    public class ListRoadSegmentsV3Tests
    {
        private const string NextPage = "https://api.staging-basisregisters.vlaanderen/v3/wegsegmenten?offset=2&limit=2";

        private readonly PublicApiTestHost _host;

        public ListRoadSegmentsV3Tests(PublicApiTestHost host)
        {
            _host = host;
            _host.RoadRegistry.Reset();
        }

        [Fact]
        public async Task TheFiltersAndThePagingArePassedOnToTheRoadRegistry()
        {
            _host.RoadRegistry.RespondWith(() => Ok(volgende: null));

            using var response = await ListRoadSegments("?straatnaamObjectId=14291&wegsegmentstatus=buiten%20gebruik&offset=4&limit=2");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var request = _host.RoadRegistry.Requests.Should().ContainSingle().Subject;
            request.RequestUri!.AbsolutePath.Should().Be("/v2/wegsegmenten");

            var filter = JObject.Parse(Uri.UnescapeDataString(request.Headers.GetValues("X-Filtering").Single()));
            filter["StreetNameId"]!.Value<string>().Should().Be("14291");
            filter["Status"]!.Value<string>().Should().Be("buiten gebruik");
            request.Headers.GetValues("X-Pagination").Single().Should().Be("4,2");
        }

        [Fact]
        public async Task TheLinkToTheNextPageKeepsTheFilters()
        {
            _host.RoadRegistry.RespondWith(() => Ok(volgende: NextPage));

            using var response = await ListRoadSegments("?straatnaamObjectId=14291&limit=2");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = JObject.Parse(await response.Content.ReadAsStringAsync());
            list["wegsegmenten"]!.Should().HaveCount(2);
            list["volgende"]!.Value<string>().Should().Be($"{NextPage}&straatnaamObjectId=14291");
        }

        private async Task<HttpResponseMessage> ListRoadSegments(string query)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/v3/wegsegmenten{query}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _host.CreateClient().SendAsync(request);
        }

        private static HttpResponseMessage Ok(string? volgende)
        {
            var list = new JObject
            {
                ["wegsegmenten"] = new JArray(
                    new JObject { ["objectId"] = "1", ["wegsegmentstatus"] = "gerealiseerd", ["detail"] = "https://api.staging-basisregisters.vlaanderen/v3/wegsegmenten/1" },
                    new JObject { ["objectId"] = "2", ["wegsegmentstatus"] = "gerealiseerd", ["detail"] = "https://api.staging-basisregisters.vlaanderen/v3/wegsegmenten/2" })
            };
            if (volgende is not null)
            {
                list["volgende"] = volgende;
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(list.ToString(), Encoding.UTF8, "application/json")
            };
        }
    }
}
