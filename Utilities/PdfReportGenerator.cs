using Microsoft.Playwright;

namespace AtomicCRM.Utilities
{
    /// <summary>
    /// Generates PDF reports from HTML ExtentReports using Playwright
    /// </summary>
    public class PdfReportGenerator
    {
        private readonly Logger _logger;

        public PdfReportGenerator()
        {
            _logger = new Logger();
        }

        /// <summary>
        /// Converts HTML report to PDF using Playwright's PDF generation capability
        /// </summary>
        /// <param name="htmlReportPath">Full path to the HTML report file</param>
        /// <param name="pdfOutputPath">Full path where PDF should be saved</param>
        /// <returns>Path to the generated PDF file</returns>
        public async Task<string> ConvertHtmlToPdfAsync(string htmlReportPath, string pdfOutputPath)
        {
            try
            {
                _logger.Info($"Starting PDF generation from HTML: {htmlReportPath}");

                // Validate input file exists
                if (!File.Exists(htmlReportPath))
                {
                    throw new FileNotFoundException($"HTML report not found at: {htmlReportPath}");
                }

                // Ensure output directory exists
                string? outputDirectory = Path.GetDirectoryName(pdfOutputPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                    _logger.Info($"Created output directory: {outputDirectory}");
                }

                // Initialize Playwright for PDF generation
                var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

                var page = await browser.NewPageAsync();

                // Load HTML file
                await page.GotoAsync($"file:///{htmlReportPath.Replace("\\", "/")}");

                // Wait for content to load
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Generate PDF with custom options
                await page.PdfAsync(new PagePdfOptions
                {
                    Path = pdfOutputPath,
                    Format = "A4",
                    PrintBackground = true,
                    Margin = new Margin
                    {
                        Top = "20px",
                        Right = "20px",
                        Bottom = "20px",
                        Left = "20px"
                    }
                });

                _logger.Info($"PDF report generated successfully: {pdfOutputPath}");

                await browser.CloseAsync();
                playwright.Dispose();

                return pdfOutputPath;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating PDF report: {ex.Message}");
                throw new Exception($"Failed to generate PDF report: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Finds the latest HTML report in the specified directory and converts it to PDF
        /// </summary>
        /// <param name="reportDirectory">Directory containing HTML reports</param>
        /// <param name="pdfOutputDirectory">Directory where PDF should be saved</param>
        /// <returns>Path to the generated PDF file, or empty string if no HTML report found</returns>
        public async Task<string> ConvertLatestHtmlToPdfAsync(string reportDirectory, string pdfOutputDirectory)
        {
            try
            {
                // Find the latest HTML report
                var htmlFiles = Directory.GetFiles(reportDirectory, "*.html")
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .ToArray();

                if (htmlFiles.Length == 0)
                {
                    _logger.Warning($"No HTML reports found in directory: {reportDirectory}");
                    return string.Empty;
                }

                string latestHtmlReport = htmlFiles[0];
                string htmlFileName = Path.GetFileNameWithoutExtension(latestHtmlReport);
                string pdfFileName = $"{htmlFileName}.pdf";
                string pdfOutputPath = Path.Combine(pdfOutputDirectory, pdfFileName);

                _logger.Info($"Converting latest HTML report: {Path.GetFileName(latestHtmlReport)}");

                return await ConvertHtmlToPdfAsync(latestHtmlReport, pdfOutputPath);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error converting latest HTML to PDF: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the most recently created HTML report file path
        /// </summary>
        /// <param name="reportDirectory">Directory to search for HTML reports</param>
        /// <returns>Full path to the latest HTML report, or null if none found</returns>
        public string? GetLatestHtmlReport(string reportDirectory)
        {
            try
            {
                if (!Directory.Exists(reportDirectory))
                {
                    _logger.Warning($"Report directory does not exist: {reportDirectory}");
                    return null;
                }

                var htmlFiles = Directory.GetFiles(reportDirectory, "*.html")
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .ToArray();

                return htmlFiles.Length > 0 ? htmlFiles[0] : null;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error finding latest HTML report: {ex.Message}");
                return null;
            }
        }
    }
}
