namespace Common.Infrastructure.Configuration
{
    using System.Collections.Generic;

    public class ApiConfigurationSection : Dictionary<string, ApiConfiguration>
    {
    }

    public class ApiConfiguration
    {
        public string ApiUrl { get; set; }
        public string HealthUrl { get; set; }
        public bool UseCache { get; set; }
    }
}
