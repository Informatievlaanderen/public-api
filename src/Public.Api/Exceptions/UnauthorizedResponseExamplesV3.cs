namespace Be.Vlaanderen.Basisregisters.Api.Exceptions;

using Microsoft.AspNetCore.Http;

public class UnauthorizedResponseExamplesV3 : UnauthorizedResponseExamples
{
    public UnauthorizedResponseExamplesV3(
        IHttpContextAccessor httpContextAccessor,
        ProblemDetailsHelper problemDetailsHelper) : base(httpContextAccessor, problemDetailsHelper, "v3")
    { }
}
