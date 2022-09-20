namespace Common.Infrastructure
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing;
    using StackExchange.Redis;
    using StackExchange.Redis.Profiling;

    public class TraceConnectionMultiplexer : IConnectionMultiplexer
    {
        private const string DefaultServiceName = "redis";

        private string ServiceName { get; }

        private readonly ISpanSource _spanSource;
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public string ClientName => _connectionMultiplexer.ClientName;

        public string Configuration => _connectionMultiplexer.Configuration;

        public int TimeoutMilliseconds => _connectionMultiplexer.TimeoutMilliseconds;

        public long OperationCount => _connectionMultiplexer.OperationCount;

        [Obsolete("Not supported; if you require ordered pub/sub, please see " + nameof(ChannelMessageQueue), false)]
        public bool PreserveAsyncOrder
        {
            get => _connectionMultiplexer.PreserveAsyncOrder;
            set => _connectionMultiplexer.PreserveAsyncOrder = value;
        }

        public bool IsConnected => _connectionMultiplexer.IsConnected;

        public bool IsConnecting => _connectionMultiplexer.IsConnecting;

        public bool IncludeDetailInExceptions
        {
            get => _connectionMultiplexer.IncludeDetailInExceptions;
            set => _connectionMultiplexer.IncludeDetailInExceptions = value;
        }

        public int StormLogThreshold
        {
            get => _connectionMultiplexer.StormLogThreshold;
            set => _connectionMultiplexer.StormLogThreshold = value;
        }

        public event EventHandler<RedisErrorEventArgs> ErrorMessage
        {
            add => _connectionMultiplexer.ErrorMessage += value;
            remove => _connectionMultiplexer.ErrorMessage -= value;
        }

        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed
        {
            add => _connectionMultiplexer.ConnectionFailed += value;
            remove => _connectionMultiplexer.ConnectionFailed -= value;
        }

        public event EventHandler<InternalErrorEventArgs> InternalError
        {
            add => _connectionMultiplexer.InternalError += value;
            remove => _connectionMultiplexer.InternalError -= value;
        }

        public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored
        {
            add => _connectionMultiplexer.ConnectionRestored += value;
            remove => _connectionMultiplexer.ConnectionRestored -= value;
        }

        public event EventHandler<EndPointEventArgs> ConfigurationChanged
        {
            add => _connectionMultiplexer.ConfigurationChanged += value;
            remove => _connectionMultiplexer.ConfigurationChanged -= value;
        }

        public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast
        {
            add => _connectionMultiplexer.ConfigurationChangedBroadcast += value;
            remove => _connectionMultiplexer.ConfigurationChangedBroadcast -= value;
        }

        public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved
        {
            add => _connectionMultiplexer.HashSlotMoved += value;
            remove => _connectionMultiplexer.HashSlotMoved -= value;
        }

        public TraceConnectionMultiplexer(ConnectionMultiplexer connectionMultiplexer)
            : this(connectionMultiplexer, DefaultServiceName, TraceContextSpanSource.Instance) { }

        public TraceConnectionMultiplexer(ConnectionMultiplexer connectionMultiplexer, string serviceName)
            : this(connectionMultiplexer, serviceName, TraceContextSpanSource.Instance) { }

        public TraceConnectionMultiplexer(ConnectionMultiplexer connectionMultiplexer, ISpanSource spanSource)
            : this(connectionMultiplexer, DefaultServiceName, spanSource) { }

        public TraceConnectionMultiplexer(ConnectionMultiplexer connectionMultiplexer, string serviceName, ISpanSource spanSource)
        {
            _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
            _spanSource = spanSource ?? throw new ArgumentNullException(nameof(spanSource));

            ServiceName = string.IsNullOrWhiteSpace(serviceName)
                ? DefaultServiceName
                : serviceName;
        }

        public void Dispose()
            => _connectionMultiplexer.Dispose();

        public void RegisterProfiler(Func<ProfilingSession> profilingSessionProvider)
            => _connectionMultiplexer.RegisterProfiler(profilingSessionProvider);

        public ServerCounters GetCounters()
            => _connectionMultiplexer.GetCounters();

        public EndPoint[] GetEndPoints(bool configuredOnly = false)
            => _connectionMultiplexer.GetEndPoints(configuredOnly);

        public void Wait(Task task)
            => _connectionMultiplexer.Wait(task);

        public T Wait<T>(Task<T> task)
            => _connectionMultiplexer.Wait(task);

        public void WaitAll(params Task[] tasks)
            => _connectionMultiplexer.WaitAll(tasks);

        public int HashSlot(RedisKey key)
            => _connectionMultiplexer.HashSlot(key);

        public ISubscriber GetSubscriber(object? asyncState = null)
            => _connectionMultiplexer.GetSubscriber(asyncState);

        public IDatabase GetDatabase(int db = -1, object? asyncState = null)
            => new TraceDatabase(
                _connectionMultiplexer.GetDatabase(db, asyncState),
                ServiceName,
                _spanSource);

        public IServer GetServer(string host, int port, object? asyncState = null)
            => _connectionMultiplexer.GetServer(host, port, asyncState);

        public IServer GetServer(string hostAndPort, object? asyncState = null)
            => _connectionMultiplexer.GetServer(hostAndPort, asyncState);

        public IServer GetServer(IPAddress host, int port)
            => _connectionMultiplexer.GetServer(host, port);

        public IServer GetServer(EndPoint endpoint, object? asyncState = null)
            => _connectionMultiplexer.GetServer(endpoint, asyncState);

        public Task<bool> ConfigureAsync(TextWriter? log = null)
            => _connectionMultiplexer.ConfigureAsync(log);

        public bool Configure(TextWriter? log = null)
            => _connectionMultiplexer.Configure(log);

        public string GetStatus()
            => _connectionMultiplexer.GetStatus();

        public void GetStatus(TextWriter log)
            => _connectionMultiplexer.GetStatus(log);

        public void Close(bool allowCommandsToComplete = true)
            => _connectionMultiplexer.Close(allowCommandsToComplete);

        public Task CloseAsync(bool allowCommandsToComplete = true)
            => _connectionMultiplexer.CloseAsync(allowCommandsToComplete);

        public string GetStormLog()
            => _connectionMultiplexer.GetStormLog();

        public void ResetStormLog()
            => _connectionMultiplexer.ResetStormLog();

        public long PublishReconfigure(CommandFlags flags = CommandFlags.None)
            => _connectionMultiplexer.PublishReconfigure(flags);

        public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None)
            => _connectionMultiplexer.PublishReconfigureAsync(flags);

        public int GetHashSlot(RedisKey key)
            => _connectionMultiplexer.GetHashSlot(key);

        public void ExportConfiguration(Stream destination, ExportOptions options = ExportOptions.All)
            => _connectionMultiplexer.ExportConfiguration(destination, options);
    }
}
