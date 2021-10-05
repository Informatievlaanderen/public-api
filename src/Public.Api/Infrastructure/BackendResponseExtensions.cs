namespace Public.Api.Infrastructure
{
    using System;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public static class BackendResponseExtensions
    {
        public static ActionResult ToActionResult(
            this IBackendResponse response,
            BackendResponseResultOptions options = default)
        {
            switch (response)
            {
                case BackendResponse backendResponse:
                    return new BackendResponseResult(backendResponse, options);
                case StreamingBackendResponse streamingBackendResponse:
                    return new StreamingBackendResponseResult(streamingBackendResponse, options);
            }

            throw new ArgumentException("response has unsupported type", nameof(response));
        }
    }
}
