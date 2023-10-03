namespace Public.Api.Infrastructure.ProblemDetailsExceptionMapping
{
    using System;
    using System.Text.RegularExpressions;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using BuildingRegistry.Api.Oslo.Infrastructure.ParcelMatching.Wfs;
    using Microsoft.AspNetCore.Http;

    public class GrbWfsExceptionMapping : ApiProblemDetailsExceptionMapping
    {
        public override bool HandlesException(ApiProblemDetailsException exception)
        {
            const string nameSpacePlaceHolder = "NAMESPACE_WONT_REGEX_ESCAPE";
            var typeUri = ProblemDetails.GetTypeUriFor<GrbWfsException>(nameSpacePlaceHolder);
            var typeUriPattern = Regex
                .Escape(typeUri)
                .Replace(nameSpacePlaceHolder, ".*", StringComparison.InvariantCultureIgnoreCase);

            return new Regex(typeUriPattern).IsMatch(exception?.Details?.ProblemTypeUri ?? string.Empty);
        }

        public override ProblemDetails MapException(
            HttpContext httpContext,
            ApiProblemDetailsException exception,
            ProblemDetailsHelper problemDetailsHelper)
            => new ProblemDetails
            {
                HttpStatus = exception.StatusCode,
                Title = ProblemDetails.DefaultTitle,
                Detail = "Probleem bij het contacteren van de GRB WFS-service.",
                ProblemTypeUri = problemDetailsHelper.GetExceptionTypeUriFor<GrbWfsException>(),
                ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri(httpContext)}/{ProblemDetails.GetProblemNumber()}"
            };
    }
}
