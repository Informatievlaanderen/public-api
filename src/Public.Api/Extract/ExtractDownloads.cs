namespace Public.Api.Extract
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class ExtractDownloads
    {
        private readonly IAmazonS3 _client;
        private readonly DownloadConfiguration _config;
        private readonly MarketingVersion _version;

        public ExtractDownloads(IAmazonS3 s3Client, DownloadConfiguration config, MarketingVersion version)
        {
            _client = s3Client;
            _config = config;
            _version = version;
        }

        public async Task<IActionResult> RedirectToMostRecent(CancellationToken cancellationToken)
            => await RedirectTo(null, cancellationToken);

        public async Task<IActionResult> RedirectTo(DateTime extractDate, CancellationToken cancellationToken)
            => await RedirectTo((DateTime?) extractDate, cancellationToken);

        private async Task<IActionResult> RedirectTo(DateTime? extractDate, CancellationToken cancellationToken)
        {
            try
            {
                var extractObjectRegex = CreateFileRegexForDate(extractDate);
                var response = await _client.ListObjectsAsync(_config.Bucket, cancellationToken);
                var extract = response
                    ?.S3Objects
                    ?.Where(item => extractObjectRegex.IsMatch(item.Key))
                    .OrderByDescending(item => item.LastModified)
                    .FirstOrDefault();

                if (extract == null)
                    throw new ApiException("Onbestaand testbestand.", StatusCodes.Status404NotFound);

                var signedUrl = _client.GetPreSignedURL(
                    new GetPreSignedUrlRequest
                    {
                        BucketName = extract.BucketName,
                        Key = extract.Key,
                        Expires = DateTime.Now.AddSeconds(_config.ExpiresInSeconds)
                    });

                return new RedirectResult(signedUrl, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Regex CreateFileRegexForDate(DateTime? matchDate)
        {
            var prefix = _config.DestinationPath;
            if (!string.IsNullOrWhiteSpace(prefix))
                prefix = (prefix + "/").Replace("//", "/");

            const string extractDateFormat = "yyyyMMdd";
            var date = matchDate.HasValue
                    ? matchDate.Value.ToString(extractDateFormat)
                    : new Regex(@"[^-]").Replace(extractDateFormat, @"\d");

            var bundleName = _config.BundleName
                .Replace("[VERSION]", Regex.Escape(_version))
                .Replace("[DATE]", date);
            
            return new Regex(
                $@"^{prefix}{bundleName}\.zip$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
    }
}
