// TIJDELIJK - verwijderen zodra Be.Vlaanderen.Basisregisters.RoadRegistry.BackOffice.Api opgehoogd is naar de versie
// met het lijst-endpoint van de wegsegmenten (GAWR-7815).
//
// Letterlijke kopie uit road-registry (src/RoadRegistry.BackOffice.Api/V2/RoadSegments/RoadSegmentsController-List.cs),
// met dezelfde namespace, zodat de controller hierboven ongewijzigd blijft als dit bestand verdwijnt. Enige afwijking:
// het voorbeeld bouwt de urls hier zelf op, want de extension methods die road-registry daarvoor gebruikt (PublicApi) zijn
// internal in het pakket.
namespace RoadRegistry.BackOffice.Api.V2.RoadSegments
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using RoadRegistry.BackOffice.Api.Infrastructure;
    using RoadRegistry.BackOffice.Api.Infrastructure.Controllers.Attributes;
    using RoadRegistry.BackOffice.Api.Infrastructure.Options;
    using RoadRegistry.ValueObjects;
    using Swashbuckle.AspNetCore.Filters;

    public class RoadSegmentFilter
    {
        public string? StreetNameId { get; set; }
        public string? Status { get; set; }
    }

    /// <summary>
    ///     Een verzameling van wegsegmenten.
    /// </summary>
    [DataContract(Name = "WegsegmentV2Lijst", Namespace = "")]
    [CustomSwaggerSchemaId("WegsegmentV2Lijst")]
    public class WegsegmentV2Lijst
    {
        /// <summary>
        ///     De verzameling van wegsegmenten.
        /// </summary>
        [DataMember(Name = "Wegsegmenten", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public required List<WegsegmentV2LijstItem> Wegsegmenten { get; set; }

        /// <summary>
        ///     De URL voor het ophalen van de volgende verzameling.
        /// </summary>
        [DataMember(Name = "Volgende", Order = 2, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public Uri? Volgende { get; set; }
    }

    /// <summary>
    ///     Een wegsegment in een verzameling van wegsegmenten.
    /// </summary>
    [DataContract(Name = "WegsegmentV2LijstItem", Namespace = "")]
    [CustomSwaggerSchemaId("WegsegmentV2LijstItem")]
    public class WegsegmentV2LijstItem
    {
        /// <summary>
        ///     De objectidentificator van het wegsegment.
        /// </summary>
        [DataMember(Name = "ObjectId", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ObjectId { get; set; }

        /// <summary>
        ///     Attribuut dat de levensloopfase van een wegsegment aangeeft.
        /// </summary>
        [DataMember(Name = "Wegsegmentstatus", Order = 2)]
        [JsonProperty(Required = Required.DisallowNull)]
        [RoadRegistryEnumDataType(typeof(RoadSegmentStatusV2))]
        public string Wegsegmentstatus { get; set; }

        /// <summary>
        ///     Link naar het detail van het wegsegment.
        /// </summary>
        [DataMember(Name = "Detail", Order = 3)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Detail { get; set; }

        public WegsegmentV2LijstItem(RoadSegmentId roadSegmentId, RoadSegmentStatusV2 status, string detailUrlFormat)
        {
            ObjectId = roadSegmentId.ToString();
            Wegsegmentstatus = status.ToDutchString();
            Detail = string.Format(detailUrlFormat, roadSegmentId);
        }
    }

    public class WegsegmentV2LijstResponseExamples : IExamplesProvider<WegsegmentV2Lijst>
    {
        private readonly ApiOptions _apiOptions;

        public WegsegmentV2LijstResponseExamples(ApiOptions apiOptions)
        {
            _apiOptions = apiOptions;
        }

        public WegsegmentV2Lijst GetExamples()
        {
            var detailUrlFormat = $"{_apiOptions.BaseUrl}/v3/wegsegmenten/{{0}}";
            var volgendeUrlFormat = $"{_apiOptions.BaseUrl}/v3/wegsegmenten?offset={{0}}&limit={{1}}";

            return new WegsegmentV2Lijst
            {
                Wegsegmenten =
                [
                    new WegsegmentV2LijstItem(new RoadSegmentId(123456), RoadSegmentStatusV2.Gerealiseerd, detailUrlFormat),
                    new WegsegmentV2LijstItem(new RoadSegmentId(123457), RoadSegmentStatusV2.Gepland, detailUrlFormat)
                ],
                Volgende = new Uri(string.Format(volgendeUrlFormat, 2, 2))
            };
        }
    }
}
