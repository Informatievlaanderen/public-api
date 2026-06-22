namespace Be.Vlaanderen.Basisregisters.Api.Exceptions;

using Microsoft.AspNetCore.Http;

public class InternalServerErrorResponseExamplesV3 : InternalServerErrorResponseExamples
{
    public InternalServerErrorResponseExamplesV3(
        IHttpContextAccessor httpContextAccessor,
        ProblemDetailsHelper problemDetailsHelper) : base(httpContextAccessor, problemDetailsHelper, "v3")
    { }
}
