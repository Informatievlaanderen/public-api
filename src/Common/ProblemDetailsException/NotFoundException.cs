namespace Common.ProblemDetailsException
{
    using System;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    [Serializable]
    public sealed class NotFoundException : ApiProblemDetailsException
    {
        public string RegistryName { get; }

        public NotFoundException(string message, string registryName): base(message, StatusCodes.Status404NotFound, new ExceptionProblemDetails(), null)
        {
            RegistryName = registryName;
        }
    
        private NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
