namespace Public.Api.Infrastructure.ProblemDetailsExceptionMappings
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Common.ProblemDetailsException;
    using Microsoft.AspNetCore.Http;

    public class UnauthorizedExceptionMapping : ApiProblemDetailsExceptionMapping
    {
        public override bool HandlesException(ApiProblemDetailsException exception)
            => exception.GetType() == typeof(UnauthorizedException);

        public override ProblemDetails MapException(
            HttpContext httpContext,
            ApiProblemDetailsException exception,
            ProblemDetailsHelper problemDetailsHelper)
        {
            var registryName = ((UnauthorizedException) exception).RegistryName;
            return new ProblemDetails
            {
                ProblemTypeUri = $"urn:be.vlaanderen.basisregisters.api:{registryName}:unauthorized",
                HttpStatus = StatusCodes.Status401Unauthorized,
                Title = ProblemDetails.DefaultTitle,
                Detail = exception.Message,
                ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri(httpContext)}/{ProblemDetails.GetProblemNumber()}"
            };
        }
    }
}
