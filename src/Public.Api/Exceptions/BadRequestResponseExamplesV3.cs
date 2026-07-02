namespace Be.Vlaanderen.Basisregisters.Api.Exceptions;

using Microsoft.AspNetCore.Http;

public class BadRequestResponseExamplesV3 : BadRequestResponseExamples
{
    public BadRequestResponseExamplesV3(
        IHttpContextAccessor httpContextAccessor,
        ProblemDetailsHelper problemDetailsHelper) : base(httpContextAccessor, problemDetailsHelper, "v3")
    { }
}
