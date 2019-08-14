namespace Common.Infrastructure
{
    using StackExchange.Redis;

    public class ConnectionMultiplexerProvider
    {
        private readonly ApiRedisToggle _useRedis;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public ConnectionMultiplexerProvider(
            ApiRedisToggle useRedis,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _useRedis = useRedis;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public IConnectionMultiplexer GetConnectionMultiplexer()
            => _useRedis.FeatureEnabled
                ? _connectionMultiplexer
                : null;
    }
}
