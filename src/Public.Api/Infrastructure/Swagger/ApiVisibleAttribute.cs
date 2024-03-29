namespace Public.Api.Infrastructure.Swagger
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class ApiVisibleAttribute : Attribute
    {
        public bool Visible { get; }

        public ApiVisibleAttribute() : this(true) { }
        public ApiVisibleAttribute(bool visible) => Visible = visible;
    }
}
