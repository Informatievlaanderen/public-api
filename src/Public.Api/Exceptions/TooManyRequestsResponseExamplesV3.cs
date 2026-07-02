namespace Be.Vlaanderen.Basisregisters.Api.Exceptions;

using Microsoft.AspNetCore.Http;

public class TooManyRequestsResponseExamplesV3 : TooManyRequestsResponseExamples
{
    public TooManyRequestsResponseExamplesV3(
        IHttpContextAccessor httpContextAccessor,
        ProblemDetailsHelper problemDetailsHelper) : base(httpContextAccessor, problemDetailsHelper, "v3")
    { }
}
