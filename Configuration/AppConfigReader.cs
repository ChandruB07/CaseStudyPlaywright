using Microsoft.Extensions.Configuration;

namespace AtomicCRM.Configuration
{
    public class AppConfigReader
    {
        private readonly IConfiguration _configuration;

        public AppConfigReader()
        {
            try
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                var configPath = Path.Combine(currentDirectory, "Configuration", "ConfigSettings.json");
                
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException($"Configuration file not found at: {configPath}");
                }

                var builder = new ConfigurationBuilder()
                    .SetBasePath(currentDirectory)
                    .AddJsonFile("Configuration\\ConfigSettings.json", optional: false, reloadOnChange: true);
                _configuration = builder.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
                throw;
            }
        }

        public string GetBrowser() => _configuration["Browser"] ?? "Chrome";
        
        public string GetEnvironment() => _configuration["Environment"] ?? "DEMO";
        
        public string GetHeadlessCondition() => _configuration["HeadlessCondition"] ?? "No";
        
        public int GetPageLoadTimeout() => int.Parse(_configuration["PageLoadTimeout"] ?? "60");
        
        public int GetElementLoadTimeout() => int.Parse(_configuration["ElementLoadTimeout"] ?? "60");
        
        public int GetTimeout() => int.Parse(_configuration["Timeout"] ?? "120000");
        
        public bool IsScenarioScreenshotEnabled() => bool.Parse(_configuration["isScenarioScreenshotEnabled"] ?? "true");
        
        public bool IsSuccessStepsScreenshotEnabled() => bool.Parse(_configuration["SuccessStepsScreenshotEnabled"] ?? "true");
        
        public bool IsTraceViewEnabled() => bool.Parse(_configuration["isTraceViewEnabled"] ?? "true");
        
        public bool IsNetworkEventsEnabled() => bool.Parse(_configuration["isNetworkEventsEnabled"] ?? "false");
        
        public string GetBaseUrl() => _configuration["BaseUrl"] ?? "https://marmelab.com/atomic-crm-demo/";
        
        public string GetProjectName() => _configuration["ProjectName"] ?? "Atomic CRM Automation";
        
        public string GetBuildName() => _configuration["BuildName"] ?? "Atomic CRM Case Study Execution";
        
        public string GetPdfReportPath() => _configuration["PdfReportPath"] ?? "Reports";
        
        public string GetScreenshotPath() => _configuration["ScreenshotPath"] ?? "Screenshots";
        
        public string GetTracePath() => _configuration["TracePath"] ?? "Traces";
        
        public string GetTestDataPath()
        {
            string environment = GetEnvironment();
            return _configuration[environment] ?? "TestData\\TestDataDemo.json";
        }
    }
}
