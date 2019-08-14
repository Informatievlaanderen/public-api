namespace Public.Api.Infrastructure
{
    using Autofac;
    using Common.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using StackExchange.Redis;

    public class RedisModule : Module
    {
        private readonly bool _redis;
        private readonly string _redisConnectionstring;
        private readonly string _clientName;

        public RedisModule(IConfiguration configuration)
        {
            if (!bool.TryParse(configuration["Redis:Enabled"], out _redis))
                _redis = false;

            _redisConnectionstring = configuration["Redis:ConnectionString"];
            _clientName = configuration["Redis:ClientName"];
        }

        protected override void Load(ContainerBuilder builder)
        {
            var useRedis = new ApiRedisToggle(_redis);
            builder.Register(c => useRedis);

            if (!_redis)
            {
                builder
                    .RegisterInstance(new ConnectionMultiplexerProvider(useRedis, null))
                    .As<ConnectionMultiplexerProvider>()
                    .SingleInstance();

                return;
            }

            var options = ConfigurationOptions.Parse(_redisConnectionstring);

            options.ClientName = _clientName;
            options.ConnectRetry = 3;
            options.KeepAlive = 60;
            options.ReconnectRetryPolicy = new ExponentialRetry(5000);

            var connectionMultiplexer = ConnectionMultiplexer.Connect(options);
            var redis = new TraceConnectionMultiplexer(connectionMultiplexer);

            builder
                .RegisterInstance(new ConnectionMultiplexerProvider(useRedis, redis))
                .As<ConnectionMultiplexerProvider>()
                .SingleInstance();
        }
    }
}
