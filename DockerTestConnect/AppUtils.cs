using System.Text.Json;
using Serilog;
namespace DockerTestConnect
{
    internal class AppUtils
    {
        public static void LogInfo(Serilog.ILogger logger, string message)
        {
            logger?.Information(message);
        }

        public static void LogInfo(Serilog.ILogger logger, string message, Exception ex)
        {
            var error = GetExceptionMessage(ex);
            if (string.IsNullOrEmpty(error))
                logger?.Information($"{message}");
            else logger?.Information($"{message} - Error message: {error}");
        }

        public static Serilog.ILogger CreateLog(string fileName = "logs/AppInit_.txt", bool hasConsole = true)
        {
            var tmpInstance = new LoggerConfiguration().MinimumLevel.Information();
            if (hasConsole)
            {
                tmpInstance = tmpInstance.WriteTo.Console();
            }
            tmpInstance = tmpInstance
                .WriteTo.File(
                path: fileName,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 60,
                fileSizeLimitBytes: 10485760,
                rollingInterval: RollingInterval.Day);

            return tmpInstance.CreateLogger();
        }

        //public static void ShowSystemConfig(Serilog.ILogger logger)
        //{
        //    try
        //    {
        //        // Log App Thread Setting
        //        var appConfig = GetLogConfig<AppJobConfig>("");
        //        ThreadPool.GetMaxThreads(out int MaxThread, out int MaxThreadIO);
        //        ThreadPool.GetMinThreads(out int MinThread, out int MinThreadIO);
        //        LogInfo(logger, "System Thread: " + GetVisiableJsonString(new
        //        {
        //            MaxThread,
        //            MaxThreadIO,
        //            MinThread,
        //            MinThreadIO,
        //        }, true));

        //        // Log AppSystemConfig
        //        if (appConfig.AppSystem == null)
        //            LogInfo(logger, "Can not get \"AppSystem\" in appsetting");
        //        else
        //        {
        //            LogInfo(logger, "Inteval(Minisecond): " + appConfig.AppSystem.IntevalMinisecond);
        //            //LogInfo(logger, "JobTimesInDay: " + GetVisiableJsonString(appConfig.AppSystem.JobTimesInDay, true));
        //            LogInfo(logger, "Job Mode: " + appConfig.AppSystem.JobMode);
        //            LogInfo(logger, "Job clear: " + appConfig.AppSystem.JobTimeClearExecuteFlag);
        //            LogInfo(logger, "Job Validate At RMS: " + appConfig.AppSystem.ValidateAtRMS);
        //            LogInfo(logger, "Job Queue Type (1: RMQ, 2: RedisStream): " + appConfig.AppSystem.GatewayQueueType);
        //            LogInfo(logger, "Job Time get From (1: Redis, 0: Configuration file): " + appConfig.AppSystem.JobTimeFromWhat);
        //            LogInfo(logger, "FixAgentID: " + appConfig.AppSystem.FixAgentID);
        //            LogInfo(logger, "ValidateAtRMS: " + appConfig.AppSystem.ValidateAtRMS);
        //            LogInfo(logger, "CheckHoliday: " + appConfig.AppSystem.CheckHoliday);
        //        }

        //        if (appConfig?.GrpcServer == null)
        //            LogInfo(logger, "Can not get \"GrpcServer\" in appsetting");
        //        else
        //            LogInfo(logger, "GrpcServer: " + GetVisiableJsonString(appConfig.GrpcServer, true));

        //        if (appConfig?.RedisServer == null)
        //            LogInfo(logger, "Can not get \"RedisServer\" in appsetting");
        //        else
        //            LogInfo(logger, "RedisServer: " + GetVisiableJsonString(appConfig.RedisServer, true));

        //        if (appConfig?.AppSystem?.GatewayQueueType == 1)
        //        {
        //            var rabbitMQ = GetLogConfig<object>("RMQ:Conn", true);
        //            if (rabbitMQ == null)
        //                LogInfo(logger, "Can not get \"RMQ\" in appsetting");
        //            else
        //                LogInfo(logger, "RabbitMQ: " + GetVisiableJsonString(new { Host = GetLogConfig<string>("RMQ:Conn:Host"), Port = GetLogConfig<string>("RMQ:Conn:Port") }, true));
        //        }
        //    }
        //    catch (Exception) { }

        //}

        public static T GetLogConfig<T>(string key, bool IsSession = false)
        {
            T? logLevel = default;
            var appFile = "appsettings.json";
#if DEBUG
            appFile = "appsettings.Development.json";
#endif
            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile(appFile, optional: false)
                    .Build();
                if (string.IsNullOrEmpty(key))
                    logLevel = config.Get<T>() ?? default;
                else if (IsSession)
                    logLevel = config.GetSection(key).Get<T>();
                else
                    logLevel = config.GetValue<T>(key);
            }
            catch (Exception) { }
            return logLevel ?? default;
        }

        public static string GetExceptionMessage(Exception ex)
        {
            if (ex == null) return string.Empty;
            var innerException = ex;
            if (innerException.InnerException != null) innerException = innerException.InnerException;
            if (innerException.InnerException != null) innerException = innerException.InnerException;
            if (innerException.InnerException != null) innerException = innerException.InnerException;
            if (ex == innerException) return $"{ex.Message} {ex.StackTrace}";
            return $"{ex.Message} \n {innerException.Message} {innerException.Message}";
        }
        public static string GetVisiableJsonString<T>(T data, bool PrettyFormat = false)
        {
            if (data == null) return "NULL";
            if (PrettyFormat)
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            else
                return JsonSerializer.Serialize(data);
        }
        public static DateTime GetDateTime(TimeOnly time)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        public static TimeOnly GetTimeOnly(DateTime time)
        {
            return new TimeOnly(time.Hour, time.Minute, time.Second, time.Millisecond);
        }
    }
}
