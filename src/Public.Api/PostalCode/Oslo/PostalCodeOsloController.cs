namespace Public.Api.PostalCode.Oslo
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.V2)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Postinfo")]
    [ApiOrder(Order = ApiOrder.PostalCode)]
    [ApiProduces(EndpointType.Oslo)]
    public partial class PostalCodeOsloController : RegistryApiController<PostalCodeOsloController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande postcode.";
        protected override string GoneExceptionMessage => "Verwijderde postcode.";

        public PostalCodeOsloController(
            [KeyFilter(RegistryKeys.PostalV2)] IRestClient restClient,
            [KeyFilter(RegistryKeys.PostalV2)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<PostalCodeOsloController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Oslo, context);
    }
}
