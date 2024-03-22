// namespace Public.Api.BuildingUnit
// {
//     using Autofac.Features.AttributeFilters;
//     using Be.Vlaanderen.Basisregisters.Api;
//     using Common.Infrastructure;
//     using Common.Infrastructure.Controllers;
//     using Common.Infrastructure.Controllers.Attributes;
//     using FeatureToggle;
//     using Infrastructure.Configuration;
//     using Infrastructure.Swagger;
//     using Infrastructure.Version;
//     using Microsoft.AspNetCore.Http;
//     using Microsoft.AspNetCore.Mvc;
//     using Microsoft.Extensions.Logging;
//
//     [ApiVisible]
//     [ApiVersion(Version.Current)]
//     [AdvertiseApiVersions(Version.CurrentAdvertised)]
//     [ApiRoute("")]
//     [ApiExplorerSettings(GroupName = "Gebouweenheden")]
//     [ApiProduces]
//     public partial class BuildingUnitController : RegistryApiController<BuildingUnitController>
//     {
//         protected override string NotFoundExceptionMessage => "Onbestaande gebouweenheid.";
//         protected override string GoneExceptionMessage => "Verwijderde gebouweenheid.";
//
//         public BuildingUnitController(
//             IHttpContextAccessor httpContextAccessor,
//             [KeyFilter(RegistryKeys.Building)] IRestClient restClient,
//             [KeyFilter(RegistryKeys.Building)] IFeatureToggle cacheToggle,
//             ConnectionMultiplexerProvider redis,
//             ILogger<BuildingUnitController> logger)
//             : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }
//
//         private static ContentFormat DetermineFormat(ActionContext context)
//             => ContentFormat.For(EndpointType.Legacy, context);
//     }
// }
