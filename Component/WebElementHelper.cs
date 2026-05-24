using Microsoft.Playwright;

namespace AtomicCRM.Component
{
    public class WebElementHelper
    {
        private readonly IPage _page;

        public WebElementHelper(IPage page)
        {
            _page = page;
        }

        public async Task<bool> IsElementVisibleAsync(string locator)
        {
            try
            {
                return await _page.Locator(locator).IsVisibleAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsElementHiddenAsync(string locator)
        {
            try
            {
                return await _page.Locator(locator).IsHiddenAsync();
            }
            catch
            {
                return true;
            }
        }

        public async Task<bool> IsElementCheckedAsync(string locator)
        {
            try
            {
                return await _page.Locator(locator).IsCheckedAsync();
            }
            catch
            {
                return false;
            }
        }


        public async Task<int> GetElementCountAsync(string locator)
        {
            try
            {
                return await _page.Locator(locator).CountAsync();
            }
            catch
            {
                return 0;
            }
        }

        public async Task<IReadOnlyList<ILocator>> GetAllElementsAsync(string locator)
        {
            return await _page.Locator(locator).AllAsync();
        }

        public async Task<string?> GetElementAttributeAsync(string locator, string attributeName)
        {
            try
            {
                return await _page.Locator(locator).GetAttributeAsync(attributeName);
            }
            catch
            {
                return null;
            }
        } 
    }
}
