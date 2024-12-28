using Connections.Mongo;
using Connections.MsSql;
using Connections.MySql;
using Connections.Redis;
using DockerTestConnect;
using IConnections;
using Serilog;

Serilog.ILogger? appStartloger = null;

try
{
    Directory.SetCurrentDirectory(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) ?? AppDomain.CurrentDomain.BaseDirectory);

    appStartloger = AppUtils.CreateLog();
    AppUtils.LogInfo(appStartloger, "Program Starting");

    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    // Add services to the container.
    builder.Services.AddScoped<IAppDBConnection, AppMSSQLConnection>();
    builder.Services.AddScoped<IAppDBConnection, AppMongoConnection>();
    builder.Services.AddScoped<IAppDBConnection, AppMySqlConnection>();
    builder.Services.AddScoped<IAppMemConnection, AppMemRedisConnection>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseAuthorization();
    app.MapControllers();
    AppUtils.LogInfo(appStartloger, "Program inialized");

    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

    app.Run();
    AppUtils.LogInfo(appStartloger, "Program Stop");
}
catch (Exception ex)
{
    AppUtils.LogInfo(appStartloger!, "Program error", ex);
}
