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
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Percelen")]
    [ApiProduces]
    public partial class ParcelController : RegistryApiController<ParcelController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaand perceel.";
        protected override string GoneExceptionMessage => "Verwijderd perceel.";

        public ParcelController(
            IHttpContextAccessor httpContextAccessor,
            [KeyFilter(RegistryKeys.Parcel)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Parcel)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<ParcelController> logger)
            : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, context);
    }
}
