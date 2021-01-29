namespace Public.Api.Infrastructure.ProblemDetailsExceptionMapping
{
    using System;
    using System.Text.RegularExpressions;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using BuildingRegistry.Api.Legacy.Infrastructure.Grb.Wfs;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

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

        public override ProblemDetails MapException(ApiProblemDetailsException exception, ProblemDetailsHelper problemDetailsHelper)
            => new ProblemDetails
            {
                HttpStatus = exception.StatusCode,
                Title = ProblemDetails.DefaultTitle,
                Detail = "Probleem bij het contacteren van de GRB WFS-service.",
                ProblemTypeUri = problemDetailsHelper.GetExceptionTypeUriFor<GrbWfsException>(),
                ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}"
            };
    }
}
