namespace Public.Api.Extract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Microsoft.AspNetCore.Mvc;

    public class ExtractDownloads
    {
        private readonly IAmazonS3 _client;
        private readonly DownloadConfiguration _config;

        public ExtractDownloads(IAmazonS3 s3Client, DownloadConfiguration config)
        {
            _client = s3Client;
            _config = config;
        }

        public async Task<IActionResult> RedirectToMostRecent(CancellationToken cancellationToken)
        {
            S3Object FindMostRecent(IEnumerable<S3Object> extracts) =>
                extracts
                    .OrderByDescending(item => item.LastModified)
                    .FirstOrDefault();

            return await RedirectTo(FindMostRecent, cancellationToken);
        }

        public async Task<IActionResult> RedirectTo(DateTime? extractDate, CancellationToken cancellationToken)
        {
            if (!extractDate.HasValue)
                return new BadRequestResult();

            S3Object FindByExtractDate(IEnumerable<S3Object> extracts)
            {
                var extractDownloadRegex = CreateFileRegexForDate(extractDate);
                return extracts.FirstOrDefault(item => extractDownloadRegex.IsMatch(item.Key));
            }

            return await RedirectTo(FindByExtractDate, cancellationToken);
        }

        private async Task<IActionResult> RedirectTo(Func<IEnumerable<S3Object>, S3Object> selectDownload, CancellationToken cancellationToken)
        {
            var extractObjectRegex = CreateFileRegexForDate(null);
            var response = await _client.ListObjectsAsync(_config.Bucket, cancellationToken);
            var extracts = response
                               ?.S3Objects
                               ?.Where(item => extractObjectRegex.IsMatch(item.Key));
            var extract = selectDownload(extracts ?? new S3Object[0]);

            if (extract == null)
                return new NotFoundResult();

            var signedUrl = _client.GetPreSignedURL(
                new GetPreSignedUrlRequest
                {
                    BucketName = extract.BucketName,
                    Key = extract.Key,
                    Expires = DateTime.Now.AddSeconds(_config.ExpiresInSeconds)
                });

            return new RedirectResult(signedUrl, false);
        }

        private Regex CreateFileRegexForDate(DateTime? matchDate)
        {
            var prefix = _config.DestinationPath;
            if (!string.IsNullOrWhiteSpace(prefix))
                prefix = (prefix + "/").Replace("//", "/");

            const string extractDateFormat = "yyyy-MM-dd";
            var date = matchDate.HasValue
                    ? matchDate.Value.ToString(extractDateFormat)
                    : new Regex("[^-]").Replace(extractDateFormat, "\\d");

            return new Regex(
                $"^{prefix}{_config.BundleName}-{date}\\.zip$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
    }
}
