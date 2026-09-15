namespace Public.Api.Tests.Infrastructure
{
    using FluentAssertions;

    /// <summary>
    /// Guards that Swashbuckle can build the OpenAPI document of every API version, as the documentation asks for it.
    /// A controller or response type that introduces a schema conflict - a duplicate schema id, an unresolvable type -
    /// fails here instead of only at runtime, as a 500 on the documentation.
    /// </summary>
    [Collection(PublicApiCollection.Name)]
    public class SwaggerTests
    {
        private readonly PublicApiTestHost _host;

        public SwaggerTests(PublicApiTestHost host)
        {
            _host = host;
        }

        [Fact]
        public void SwaggerDocumentCanBeBuiltForEveryApiVersion()
        {
            var documentNames = SwaggerDocuments.Names(_host);

            documentNames.Should().NotBeEmpty();

            foreach (var documentName in documentNames)
            {
                var buildSwaggerDocument = () => SwaggerDocuments.Build(_host, documentName);

                buildSwaggerDocument.Should().NotThrow(
                    $"the OpenAPI document for '{documentName}' must be buildable, or the documentation answers a 500");
            }
        }
    }
}
