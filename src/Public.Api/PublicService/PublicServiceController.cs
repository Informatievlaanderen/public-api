namespace Public.Api.PublicService
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Dienstverleningen")]
    [ApiProduces]
    public partial class PublicServiceController : RegistryApiController<PublicServiceController>
    {
        private const string Registry = "PublicServiceRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaande dienstverlening.";
        protected override string GoneExceptionMessage => "Verwijderde dienstverlening.";

        public PublicServiceController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<PublicServiceController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, context);
    }
}
