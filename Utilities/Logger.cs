using Serilog;
using Serilog.Events;

namespace AtomicCRM.Utilities
{
    public class Logger
    {
        private readonly ILogger _logger;

        public Logger()
        {
            string logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            
            // Ensure logs directory exists
            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }

            string logPath = Path.Combine(logsDirectory, $"TestLog_{DateTime.Now:yyyyMMdd}.txt");
            
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File(logPath, 
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();
        }

        public void Info(string message) => _logger.Information(message);
        
        public void Warning(string message) => _logger.Warning(message);
        
        public void Error(string message) => _logger.Error(message);
    }
}
