using Reqnroll;
using NUnit.Framework;
using Microsoft.Playwright;
using AtomicCRM.Configuration;
using AtomicCRM.Drivers;
using AtomicCRM.Utilities;
using AventStack.ExtentReports;

namespace AtomicCRM.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IReqnrollOutputHelper _outputHelper;
        private Driver? _driver;
        private AppConfigReader? _appConfigReader;
        private TestDataReader? _testDataReader;
        private PdfReportHelper? _pdfReportHelper;
        private ScreenshotHelper? _screenshotHelper;
        private readonly Logger _logger;
        private IPage? _page;
        private static bool _isReportInitialized = false;
        private static PdfReportHelper? _staticReportHelper;

        public Hooks(ScenarioContext scenarioContext, IReqnrollOutputHelper outputHelper)
        {
            _scenarioContext = scenarioContext;
            _outputHelper = outputHelper;
            _logger = new Logger();
        }

        [BeforeTestRun(Order = 0)]
        public static void BeforeTestRun()
        {
            try
            {
                Console.WriteLine("=== BeforeTestRun Hook Started ===");
                
                if (!_isReportInitialized)
                {
                    var appConfig = new AppConfigReader();
                    string reportPath = appConfig.GetPdfReportPath();
                    
                    Console.WriteLine($"Initializing PDF Report at: {reportPath}");
                    
                    _staticReportHelper = new PdfReportHelper(reportPath);
                    _staticReportHelper.InitializeReport("AtomicCRM_TestReport");
                    _isReportInitialized = true;
                    
                    Console.WriteLine("✅ PDF Report Helper initialized successfully");
                }
                
                Console.WriteLine("=== BeforeTestRun Hook Completed ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in BeforeTestRun: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            try
            {
                // Initialize report if not already done
                if (!_isReportInitialized)
                {
                    var appConfig = new AppConfigReader();
                    string reportPath = appConfig.GetPdfReportPath();
                    
                    Console.WriteLine($"=== Initializing PDF Report at: {reportPath} ===");
                    
                    _staticReportHelper = new PdfReportHelper(reportPath);
                    _staticReportHelper.InitializeReport("AtomicCRM_TestReport");
                    _isReportInitialized = true;
                    
                    Console.WriteLine("✅ PDF Report Helper initialized");
                }
                
                string featureName = featureContext.FeatureInfo.Title;
                string description = featureContext.FeatureInfo.Description;
                
                Console.WriteLine($"=== BeforeFeature: {featureName} ===");
                
                _staticReportHelper?.CreateFeature(featureName, description);
                
                Console.WriteLine($"✅ Feature created in report: {featureName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in BeforeFeature: {ex.Message}");
            }
        }

        [AfterFeature]
        public static void AfterFeature(FeatureContext featureContext)
        {
            try
            {
                string featureName = featureContext.FeatureInfo.Title;
                Console.WriteLine($"=== AfterFeature: {featureName} ===");
                Console.WriteLine("Generating PDF report...");
                
                // Generate PDF report
                _staticReportHelper?.GeneratePdfReport("AtomicCRM_TestReport");
                
                string? pdfPath = PdfReportHelper.GetLastGeneratedPdfPath();
                
                if (!string.IsNullOrEmpty(pdfPath))
                {
                    Console.WriteLine($"✅ PDF report generated at: {pdfPath}");
                }
                else
                {
                    Console.WriteLine($"⚠️ PDF report generation completed but path not found.");
                }
                
                Console.WriteLine("=== AfterFeature Completed ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in AfterFeature: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        [BeforeScenario]
        public async Task BeforeScenario(ScenarioContext scenarioContext)
        {
            try
            {
                _logger?.Info($"Starting scenario: {scenarioContext.ScenarioInfo.Title}");
                
                // Initialize configuration readers
                _appConfigReader = new AppConfigReader();
                _scenarioContext.Set(_appConfigReader, "AppConfigReader");
                
                _testDataReader = new TestDataReader(_appConfigReader);
                _scenarioContext.Set(_testDataReader, "TestDataReader");

                // Initialize Playwright driver
                _driver = new Driver(_scenarioContext);
                string browserName = _appConfigReader.GetBrowser();
                _page = await _driver.InitializeAsync(browserName, _outputHelper);
                
                // Initialize helpers
                _screenshotHelper = new ScreenshotHelper(_page, _appConfigReader.GetScreenshotPath());
                _scenarioContext.Set(_screenshotHelper, "ScreenshotHelper");
                
                _pdfReportHelper = _staticReportHelper;
                _scenarioContext.Set(_pdfReportHelper, "PdfReportHelper");
                
                _scenarioContext.Set(_driver, "Driver");
                
                // Create scenario in report
                _pdfReportHelper?.CreateScenario(
                    scenarioContext.ScenarioInfo.Title, 
                    string.Join(", ", scenarioContext.ScenarioInfo.Tags));

                // Navigate to base URL
                string baseUrl = _appConfigReader?.GetBaseUrl() ?? "https://marmelab.com/atomic-crm-demo/";
                await _driver.NavigateToUrlAsync(baseUrl);
                
                _logger.Info($"Navigated to: {baseUrl}");
                _pdfReportHelper?.LogInfo($"Application launched: {baseUrl}");

                // Capture initial screenshot if enabled
                if (_appConfigReader.IsScenarioScreenshotEnabled() && _screenshotHelper != null)
                {
                    string screenshotPath = await _screenshotHelper.CaptureScreenshotAsync("ScenarioStart");
                    _pdfReportHelper?.LogInfo($"Initial screenshot captured", screenshotPath);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in BeforeScenario: {ex.Message}");
                _pdfReportHelper?.LogFail($"Scenario initialization failed: {ex.Message}");
                throw;
            }
        }

        [AfterStep]
        public async Task AfterStep(ScenarioContext scenarioContext)
        {
            try
            {
                var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
                var stepText = scenarioContext.StepContext.StepInfo.Text;
                
                if (scenarioContext.TestError == null)
                {
                    _logger.Info($"PASS - {stepType} {stepText}");
                    
                    if (_appConfigReader?.IsSuccessStepsScreenshotEnabled() == true)
                    {
                        string screenshotPath = await _screenshotHelper!.CaptureScreenshotAsync($"Step_{stepType}");
                        _pdfReportHelper?.LogPass($"{stepType} {stepText}", screenshotPath);
                    }
                    else
                    {
                        _pdfReportHelper?.LogPass($"{stepType} {stepText}");
                    }
                }
                else
                {
                    _logger.Error($"FAIL - {stepType} {stepText}: {scenarioContext.TestError.Message}");
                    
                    string screenshotPath = await _screenshotHelper!.CaptureScreenshotAsync($"Step_Failed_{stepType}");
                    _pdfReportHelper?.LogFail(
                        $"{stepType} {stepText} - Error: {scenarioContext.TestError.Message}", 
                        screenshotPath);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in AfterStep: {ex.Message}");
            }
        }

        [AfterScenario]
        public async Task AfterScenario(ScenarioContext scenarioContext)
        {
            try
            {
                _logger.Info($"Finishing scenario: {scenarioContext.ScenarioInfo.Title}");
                
                // Capture final screenshot
                if (_appConfigReader?.IsScenarioScreenshotEnabled() == true && _screenshotHelper != null)
                {
                    string screenshotPath = await _screenshotHelper.CaptureScreenshotAsync("ScenarioEnd");
                    _pdfReportHelper?.LogInfo($"Final screenshot captured", screenshotPath);
                }

                // Close browser
                if (_driver != null)
                {
                    string scenarioName = scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
                    await _driver.TearDownAsync(scenarioName);
                }

                // Complete scenario and log result
                bool scenarioPassed = scenarioContext.TestError == null;
                
                if (!scenarioPassed)
                {
                    _logger.Error($"Scenario failed: {scenarioContext.TestError!.Message}");
                    _pdfReportHelper?.LogFail($"Scenario failed with error: {scenarioContext.TestError.Message}");
                }
                else
                {
                    _logger.Info($"Scenario passed successfully");
                    _pdfReportHelper?.LogPass($"Scenario completed successfully");
                }
                
                // Mark scenario as complete
                _pdfReportHelper?.CompleteScenario(scenarioPassed);
                
                // Generate PDF report after scenario completion
                Console.WriteLine("=== Generating PDF Report ===");
                _staticReportHelper?.GeneratePdfReport("AtomicCRM_TestReport");
                
                string? pdfPath = PdfReportHelper.GetLastGeneratedPdfPath();
                
                if (!string.IsNullOrEmpty(pdfPath))
                {
                    Console.WriteLine($"✅ PDF report generated at: {pdfPath}");
                    _logger.Info($"PDF report generated at: {pdfPath}");
                }
                else
                {
                    Console.WriteLine($"⚠️ PDF report generation completed but path not found.");
                    _logger.Warning($"PDF report path not found");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in AfterScenario: {ex.Message}");
                Console.WriteLine($"❌ Error in AfterScenario: {ex.Message}");
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            try
            {
                Console.WriteLine("=== AfterTestRun Hook Started ===");
                
                // Generate PDF report directly using DinkToPdf
                _staticReportHelper?.GeneratePdfReport("AtomicCRM_TestReport");
                
                string? pdfPath = PdfReportHelper.GetLastGeneratedPdfPath();
                
                if (!string.IsNullOrEmpty(pdfPath))
                {
                    Console.WriteLine($"✅ Test execution completed. PDF report generated at: {pdfPath}");
                }
                else
                {
                    Console.WriteLine($"⚠️ PDF report generation completed but path not found.");
                }
                
                Console.WriteLine("=== AfterTestRun Hook Completed ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in AfterTestRun: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
