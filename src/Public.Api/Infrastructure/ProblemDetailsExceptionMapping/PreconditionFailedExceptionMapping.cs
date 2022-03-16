namespace Public.Api.Infrastructure.ProblemDetailsExceptionMappings
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Common;
    using Common.ProblemDetailsException;
    using Microsoft.AspNetCore.Http;

    public class PreconditionFailedExceptionMapping : ApiProblemDetailsExceptionMapping
    {
        public override bool HandlesException(ApiProblemDetailsException exception)
            => exception.GetType() == typeof(PreconditionFailedException);

        public override ProblemDetails MapException(
            ApiProblemDetailsException exception,
            ProblemDetailsHelper problemDetailsHelper)
        {
            var registryName = ((PreconditionFailedException)exception).RegistryName;
            return new ProblemDetails
            {
                HttpStatus = StatusCodes.Status412PreconditionFailed,
                Title = ProblemDetails.DefaultTitle,
                Detail = "De If-Match header komt niet overeen met de laatst gekende ETag.",
                ProblemTypeUri = $"urn:be.vlaanderen.basisregisters.api:{registryName}:precondition-failed",
                ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}"
            };
        }
    }
}
