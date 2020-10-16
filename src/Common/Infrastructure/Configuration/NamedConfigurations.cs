namespace Common.Infrastructure.Configuration
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    public class NamedConfigurations<T>
        : Dictionary<string, T>
    {
        public NamedConfigurations(IConfiguration configuration, string sectionName)
        {
            configuration
                .GetSection(sectionName)
                .Bind(this);
        }
    }
}
