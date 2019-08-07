namespace Public.Api.AddressRepresentation
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
    [ApiExplorerSettings(GroupName = "Adresvoorstellingen")]
    [Produces(AcceptTypes.Json, AcceptTypes.JsonLd, AcceptTypes.Xml)]
    public partial class AddressRepresentationController : RegistryApiController<AddressRepresentationController>
    {
        private const string Registry = "AddressRegistry";

        protected override string NotFoundExceptionMessage => "Onbestaand adres.";
        protected override string GoneExceptionMessage => "Adres verwijderd";

        public AddressRepresentationController(
            [KeyFilter(Registry)] IRestClient restClient,
            [KeyFilter(Registry)] IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<AddressRepresentationController> logger)
            : base(restClient, cacheToggle, redis, logger) { }
    }
}
