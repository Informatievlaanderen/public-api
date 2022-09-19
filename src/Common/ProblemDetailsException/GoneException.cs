namespace Common.ProblemDetailsException
{
    using System;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    [Serializable]
    public sealed class GoneException : ApiProblemDetailsException
    {
        public string RegistryName { get; }

        public GoneException(string message, string registryName) : base(message, StatusCodes.Status410Gone, new ExceptionProblemDetails(), null)
        {
            RegistryName = registryName;
        }
        
        private GoneException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
