namespace Public.Api.Infrastructure.ProblemDetailsExceptionMappings
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Common.Infrastructure.ProblemDetailsException;
    using Microsoft.AspNetCore.Http;

    public class GoneExceptionMapping : ApiProblemDetailsExceptionMapping
    {
        public override bool HandlesException(ApiProblemDetailsException exception)
            => exception.GetType() == typeof(GoneException);

        public override ProblemDetails MapException(ApiProblemDetailsException exception, ProblemDetailsHelper problemDetailsHelper)
        {
            var registryName = ((GoneException) exception).RegistryName;
            return new ProblemDetails
            {
                ProblemTypeUri = $"urn:be.vlaanderen.basisregisters.api:{registryName}:gone",
                HttpStatus = StatusCodes.Status410Gone,
                Title = ProblemDetails.DefaultTitle,
                Detail = exception?.Message,
                ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}"
            };
        }
    }
}
