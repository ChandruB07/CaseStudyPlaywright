using Microsoft.Playwright;

namespace AtomicCRM.Component
{
    public class WebActionHelper
    {
        private readonly IPage _page;

        public WebActionHelper(IPage page)
        {
            _page = page;
        }

        public async Task ClickAsync(string locator)
        {
            await _page.ClickAsync(locator);
        }

        public async Task ClickAsync(ILocator locator)
        {
            await locator.ClickAsync();
        }

        public async Task FillAsync(string locator, string value)
        {
            await _page.FillAsync(locator, value);
        }

        public async Task FillAsync(ILocator locator, string value)
        {
            await locator.FillAsync(value);
        }

        public async Task SelectOptionAsync(string locator, string value)
        {
            await _page.SelectOptionAsync(locator, value);
        }

        public async Task CheckAsync(string locator)
        {
            await _page.CheckAsync(locator);
        }

        public async Task UncheckAsync(string locator)
        {
            await _page.UncheckAsync(locator);
        }

        public async Task HoverAsync(string locator)
        {
            await _page.HoverAsync(locator);
        }

        public async Task DoubleClickAsync(string locator)
        {
            await _page.DblClickAsync(locator);
        }

        public async Task RightClickAsync(string locator)
        {
            await _page.ClickAsync(locator, new PageClickOptions { Button = MouseButton.Right });
        }

        public async Task TypeAsync(string locator, string text)
        {
            await _page.Locator(locator).PressSequentiallyAsync(text);
        }

        public async Task ClearAsync(string locator)
        {
            await _page.Locator(locator).ClearAsync();
        }

        public async Task UploadFileAsync(string locator, string filePath)
        {
            await _page.SetInputFilesAsync(locator, filePath);
        }

        public async Task<string?> GetTextAsync(string locator)
        {
            return await _page.Locator(locator).TextContentAsync();
        }
    }
}
