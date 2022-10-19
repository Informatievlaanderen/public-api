namespace Public.Api.PostalCode
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
    [ApiExplorerSettings(GroupName = "Postinfo")]
    [ApiProduces]
    public partial class PostalCodeController : RegistryApiController<PostalCodeController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande postcode.";
        protected override string GoneExceptionMessage => "Verwijderde postcode.";

        public PostalCodeController(
            [KeyFilter(RegistryKeys.Postal)] RestClient restClient,
            [KeyFilter(RegistryKeys.Postal)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<PostalCodeController> logger)
            : base(restClient, cacheToggle, redis, logger) { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.Legacy, context);
    }
}
