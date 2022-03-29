namespace Common.ProblemDetailsException;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;

public class ConflictException : ApiProblemDetailsException
{
    public string RegistryName { get; }

    public ConflictException(string message, string registryName) : base(message, StatusCodes.Status409Conflict, new ExceptionProblemDetails(), null)
    {
        RegistryName = registryName;
    }
}
