namespace Common.ProblemDetailsException
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    public class GoneException : ApiProblemDetailsException
    {
        public string RegistryName { get; }
        public GoneException(string message, string registryName) : base(message, StatusCodes.Status410Gone, new ExceptionProblemDetails(), null)
        {
            RegistryName = registryName;
        }
    }
}
