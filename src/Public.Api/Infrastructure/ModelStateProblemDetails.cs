namespace Public.Api.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.BasicApiProblem;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Newtonsoft.Json;

    /// <summary>
    /// Implementation of Problem Details for HTTP APIs https://tools.ietf.org/html/rfc7807 with additional Validation Errors
    /// </summary>
    public class ModelStateProblemDetails : StatusCodeProblemDetails
    {
        /// <summary>Validatie fouten.</summary>
        [JsonProperty("validationErrors", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DataMember(Name = "validationErrors", Order = 600, EmitDefaultValue = false)]
        public Dictionary<string, string[]> ValidationErrors { get; set; }

        // Here to make DataContractSerializer happy
        public ModelStateProblemDetails(ModelStateDictionary modelState) : base(StatusCodes.Status400BadRequest)
        {
            Title = "Er heeft zich een fout voorgedaan!";
            Detail = "Validatie mislukt.";
            ProblemInstanceUri = GetProblemNumber();
            ProblemTypeUri = "urn:base-registries:validation".ToLowerInvariant();

            ValidationErrors = modelState
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(error => error.ErrorMessage).ToArray());
        }
    }
}
