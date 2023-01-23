namespace Public.Api.Infrastructure.ProblemDetailsExceptionMappings
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Common.ProblemDetailsException;
    using Microsoft.AspNetCore.Http;

    public class ForbiddenExceptionMapping : ApiProblemDetailsExceptionMapping
    {
        public override bool HandlesException(ApiProblemDetailsException exception)
            => exception.GetType() == typeof(ForbiddenException);

        public override ProblemDetails MapException(
            HttpContext httpContext,
            ApiProblemDetailsException exception,
            ProblemDetailsHelper problemDetailsHelper)
        {
            var registryName = ((ForbiddenException) exception).RegistryName;
            return new ProblemDetails
            {
                ProblemTypeUri = $"urn:be.vlaanderen.basisregisters.api:{registryName}:forbidden",
                HttpStatus = StatusCodes.Status403Forbidden,
                Title = ProblemDetails.DefaultTitle,
                Detail = exception.Message,
                ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri(httpContext)}/unauthorized"
            };
        }
    }
}
