// namespace Public.Api.StreetName
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
//     [ApiExplorerSettings(GroupName = "Straatnamen")]
//     [ApiProduces]
//     public partial class StreetNameController : RegistryApiController<StreetNameController>
//     {
//         protected override string NotFoundExceptionMessage => "Onbestaande straatnaam.";
//         protected override string GoneExceptionMessage => "Verwijderde straatnaam.";
//
//         public StreetNameController(
//             IHttpContextAccessor httpContextAccessor,
//             [KeyFilter(RegistryKeys.StreetName)] IRestClient restClient,
//             [KeyFilter(RegistryKeys.StreetName)] IFeatureToggle cacheToggle,
//             ConnectionMultiplexerProvider redis,
//             ILogger<StreetNameController> logger)
//             : base(httpContextAccessor, redis, logger, restClient, cacheToggle) { }
//
//         private static ContentFormat DetermineFormat(ActionContext context)
//             => ContentFormat.For(EndpointType.Legacy, context);
//     }
// }
