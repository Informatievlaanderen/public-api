namespace Public.Api.Infrastructure
{
    using System.Linq;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Implementation of Problem Details for HTTP APIs https://tools.ietf.org/html/rfc7807 with additional Validation Errors
    /// </summary>
    [DataContract(Name = "ProblemDetails", Namespace = "")]
    public class ModelStateProblemDetails : ValidationProblemDetails
    {
        // Here to make DataContractSerializer happy
        public ModelStateProblemDetails(ModelStateDictionary modelState)
        {
            ValidationErrors = modelState
                .Where(x => x.Value.Errors.Any())
                .ToDictionary(
                    x => x.Key,
                    x => new Errors(x.Value.Errors.Select(error => new ValidationError(error.ErrorMessage)).ToList()));
        }
    }
}
