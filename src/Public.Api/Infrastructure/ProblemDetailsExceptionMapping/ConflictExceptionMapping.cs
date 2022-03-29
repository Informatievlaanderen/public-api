namespace Public.Api.Infrastructure.ProblemDetailsExceptionMappings;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Common.ProblemDetailsException;
using Microsoft.AspNetCore.Http;

public class ConflictExceptionMapping : ApiProblemDetailsExceptionMapping
{
    public override bool HandlesException(ApiProblemDetailsException exception)
        => exception.GetType() == typeof(ConflictException);

    public override ProblemDetails MapException(
        ApiProblemDetailsException exception,
        ProblemDetailsHelper problemDetailsHelper)
    {
        var registryName = ((ConflictException)exception).RegistryName;
        return new ProblemDetails
        {
            HttpStatus = StatusCodes.Status409Conflict,
            Title = ProblemDetails.DefaultTitle,
            Detail = "Er is een conflict met de laatste versie van deze resource.",
            ProblemTypeUri = $"urn:be.vlaanderen.basisregisters.api:{registryName}:conflict",
            ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}"
        };
    }
}
