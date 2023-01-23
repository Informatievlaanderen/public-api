namespace Common.ProblemDetailsException
{
    using System;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    [Serializable]
    public sealed class ForbiddenException : ApiProblemDetailsException
    {
        private const string Message = "U beschikt niet over de correcte rechten om deze actie uit te voeren.";
        public string RegistryName { get; }

        public ForbiddenException(string registryName) : base(Message, StatusCodes.Status403Forbidden, new ExceptionProblemDetails(), null)
        {
            RegistryName = registryName;
        }

        private ForbiddenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
