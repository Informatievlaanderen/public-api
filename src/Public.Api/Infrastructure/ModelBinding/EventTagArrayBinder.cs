namespace Public.Api.Infrastructure.ModelBinding
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class EventTagArrayBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext context)
        {
            context.Result = Bind(context);
            return Task.CompletedTask;
        }

        private static ModelBindingResult Bind(ModelBindingContext context)
        {
            if (!context.ModelType.IsAssignableFrom(typeof(EventTag[])))
                return ModelBindingResult.Failed();

            var eventTags = context
                .ValueProvider
                .GetValue(context.ModelName)
                .Where(parameterValue => !string.IsNullOrWhiteSpace(parameterValue))
                .Select(parameterValue
                    => parameterValue
                        .Split(',')
                        .Where(tagValue => !string.IsNullOrWhiteSpace(tagValue))
                        .Select(tagValue => tagValue.Trim())
                        .Select(EventTag.Create))
                .Aggregate(
                    new List<EventTag>(),
                    (tags, tagValues) =>
                    {
                        tags.AddRange(tagValues);
                        return tags;
                    })
                .Distinct()
                .ToArray();

            return ModelBindingResult.Success(eventTags);
        }
    }

    public class EventTagArrayBinderAttribute : ModelBinderAttribute
    {
        public EventTagArrayBinderAttribute() : base(typeof(EventTagArrayBinder)) { }
    }
}
