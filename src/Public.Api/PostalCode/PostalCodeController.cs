namespace Public.Api.PostalCode
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using FeatureToggle;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Post Informatie")]
    [Produces(AcceptTypes.Json, AcceptTypes.JsonLd, AcceptTypes.Xml)]
    public partial class PostalCodeController : RegistryApiController<PostalCodeController>
    {
        private const string Registry = "PostalRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaande postcode.";
        protected override string GoneExceptionMessage => "Postcode verwijderd";

        public PostalCodeController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<PostalCodeController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
