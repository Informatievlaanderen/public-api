namespace Be.Vlaanderen.Basisregisters.Api.Exceptions;

using Microsoft.AspNetCore.Http;

public class PreconditionFailedResponseExamplesV3 : PreconditionFailedResponseExamples
{
    public PreconditionFailedResponseExamplesV3(
        IHttpContextAccessor httpContextAccessor,
        ProblemDetailsHelper problemDetailsHelper) : base(httpContextAccessor, problemDetailsHelper, "v3")
    { }
}
