// namespace Public.Api.CrabBuilding
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
//     using RestSharp;
//
//     [ApiVisible]
//     [ApiVersion(Version.Current)]
//     [AdvertiseApiVersions(Version.CurrentAdvertised)]
//     [ApiRoute("")]
//     [ApiExplorerSettings(GroupName = "CRAB Gebouwen")]
//     [ApiOrder(ApiOrder.CrabBuildings)]
//     [ApiProduces]
//     public partial class CrabBuildingController : RegistryApiController<CrabBuildingController>
//     {
//         protected override string NotFoundExceptionMessage => "Onbestaand gebouw.";
//         protected override string GoneExceptionMessage => "Verwijderd gebouw.";
//
//         public CrabBuildingController(
//             IHttpContextAccessor httpContextAccessor,
//             [KeyFilter(RegistryKeys.Building)] IRestClient restClient,
//             [KeyFilter(RegistryKeys.Building)] IFeatureToggle cacheToggle,
//             ConnectionMultiplexerProvider redis,
//             ILogger<CrabBuildingController> logger)
//             : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }
//
//         private static ContentFormat DetermineFormat(ActionContext context)
//             => ContentFormat.For(EndpointType.Legacy, context);
//     }
// }
