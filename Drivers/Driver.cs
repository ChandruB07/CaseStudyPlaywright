using Microsoft.Playwright;
using Reqnroll;
using AtomicCRM.Configuration;
using AtomicCRM.Utilities;

namespace AtomicCRM.Drivers
{
    public class Driver
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;
        private readonly ScenarioContext _scenarioContext;
        private readonly AppConfigReader _appConfigReader;
        private readonly Logger _logger;

        public Driver(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _appConfigReader = _scenarioContext.Get<AppConfigReader>("AppConfigReader");
            _logger = new Logger();
        }

        public async Task<IPage> InitializeAsync(string browserName, IReqnrollOutputHelper outputHelper)
        {
            try
            {
                bool isHeadless = _appConfigReader.GetHeadlessCondition().Equals("Yes", StringComparison.OrdinalIgnoreCase);
                
                _playwright = await Playwright.CreateAsync();
                _scenarioContext.Set(_playwright, "PlaywrightObject");

                var browserOptions = GetBrowserOptions(browserName, isHeadless);

                _browser = await LaunchBrowserAsync(browserName, browserOptions, outputHelper);
                
                // Create browser context with tracing enabled if configured
                string? videoDir = null;
                if (_appConfigReader.IsTraceViewEnabled())
                {
                    videoDir = Path.IsPathRooted(_appConfigReader.GetTracePath())
                        ? _appConfigReader.GetTracePath()
                        : Path.Combine(Directory.GetCurrentDirectory(), _appConfigReader.GetTracePath());
                    
                    // Ensure trace directory exists
                    if (!Directory.Exists(videoDir))
                    {
                        Directory.CreateDirectory(videoDir);
                    }
                }

                var contextOptions = new BrowserNewContextOptions
                {
                    ViewportSize = ViewportSize.NoViewport,
                    RecordVideoDir = videoDir
                };

                _context = await _browser.NewContextAsync(contextOptions);
                
                if (_appConfigReader.IsTraceViewEnabled())
                {
                    await _context.Tracing.StartAsync(new TracingStartOptions
                    {
                        Screenshots = true,
                        Snapshots = true,
                        Sources = true
                    });
                }

                _page = await _context.NewPageAsync();
                _scenarioContext.Set(_page, "Page");
                _scenarioContext.Set(_context, "BrowserContext");

                // Enable network events if configured
                if (_appConfigReader.IsNetworkEventsEnabled())
                {
                    _page.Request += (_, request) => Console.WriteLine($"Request: {request.Url}");
                    _page.Response += (_, response) => Console.WriteLine($"Response: {response.Url} - {response.Status}");
                }

                _logger.Info($"Browser {browserName} launched successfully in {(isHeadless ? "headless" : "headed")} mode");
                outputHelper.WriteLine($"Browser {browserName} launched successfully");

                return _page;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error initializing Playwright: {ex.Message}");
                throw;
            }
        }

        private async Task<IBrowser> LaunchBrowserAsync(string browserName, dynamic options, IReqnrollOutputHelper outputHelper)
        {
            IBrowser browser = browserName.ToLower() switch
            {
                "chrome" => await _playwright!.Chromium.LaunchAsync(new BrowserTypeLaunchOptions 
                { 
                    Channel = "chrome", 
                    Headless = options.Headless,
                    Args = options.Args
                }),
                "chromium" => await _playwright!.Chromium.LaunchAsync(options),
                "firefox" => await _playwright!.Firefox.LaunchAsync(options),
                "webkit" or "safari" => await _playwright!.Webkit.LaunchAsync(options),
                "msedge" => await _playwright!.Chromium.LaunchAsync(new BrowserTypeLaunchOptions 
                { 
                    Channel = "msedge", 
                    Headless = options.Headless,
                    Args = options.Args
                }),
                _ => await _playwright!.Chromium.LaunchAsync(options)
            };

            outputHelper.WriteLine($"PASS: {browserName} browser launched successfully");
            return browser;
        }

        private dynamic GetBrowserOptions(string browserName, bool headless)
        {
            if (browserName.ToLower() == "firefox")
            {
                return new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    Args = new[] { "--disable-dev-shm-usage" }
                };
            }
            else
            {
                return new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    Args = new[] 
                    { 
                        "--start-maximized",
                        "--disable-dev-shm-usage",
                        "--no-sandbox"
                    }
                };
            }
        }

        public async Task NavigateToUrlAsync(string url)
        {
            if (_page == null)
                throw new InvalidOperationException("Page is not initialized");

            await _page.GotoAsync(url, new PageGotoOptions 
            { 
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = _appConfigReader.GetTimeout()
            });
            
            _logger.Info($"Navigated to URL: {url}");
        }

        public async Task TearDownAsync(string scenarioName)
        {
            try
            {
                if (_context != null && _appConfigReader.IsTraceViewEnabled())
                {
                    string tracePath = Path.IsPathRooted(_appConfigReader.GetTracePath())
                        ? _appConfigReader.GetTracePath()
                        : Path.Combine(Directory.GetCurrentDirectory(), _appConfigReader.GetTracePath());
                    
                    // Ensure trace directory exists
                    if (!Directory.Exists(tracePath))
                    {
                        Directory.CreateDirectory(tracePath);
                    }

                    string traceFile = Path.Combine(tracePath, $"{scenarioName}_{DateTime.Now:yyyyMMddHHmmss}.zip");
                    await _context.Tracing.StopAsync(new TracingStopOptions { Path = traceFile });
                    _logger.Info($"Trace saved to: {traceFile}");
                }

                if (_page != null) await _page.CloseAsync();
                if (_context != null) await _context.CloseAsync();
                if (_browser != null) await _browser.CloseAsync();
                _playwright?.Dispose();

                _logger.Info("Browser closed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during teardown: {ex.Message}");
            }
        }

        public IPage? GetPage() => _page;
    }
}
