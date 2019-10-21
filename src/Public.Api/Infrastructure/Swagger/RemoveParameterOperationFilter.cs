namespace Public.Api.Infrastructure.Swagger
{
    using System.Linq;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class RemoveParameterOperationFilter : IOperationFilter
    {
        private readonly string _parameterName;

        public RemoveParameterOperationFilter(string parameterName)
        {
            _parameterName = parameterName.ToLowerInvariant();
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            if (operation.Parameters.Count == 0)
                return;

            var parameterToRemove = operation.Parameters.FirstOrDefault(x => x.Name.ToLowerInvariant() == _parameterName);

            if (parameterToRemove != null)
               operation.Parameters.Remove(parameterToRemove);
        }
    }
}
