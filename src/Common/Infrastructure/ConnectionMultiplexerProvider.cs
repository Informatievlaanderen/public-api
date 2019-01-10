namespace Common.Infrastructure
{
    using StackExchange.Redis;

    public class ConnectionMultiplexerProvider
    {
        private readonly ApiRedisToggle _useRedis;
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public ConnectionMultiplexerProvider(ApiRedisToggle useRedis, ConnectionMultiplexer connectionMultiplexer)
        {
            _useRedis = useRedis;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public ConnectionMultiplexer GetConnectionMultiplexer() => _useRedis.FeatureEnabled ? _connectionMultiplexer : null;
    }
}
