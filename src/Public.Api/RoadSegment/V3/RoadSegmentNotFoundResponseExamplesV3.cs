namespace Public.Api.RoadSegment.V3;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using RoadRegistry.BackOffice.Api.RoadSegments.V1;

public class RoadSegmentNotFoundResponseExamplesV3 : RoadSegmentNotFoundResponseExamples
{
    public RoadSegmentNotFoundResponseExamplesV3(
        IHttpContextAccessor httpContextAccessor,
        ProblemDetailsHelper problemDetailsHelper)
        : base(httpContextAccessor, problemDetailsHelper, "v3")
    { }
}
