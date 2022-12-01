namespace Public.Api.Road.RoadSegments;

using System.Net.Http;
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

[ApiVersion(Version.Current)]
[AdvertiseApiVersions(Version.CurrentAdvertised)]
[ApiRoute("")]
[ApiExplorerSettings(GroupName = "Wegsegmenten")]
[ApiOrder(ApiOrder.Road.RoadUpload)]
[ApiKeyAuth("Road")]
public partial class RoadSegmentsController : RegistryApiController<RoadSegmentsController>
{
    protected const string EndPointRoot = "wegen";

    public RoadSegmentsController(
        IHttpContextAccessor httpContextAccessor,
        [KeyFilter(RegistryKeys.Road)] IRestClient restClient,
        [KeyFilter(RegistryKeys.Road)] IFeatureToggle cacheToggle,
        ConnectionMultiplexerProvider redis,
        ILogger<RoadSegmentsController> logger)
        : base(httpContextAccessor, redis, logger, restClient, cacheToggle)
    {
    }

    protected override string NotFoundExceptionMessage => "Onbestaande wegsegment.";
    protected override string GoneExceptionMessage => "Verwijderde wegsegment.";

    private static ContentFormat DetermineFormat(ActionContext? context)
        => ContentFormat.For(EndpointType.BackOffice, context);
}
