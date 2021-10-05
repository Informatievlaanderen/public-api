namespace Public.Api.Road.Information
{
    using System.Net.Http;
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion(Version.Current)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Informatie")]
    [ApiOrder(Order = ApiOrder.RoadInformation)]
    public partial class InformationController : RegistryApiController<InformationController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande informatie.";
        protected override string GoneExceptionMessage => "Verwijderde informatie.";

        public InformationController(
            [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
            [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<InformationController> logger)
            : base(restClient, cacheToggle, redis, logger)
        {
        }
    }
}
