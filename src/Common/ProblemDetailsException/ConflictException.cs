namespace Common.ProblemDetailsException
{
    using System;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    [Serializable]
    public sealed class ConflictException : ApiProblemDetailsException
    {
        public string RegistryName { get; }

        public ConflictException(string message, string registryName) : base(message, StatusCodes.Status409Conflict, new ExceptionProblemDetails(), null)
        {
            RegistryName = registryName;
        }
        
        private ConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
