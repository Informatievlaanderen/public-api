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
        private readonly IConfiguration _configuration;

        public ExtractDownloadModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var extractConfiguration = _configuration.GetSection("Extract:Download").Get<DownloadConfiguration>();

            var s3Config = _configuration.GetSection("Extract:S3").Get<S3Configuration>();
            var amazonS3Client = new AmazonS3Client(
                new BasicAWSCredentials(s3Config.ApiKey, s3Config.Secret),
                RegionEndpoint.GetBySystemName(s3Config.Region));

            builder
                .Register(context => new ExtractDownloads(amazonS3Client, extractConfiguration))
                .AsSelf();
        }

        private class S3Configuration
        {
            public string ApiKey { get; set; }
            public string Secret { get; set; }
            public string Region { get; set; }
        }
    }
}
