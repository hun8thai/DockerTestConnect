using Connections.Mongo;
using Connections.MsSql;
using Connections.MySql;
using Connections.Redis;
using IConnections;
using Microsoft.AspNetCore.Mvc;

namespace DockerTestConnect.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestConnectionController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<TestConnectionController> _logger;
        private readonly IAppDBConnection? _appMSSQLConnection;
        private readonly IAppDBConnection? _appMongoConnection;
        private readonly IAppDBConnection? _appMySqlConnection;
        private readonly IAppMemConnection? _appMemRedisConnection;

        public TestConnectionController(ILogger<TestConnectionController> logger, 
            IEnumerable<IAppDBConnection> services, IAppMemConnection appMemRedisConnection)
        {
            _logger = logger;
            _appMSSQLConnection = services.FirstOrDefault(i => i.GetType() == typeof(AppMSSQLConnection));
            _appMongoConnection = services.FirstOrDefault(i => i.GetType() == typeof(AppMongoConnection));
            _appMySqlConnection = services.FirstOrDefault(i => i.GetType() == typeof(AppMySqlConnection));
            _appMemRedisConnection = appMemRedisConnection;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger?.LogDebug("WeatherForecastController - Get");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("TestMSSQL")]
        public AppResponseBase<string> TestMsSql()
        {
            var tested = _appMSSQLConnection!.Test();
            return new AppResponseBase<string>
            {
                Message = _appMSSQLConnection.Configuration.ToString()!,
                Status = tested ? 1 : -1,
                data = string.Empty
            };
        }

        [HttpGet("TestMongo")]
        public AppResponseBase<string> TestMongo()
        {
            var tested = _appMongoConnection!.Test();
            return new AppResponseBase<string>
            {
                Message = _appMongoConnection.Configuration.ToString()!,
                Status = tested ? 1 : -1,
                data = string.Empty
            };
        }

        [HttpGet("TestMySql")]
        public AppResponseBase<string> TestMySql()
        {
            var tested = _appMySqlConnection!.Test();
            
            return new AppResponseBase<string>
            {
                Message = _appMySqlConnection.Configuration.ToString()!,
                Status = tested ? 1 : -1,
                data = string.Empty
            };
        }

        [HttpGet("TestRedis")]
        public AppResponseBase<string> TestRedis()
        {
            var tested = _appMemRedisConnection!.Test();

            return new AppResponseBase<string>
            {
                Message = _appMemRedisConnection.Configuration.ToString()!,
                Status = tested ? 1 : -1,
                data = string.Empty
            };
        }
    }

    public class AppResponseBase<T>
    {
        public string Message { get; set; } = string.Empty;
        public int Status { get; set; }
        public T? data { get; set; }
    }
}
