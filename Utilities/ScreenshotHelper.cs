using Microsoft.Playwright;

namespace AtomicCRM.Utilities
{
    public class ScreenshotHelper
    {
        private readonly IPage _page;
        private readonly string _screenshotPath;
        private readonly Logger _logger;

        public ScreenshotHelper(IPage page, string screenshotPath)
        {
            _page = page;
            // Ensure the path is absolute
            _screenshotPath = Path.IsPathRooted(screenshotPath) 
                ? screenshotPath 
                : Path.Combine(Directory.GetCurrentDirectory(), screenshotPath);
            _logger = new Logger();

            // Ensure screenshot directory exists
            if (!Directory.Exists(_screenshotPath))
            {
                Directory.CreateDirectory(_screenshotPath);
            }
        }

        public async Task<string> CaptureScreenshotAsync(string screenshotName)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"{screenshotName}_{timestamp}.png";
                string fullPath = Path.Combine(_screenshotPath, fileName);

                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = fullPath,
                    FullPage = true
                });

                _logger.Info($"Screenshot captured: {fullPath}");
                return fullPath;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error capturing screenshot: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
