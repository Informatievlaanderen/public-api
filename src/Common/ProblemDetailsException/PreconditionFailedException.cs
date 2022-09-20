namespace Common.ProblemDetailsException
{
    using System;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    [Serializable]
    public sealed class PreconditionFailedException : ApiProblemDetailsException
    {
        public string RegistryName { get; }

        public PreconditionFailedException(string message, string registryName) : base(message, StatusCodes.Status412PreconditionFailed, new ExceptionProblemDetails(), null)
        {
            RegistryName = registryName;
        }
    
        private PreconditionFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
