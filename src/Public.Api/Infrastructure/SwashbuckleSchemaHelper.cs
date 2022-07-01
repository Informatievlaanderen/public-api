namespace Public.Api.Infrastructure
{
    using System;
    using System.Reflection;

    internal static class SwashbuckleSchemaHelper
    {
        private static readonly string _editAssemblyName = typeof(Be.Vlaanderen.Basisregisters.GrAr.Edit.GmlConstants).GetTypeInfo().Assembly.GetName().Name;

        public static string GetSchemaId(Type type)
        {
            if (type.ToString()
                .StartsWith(_editAssemblyName, StringComparison.OrdinalIgnoreCase))
            {
                return $"Edit.{type.Name}";
            }

            return type.Name;
        }
    }
}
