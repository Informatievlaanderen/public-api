namespace Public.Api.Extract
{
    public class DownloadConfiguration
    {
        public string Bucket { get; set; }
        public string DestinationPath { get; set; }
        public string BundleName { get; set; }
        public int ExpiresInSeconds { get; set; }
    }
}
