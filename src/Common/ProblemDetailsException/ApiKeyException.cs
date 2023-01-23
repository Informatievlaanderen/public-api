namespace Common.ProblemDetailsException
{
    using System;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;

    [Serializable]
    public sealed class ApiKeyException : ApiProblemDetailsException
    {
        public ApiKeyException(string message) : base(message, StatusCodes.Status401Unauthorized, new ExceptionProblemDetails(), null)
        { }

        private ApiKeyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
