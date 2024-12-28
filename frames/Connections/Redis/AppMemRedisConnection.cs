using IConnections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using StackExchange.Redis;

namespace Connections.Redis
{
    public class AppMemRedisConnection : IAppMemConnection
    {
        public IAppMemComfiguration Configuration { get; set; }

        private ConfigurationOptions? _redisOptions = null;
        private IDatabase? _db = null;
        private ConnectionMultiplexer? _cluster = null;
        private ILogger<AppMemRedisConnection> _logger { get; set; }

        public bool IsConnected { get; set; }

        public AppMemRedisConnection(IConfiguration configuration, ILogger<AppMemRedisConnection> logger)
        {
            var connSession = configuration?.GetSection("RedisServer");
            var ip = connSession?["IP"];
            var port = connSession?["Port"] + "";
            var db = connSession?["DB"] + "";
            var CollectionName = connSession?["CollectionName"] + "";
            if (string.IsNullOrEmpty(ip))
                throw new AppDBHandleException("Connection string of Mongo Server is empty");

            if (string.IsNullOrEmpty(port))
                throw new AppDBHandleException("Database of Mongo Server is empty");

            _logger = logger;
            _logger.LogDebug("AppMongoConnection - constructure");
            Configuration = new AppMemRedisComfiguration
            {
                IP = ip,
                Port = port,
                DB = int.Parse(db),
                Username = "",
                Password =""
            };
            _redisOptions = new ConfigurationOptions
            {
                EndPoints = {
                    { Configuration.IP, int.Parse(Configuration.Port) },
                },
                AllowAdmin = true
            };

            _cluster = ConnectionMultiplexer.Connect(_redisOptions);
            _db = _cluster.GetDatabase(Configuration.DB);
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll<T>(List<string> keys, string prefix = "", AppMemDataType dataType = AppMemDataType.RedisStringString)
        {
            throw new NotImplementedException();
        }

        public T GetItem<T>(string key, AppMemDataType dataType = AppMemDataType.RedisStringString)
        {
            throw new NotImplementedException();
        }

        public bool InsertItem<T>(string key, T Listitem, AppMemDataType dataType = AppMemDataType.RedisStringString)
        {
            throw new NotImplementedException();
        }

        public bool IsConnect()
        {
            throw new NotImplementedException();
        }

        public bool ListRemoveItem<T>(string key, AppMemDataType dataType = AppMemDataType.RedisStringString)
        {
            throw new NotImplementedException();
        }

        public T ListTryRemoveItem<T>(string key, AppMemDataType dataType = AppMemDataType.RedisStringString)
        {
            throw new NotImplementedException();
        }

        public bool RemoveKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool Test()
        {
            ConnectionMultiplexer cluster = null!;
            try
            {
                var redisOptions = new ConfigurationOptions
                {
                    EndPoints = {
                    { Configuration.IP, int.Parse(Configuration.Port) },
                },
                    AllowAdmin = true
                };

                cluster = ConnectionMultiplexer.Connect(redisOptions);
                return cluster.IsConnected;
                
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                cluster?.Close();
            }
        }

        public bool UpdateItem<T>(string key, T Listitem, AppMemDataType dataType = AppMemDataType.RedisStringString)
        {
            throw new NotImplementedException();
        }
    }

    public class AppMemRedisComfiguration : IAppMemComfiguration
    {
        public string IP { get; set; } = "";
        public string Port { get; set; } = "";
        public int DB { get; set; } = 0;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public override string ToString()
        {
            return $"IP: {IP} | Port: {Port} | DB: {DB} | Username: {Username} | Password: {Password}";
        }
    }
}
