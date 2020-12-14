namespace Public.Api.Parcel
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
    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Percelen")]
    [ApiOrder(Order = ApiOrder.Parcel)]
    [ApiProduces]
    public partial class ParcelController : RegistryApiController<ParcelController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaand perceel.";
        protected override string GoneExceptionMessage => "Verwijderd perceel.";

        public ParcelController(
            [KeyFilter(RegistryKeys.Parcel)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Parcel)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<ParcelController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(string urlFormat, ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, urlFormat, context);
    }
}
