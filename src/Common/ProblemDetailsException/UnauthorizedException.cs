namespace Common.ProblemDetailsException
{
    using System;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    [Serializable]
    public sealed class UnauthorizedException : ApiProblemDetailsException
    {
        private const string Message = "U bent niet geauthenticeerd om deze actie uit te voeren.";
        public string RegistryName { get; }

        public UnauthorizedException(string registryName) : base(Message, StatusCodes.Status401Unauthorized, new ExceptionProblemDetails(), null)
        {
            RegistryName = registryName;
        }

        private UnauthorizedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
