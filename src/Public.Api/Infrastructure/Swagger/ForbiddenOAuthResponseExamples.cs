namespace Public.Api.Infrastructure.Swagger
{
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;
    using Swashbuckle.AspNetCore.Filters;

    public class ForbiddenOAuthResponseExamples : IExamplesProvider<ProblemDetails>
    {
        protected string ApiVersion { get; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProblemDetailsHelper _problemDetailsHelper;

        public ForbiddenOAuthResponseExamples(
            IHttpContextAccessor httpContextAccessor,
            ProblemDetailsHelper problemDetailsHelper,
            string apiVersion = "v2")
        {
            _httpContextAccessor = httpContextAccessor;
            _problemDetailsHelper = problemDetailsHelper;
            ApiVersion = apiVersion;
        }

        public ProblemDetails GetExamples() =>
            new ProblemDetails
            {
                ProblemTypeUri = "urn:be.vlaanderen.basisregisters.api:forbidden",
                HttpStatus = StatusCodes.Status403Forbidden,
                Title = ProblemDetails.DefaultTitle,
                Detail = "U beschikt niet over de correcte rechten om deze actie uit te voeren.",
                ProblemInstanceUri = _problemDetailsHelper.GetInstanceUri(_httpContextAccessor.HttpContext, ApiVersion)
            };
    }

    public class ForbiddenOAuthResponseExamplesV2 : ForbiddenOAuthResponseExamples
    {
        public ForbiddenOAuthResponseExamplesV2(
            IHttpContextAccessor httpContextAccessor,
            ProblemDetailsHelper problemDetailsHelper) : base(httpContextAccessor, problemDetailsHelper, "v2")
        {
        }
    }
}
