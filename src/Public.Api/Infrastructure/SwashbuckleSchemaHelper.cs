namespace Public.Api.Infrastructure
{
    using System;
    using System.Reflection;

    internal static class SwashbuckleSchemaHelper
    {
        private static readonly string _editAssemblyName = typeof(Be.Vlaanderen.Basisregisters.GrAr.Edit.GmlConstants).GetTypeInfo().Assembly.GetName().Name;
        private static readonly string _osloAssemblyName = typeof(Be.Vlaanderen.Basisregisters.GrAr.Oslo.GestructureerdeIdentificator).GetTypeInfo().Assembly.GetName().Name;

        public static string GetSchemaId(Type type)
        {
            if (type.ToString()
                .StartsWith(_editAssemblyName, StringComparison.OrdinalIgnoreCase))
            {
                return $"Edit.{type.Name}";
            }

            if (type.ToString()
                .StartsWith(_osloAssemblyName, StringComparison.OrdinalIgnoreCase))
            {
                return $"Oslo.{type.Name}";
            }

            return type.Name;
        }
    }
}
