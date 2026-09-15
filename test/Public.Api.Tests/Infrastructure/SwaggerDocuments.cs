namespace Public.Api.Tests.Infrastructure
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>The OpenAPI documents of the public API, built as the documentation asks Swashbuckle for them.</summary>
    public static class SwaggerDocuments
    {
        public static IReadOnlyCollection<string> Names(PublicApiTestHost host)
            => host.Services.GetRequiredService<IOptions<SwaggerGeneratorOptions>>().Value.SwaggerDocs.Keys.ToList();

        public static OpenApiDocument Build(PublicApiTestHost host, string documentName)
        {
            using var scope = host.Services.CreateScope();

            // Some response example providers read IHttpContextAccessor.HttpContext to build a problem details instance
            // URI. HttpContextAccessor keeps it in a static AsyncLocal, so a context set here flows into GetSwagger - and
            // is put back afterwards, so that no test after this one finds a context whose services are disposed of.
            var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            var previousHttpContext = httpContextAccessor.HttpContext;

            var httpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost");
            httpContextAccessor.HttpContext = httpContext;

            try
            {
                return scope.ServiceProvider.GetRequiredService<ISwaggerProvider>().GetSwagger(documentName);
            }
            finally
            {
                httpContextAccessor.HttpContext = previousHttpContext;
            }
        }
    }
}
