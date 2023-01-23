namespace Public.Api.Infrastructure.ProblemDetailsExceptionMappings
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Common.ProblemDetailsException;
    using Microsoft.AspNetCore.Http;

    public class ApiKeyExceptionMapping : ApiProblemDetailsExceptionMapping
    {
        public override bool HandlesException(ApiProblemDetailsException exception)
            => exception.GetType() == typeof(ApiKeyException);

        public override ProblemDetails MapException(
            HttpContext httpContext,
            ApiProblemDetailsException exception,
            ProblemDetailsHelper problemDetailsHelper)
        {
            return new ProblemDetails
            {
                ProblemTypeUri = $"urn:be.vlaanderen.basisregisters.api:unauthorized",
                HttpStatus = StatusCodes.Status401Unauthorized,
                Title = ProblemDetails.DefaultTitle,
                Detail = exception.Message,
                ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri(httpContext)}/{ProblemDetails.GetProblemNumber()}"
            };
        }
    }
}
