using DinkToPdf;
using DinkToPdf.Contracts;
using System.Text;

namespace AtomicCRM.Utilities
{
    /// <summary>
    /// Generates PDF reports directly using DinkToPdf without intermediate HTML files
    /// </summary>
    public class PdfReportHelper
    {
        private readonly string _reportPath;
        private readonly StringBuilder _htmlContent;
        private readonly List<TestResult> _testResults;
        private string? _currentFeature;
        private string? _currentScenario;
        private readonly List<StepResult> _currentSteps;
        private DateTime _testStartTime;
        private readonly Logger _logger;
        private static string? _lastGeneratedPdfPath;

        public PdfReportHelper(string reportPath)
        {
            _reportPath = Path.IsPathRooted(reportPath) ? reportPath : Path.Combine(Directory.GetCurrentDirectory(), reportPath);
            _htmlContent = new StringBuilder();
            _testResults = new List<TestResult>();
            _currentSteps = new List<StepResult>();
            _logger = new Logger();
            _testStartTime = DateTime.Now;
        }

        public void InitializeReport(string reportName)
        {
            try
            {
                Console.WriteLine($"PdfReportHelper.InitializeReport called with reportName: {reportName}");
                
                if (!Directory.Exists(_reportPath))
                {
                    Directory.CreateDirectory(_reportPath);
                    Console.WriteLine($"Created report directory: {_reportPath}");
                }

                _testStartTime = DateTime.Now;
                Console.WriteLine("✅ PDF Report Helper initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error initializing PDF report: {ex.Message}");
                throw;
            }
        }

        public void CreateFeature(string featureName, string description = "")
        {
            _currentFeature = featureName;
            Console.WriteLine($"✅ Feature set: {featureName}");
        }

        public void CreateScenario(string scenarioName, string description = "")
        {
            _currentScenario = scenarioName;
            _currentSteps.Clear();
            Console.WriteLine($"✅ Scenario created: {scenarioName}");
        }

        public void LogInfo(string message, string screenshotPath = "")
        {
            _currentSteps.Add(new StepResult
            {
                StepType = "Info",
                StepText = message,
                Status = "Info",
                ScreenshotPath = screenshotPath,
                Timestamp = DateTime.Now
            });
            _logger.Info(message);
        }

        public void LogPass(string message, string screenshotPath = "")
        {
            _currentSteps.Add(new StepResult
            {
                StepType = "Step",
                StepText = message,
                Status = "Pass",
                ScreenshotPath = screenshotPath,
                Timestamp = DateTime.Now
            });
            _logger.Info($"PASS: {message}");
        }

        public void LogFail(string message, string screenshotPath = "")
        {
            _currentSteps.Add(new StepResult
            {
                StepType = "Step",
                StepText = message,
                Status = "Fail",
                ScreenshotPath = screenshotPath,
                Timestamp = DateTime.Now
            });
            _logger.Error($"FAIL: {message}");
        }

        public void LogWarning(string message)
        {
            _currentSteps.Add(new StepResult
            {
                StepType = "Warning",
                StepText = message,
                Status = "Warning",
                Timestamp = DateTime.Now
            });
            _logger.Warning(message);
        }

        public void CompleteScenario(bool passed)
        {
            if (!string.IsNullOrEmpty(_currentScenario))
            {
                _testResults.Add(new TestResult
                {
                    FeatureName = _currentFeature ?? "Unknown Feature",
                    ScenarioName = _currentScenario,
                    Status = passed ? "Passed" : "Failed",
                    Steps = new List<StepResult>(_currentSteps),
                    StartTime = _testStartTime,
                    EndTime = DateTime.Now
                });
            }
        }

        public void GeneratePdfReport(string reportName)
        {
            try
            {
                Console.WriteLine($"Generating PDF report: {reportName}");

                string pdfFileName = $"{reportName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string pdfFilePath = Path.Combine(_reportPath, pdfFileName);

                // Build HTML content
                string htmlContent = BuildHtmlContent();

                // Convert HTML to PDF using DinkToPdf
                var converter = new SynchronizedConverter(new PdfTools());
                
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Out = pdfFilePath,
                        DocumentTitle = "Atomic CRM Test Report"
                    },
                    Objects = {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = htmlContent,
                            WebSettings = { DefaultEncoding = "utf-8", EnableJavascript = false },
                            HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                            FooterSettings = { FontSize = 8, Center = "Atomic CRM Automation Report", Line = true }
                        }
                    }
                };

                converter.Convert(doc);

                _lastGeneratedPdfPath = pdfFilePath;
                Console.WriteLine($"✅ PDF report generated successfully at: {pdfFilePath}");
                _logger.Info($"PDF report generated: {pdfFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error generating PDF report: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                _logger.Error($"Error generating PDF: {ex.Message}");
                throw;
            }
        }

        private string BuildHtmlContent()
        {
            var html = new StringBuilder();

            // HTML header with CSS
            html.Append(@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Atomic CRM Test Report</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; background-color: #f5f5f5; }
        .header { background-color: #2c3e50; color: white; padding: 20px; margin-bottom: 20px; border-radius: 5px; }
        .header h1 { margin: 0 0 10px 0; }
        .summary { background-color: white; padding: 15px; margin-bottom: 20px; border-radius: 5px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .summary-item { display: inline-block; margin-right: 30px; padding: 10px 20px; background-color: #ecf0f1; border-radius: 3px; }
        .summary-item .label { font-weight: bold; color: #34495e; }
        .summary-item .value { font-size: 24px; font-weight: bold; }
        .passed { color: #27ae60; }
        .failed { color: #e74c3c; }
        .feature { background-color: white; margin-bottom: 20px; padding: 15px; border-radius: 5px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .feature h2 { color: #2c3e50; margin-top: 0; border-bottom: 2px solid #3498db; padding-bottom: 10px; }
        .scenario { margin: 15px 0; padding: 15px; background-color: #f8f9fa; border-left: 4px solid #3498db; }
        .scenario h3 { color: #2c3e50; margin-top: 0; }
        .step { margin: 10px 0; padding: 10px; background-color: white; border-radius: 3px; }
        .step.pass { border-left: 4px solid #27ae60; }
        .step.fail { border-left: 4px solid #e74c3c; }
        .step.info { border-left: 4px solid #3498db; }
        .step-status { font-weight: bold; margin-right: 10px; }
        .status-pass { color: #27ae60; }
        .status-fail { color: #e74c3c; }
        .screenshot { max-width: 100%; height: auto; margin-top: 10px; border: 1px solid #ddd; border-radius: 3px; }
        .timestamp { color: #7f8c8d; font-size: 0.9em; margin-left: 10px; }
    </style>
</head>
<body>
");

            // Header
            html.Append($@"
    <div class='header'>
        <h1>Atomic CRM Automation Test Report</h1>
        <p>Framework: Playwright with C# | Automation Tool: Reqnroll + NUnit</p>
        <p>Execution Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
        <p>Environment: DEMO | Application: Atomic CRM</p>
    </div>
");

            // Summary
            int totalTests = _testResults.Count;
            int passedTests = _testResults.Count(r => r.Status == "Passed");
            int failedTests = _testResults.Count(r => r.Status == "Failed");
            double passRate = totalTests > 0 ? (passedTests * 100.0 / totalTests) : 0;

            html.Append($@"
    <div class='summary'>
        <div class='summary-item'>
            <div class='label'>Total Tests</div>
            <div class='value'>{totalTests}</div>
        </div>
        <div class='summary-item'>
            <div class='label'>Passed</div>
            <div class='value passed'>{passedTests}</div>
        </div>
        <div class='summary-item'>
            <div class='label'>Failed</div>
            <div class='value failed'>{failedTests}</div>
        </div>
        <div class='summary-item'>
            <div class='label'>Pass Rate</div>
            <div class='value'>{passRate:F1}%</div>
        </div>
    </div>
");

            // Group results by feature
            var featureGroups = _testResults.GroupBy(r => r.FeatureName);

            foreach (var featureGroup in featureGroups)
            {
                html.Append($@"
    <div class='feature'>
        <h2>{featureGroup.Key}</h2>
");

                foreach (var result in featureGroup)
                {
                    string statusClass = result.Status == "Passed" ? "passed" : "failed";
                    var duration = (result.EndTime - result.StartTime).TotalSeconds;

                    html.Append($@"
        <div class='scenario'>
            <h3>{result.ScenarioName} <span class='{statusClass}'>({result.Status})</span></h3>
            <p>Duration: {duration:F2} seconds</p>
");

                    foreach (var step in result.Steps)
                    {
                        string stepStatusClass = step.Status.ToLower();
                        string statusLabel = step.Status;

                        html.Append($@"
            <div class='step {stepStatusClass}'>
                <span class='step-status status-{stepStatusClass}'>[{statusLabel}]</span>
                {step.StepText}
                <span class='timestamp'>{step.Timestamp:HH:mm:ss}</span>
");

                        // Add screenshot if available
                        if (!string.IsNullOrEmpty(step.ScreenshotPath) && File.Exists(step.ScreenshotPath))
                        {
                            try
                            {
                                byte[] imageBytes = File.ReadAllBytes(step.ScreenshotPath);
                                string base64Image = Convert.ToBase64String(imageBytes);
                                string imageFormat = Path.GetExtension(step.ScreenshotPath).ToLower() == ".png" ? "png" : "jpeg";
                                
                                html.Append($@"
                <br><img class='screenshot' src='data:image/{imageFormat};base64,{base64Image}' alt='Screenshot' />
");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Warning: Could not embed screenshot {step.ScreenshotPath}: {ex.Message}");
                            }
                        }

                        html.Append("            </div>\n");
                    }

                    html.Append("        </div>\n");
                }

                html.Append("    </div>\n");
            }

            html.Append(@"
</body>
</html>");

            return html.ToString();
        }

        public string GetReportPath()
        {
            return _reportPath;
        }

        public static string? GetLastGeneratedPdfPath()
        {
            return _lastGeneratedPdfPath;
        }

        private class TestResult
        {
            public string FeatureName { get; set; } = string.Empty;
            public string ScenarioName { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public List<StepResult> Steps { get; set; } = new();
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        private class StepResult
        {
            public string StepType { get; set; } = string.Empty;
            public string StepText { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string ScreenshotPath { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }
    }
}
