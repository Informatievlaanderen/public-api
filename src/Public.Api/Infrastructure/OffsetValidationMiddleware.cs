namespace Public.Api.Infrastructure
{
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    public class OffsetValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private const int DefaultMaxOffset = 1000000;
        private readonly int _maxOffset;

        public OffsetValidationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            if (!int.TryParse(configuration.GetValue<string>("MaxOffset"), out _maxOffset))
            {
                _maxOffset = DefaultMaxOffset;
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string offsetQueryString = context.Request.Query["offset"];

            if (!string.IsNullOrEmpty(offsetQueryString) && int.TryParse(offsetQueryString, out var offset))
            {
                if (offset > _maxOffset)
                {
                    throw new ValidationException(
                        new[]
                        {
                            new ValidationFailure("offset", $"De offset is beperkt tot {_maxOffset}, gebruik extra filters.", _maxOffset)
                        });
                }
            }

            await _next(context);
        }
    }
}
