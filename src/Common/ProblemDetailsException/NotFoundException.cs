namespace Common.ProblemDetailsException
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    public class NotFoundException: ApiProblemDetailsException
    {
        public string RegistryName { get; }
        public NotFoundException(string message, string registryName): base(message, StatusCodes.Status404NotFound, new ExceptionProblemDetails(), null)
        {
            RegistryName = registryName;
        }
    }
}
