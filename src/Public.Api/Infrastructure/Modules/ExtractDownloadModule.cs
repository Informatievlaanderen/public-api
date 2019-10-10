namespace Public.Api.Infrastructure.Modules
{
    using Amazon;
    using Amazon.Runtime;
    using Amazon.S3;
    using Autofac;
    using Extract;
    using Microsoft.Extensions.Configuration;

    public class ExtractDownloadModule : Module
    {
        private readonly IConfigurationSection _extractConfiguration;

        public ExtractDownloadModule(IConfiguration configuration)
        {
            _extractConfiguration = configuration.GetSection("Extract");
        }

        protected override void Load(ContainerBuilder builder)
        {
            var extractConfiguration = _extractConfiguration.Get<DownloadConfiguration>();
            var amazonS3Client = CreateS3Client();

            builder
                .Register(context => new ExtractDownloads(amazonS3Client, extractConfiguration))
                .AsSelf();
        }

        private AmazonS3Client CreateS3Client()
        {
            var localCredentials = _extractConfiguration
                .GetSection("LocalS3Credentials")
                .Get<LocalS3Credentials>();

            var region = RegionEndpoint.GetBySystemName(_extractConfiguration.GetValue<string>("Region"));

            return localCredentials?.Configured ?? false
                ? new AmazonS3Client(localCredentials, region)
                : new AmazonS3Client(region);
        }

        private class LocalS3Credentials : AWSCredentials
        {
            public string ApiKey { get; set; }
            public string Secret { get; set; }

            public bool Configured => !string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(Secret);

            public override ImmutableCredentials GetCredentials() => new ImmutableCredentials(ApiKey, Secret, null);
        }
    }
}
