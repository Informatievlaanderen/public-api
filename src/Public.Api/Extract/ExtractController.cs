namespace Public.Api.Extract
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using FluentValidation;
    using FluentValidation.Results;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
    using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;
    using Version = Infrastructure.Version.Version;

    [ApiVersion(Version.Current)]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.CurrentAdvertised)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "RoadExtract")]
    [ApiOrder(ApiOrder.Extract)]
    [Produces(MediaTypeNames.Application.Zip)]
    public class ExtractController : ApiController<ExtractController>
    {
        private readonly ExtractDownloads _extractDownloads;
        private const int NoCaching = 0;

        public ExtractController(
            IHttpContextAccessor httpContextAccessor,
            ConnectionMultiplexerProvider redis,
            ILogger<ExtractController> logger,
            ExtractDownloads extractDownloads)
            : base(httpContextAccessor, redis, logger)
            => _extractDownloads = extractDownloads;

        /// <summary>Download het meest recente extract.</summary>
        /// <param name="cancellationToken"></param>
        /// <response code="302">Als de extract download gevonden is.</response>
        /// <response code="404">Als er geen extract download gevonden kan worden.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("extract", Name = nameof(DownloadLatestExtract))]
        [HttpCacheExpiration(MaxAge = NoCaching)]
        [ProducesResponseType(typeof(void), StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status302Found, typeof(ExtractRedirectResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ExtractNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> DownloadLatestExtract(CancellationToken cancellationToken = default)
            => await _extractDownloads.RedirectToMostRecent(cancellationToken);

        /// <summary>Download het extract voor de gevraagde datum.</summary>
        /// <param name="extractDate">yyyy-MM-dd</param>
        /// <param name="cancellationToken"></param>
        /// <response code="302">Als de extract download gevonden is.</response>
        /// <response code="400">Als de extract download datum niet het correcte formaat heeft.</response>
        /// <response code="404">Als de extract download niet gevonden kan worden.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("extract/{extractDate}", Name = nameof(DownloadExtractForDate))]
        [HttpCacheExpiration(MaxAge = NoCaching)]
        [ProducesResponseType(typeof(void), StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status302Found, typeof(ExtractRedirectResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ExtractBadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ExtractNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> DownloadExtractForDate(string extractDate, CancellationToken cancellationToken = default)
            => DateTime.TryParseExact(extractDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                ? await _extractDownloads.RedirectTo(date, cancellationToken)
                : throw new ValidationException(new List<ValidationFailure>
                {
                    new ValidationFailure("extractDate", "Ongeldige datum.")
                });
    }

    public class ExtractRedirectResponseExamples : IExamplesProvider<object>
    {
        public object GetExamples() => new object();
    }

    public class ExtractBadRequestResponseExamples : IExamplesProvider<ValidationProblemDetails>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProblemDetailsHelper _problemDetailsHelper;

        public ExtractBadRequestResponseExamples(
            IHttpContextAccessor httpContextAccessor,
            ProblemDetailsHelper problemDetailsHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _problemDetailsHelper = problemDetailsHelper;
        }

        public ValidationProblemDetails GetExamples() =>
            new ValidationProblemDetails
            {
                Title = ProblemDetails.DefaultTitle,
                ValidationErrors = new Dictionary<string, ValidationProblemDetails.Errors>
                {
                    {"extractDate", new ValidationProblemDetails.Errors{ new ValidationError("Ongeldige datum.")}}
                },
                ProblemInstanceUri = _problemDetailsHelper.GetInstanceUri(_httpContextAccessor.HttpContext)
            };
    }

    public class ExtractNotFoundResponseExamples : IExamplesProvider<ProblemDetails>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProblemDetailsHelper _problemDetailsHelper;

        public ExtractNotFoundResponseExamples(
            IHttpContextAccessor httpContextAccessor,
            ProblemDetailsHelper problemDetailsHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _problemDetailsHelper = problemDetailsHelper;
        }

        public ProblemDetails GetExamples()
            => new ProblemDetails
            {
                HttpStatus = StatusCodes.Status404NotFound,
                Title = ProblemDetails.DefaultTitle,
                Detail = "Onbestaand testbestand.",
                ProblemInstanceUri = _problemDetailsHelper.GetInstanceUri(_httpContextAccessor.HttpContext)
            };
    }
}
