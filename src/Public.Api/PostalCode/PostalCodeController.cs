namespace Public.Api.PostalCode
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using FeatureToggle;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Postinfo")]
    [ApiOrder(Order = ApiOrder.PostalCode)]
    [Produces(AcceptTypes.Json/*, AcceptTypes.JsonLd, AcceptTypes.Xml*/)]
    public partial class PostalCodeController : RegistryApiController<PostalCodeController>
    {
        private const string Registry = "PostalRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaande postcode.";
        protected override string GoneExceptionMessage => "Verwijderde postcode.";

        public PostalCodeController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<PostalCodeController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
