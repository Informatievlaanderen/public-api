namespace Public.Api.Infrastructure.ProblemDetailsExceptionMappings
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Common.ProblemDetailsException;
    using Microsoft.AspNetCore.Http;

    public class NotFoundExceptionMapping : ApiProblemDetailsExceptionMapping
    {
        public override bool HandlesException(ApiProblemDetailsException exception)
            => exception.GetType() == typeof(NotFoundException);

        public override ProblemDetails MapException(
            HttpContext httpContext,
            ApiProblemDetailsException exception,
            ProblemDetailsHelper problemDetailsHelper)
        {
            var registryName = ((NotFoundException) exception).RegistryName;
            return new ProblemDetails
            {
                ProblemTypeUri = $"urn:be.vlaanderen.basisregisters.api:{registryName}:not-found",
                HttpStatus = StatusCodes.Status404NotFound,
                Title = ProblemDetails.DefaultTitle,
                Detail = exception?.Message,
                ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri(httpContext)}/{ProblemDetails.GetProblemNumber()}"
            };
        }
    }
}
