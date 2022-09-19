namespace Common.Infrastructure.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Extensions.Configuration;

    [Serializable]
    public sealed class NamedConfigurations<T> : Dictionary<string, T>
    {
        public NamedConfigurations(IConfiguration configuration, string sectionName)
        {
            configuration
                .GetSection(sectionName)
                .Bind(this);
        }

        private NamedConfigurations(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
