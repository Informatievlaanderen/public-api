namespace Public.Api.Infrastructure
{
    using System;
    using Common.Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public static class BackendResponseExtensions
    {
        public static ActionResult ToActionResult(this IBackendResponse response)
        {
            switch (response)
            {
                case BackendResponse backendResponse:
                    return new BackendResponseResult(backendResponse);
                case StreamingBackendResponse streamingBackendResponse:
                    return new StreamingBackendResponseResult(streamingBackendResponse);
            }

            throw new ArgumentException("response has unsupported type", nameof(response));
        }
    }
}
