using Microsoft.Playwright;

namespace AtomicCRM.Component
{
    public class WaitHelper
    {
        private readonly IPage _page;

        public WaitHelper(IPage page)
        {
            _page = page;
        }

        public async Task WaitForElementAsync(string locator, int timeoutMs = 30000)
        {
            await _page.WaitForSelectorAsync(locator, new PageWaitForSelectorOptions { Timeout = timeoutMs });
        }

        public async Task WaitForElementVisibleAsync(string locator, int timeoutMs = 30000)
        {
            await _page.WaitForSelectorAsync(locator, new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Visible, 
                Timeout = timeoutMs 
            });
        }

        public async Task WaitForElementHiddenAsync(string locator, int timeoutMs = 30000)
        {
            await _page.WaitForSelectorAsync(locator, new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Hidden, 
                Timeout = timeoutMs 
            });
        }

        public async Task WaitForElementAttachedAsync(string locator, int timeoutMs = 30000)
        {
            await _page.WaitForSelectorAsync(locator, new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Attached, 
                Timeout = timeoutMs 
            });
        }

        public async Task WaitForNavigationAsync(int timeoutMs = 30000)
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = timeoutMs });
        }

        public async Task WaitForTimeoutAsync(int milliseconds)
        {
            await _page.WaitForTimeoutAsync(milliseconds);
        }

        public async Task WaitForConditionAsync(Func<Task<bool>> condition, int timeoutMs = 30000, int pollIntervalMs = 500)
        {
            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                if (await condition())
                    return;
                
                await _page.WaitForTimeoutAsync(pollIntervalMs);
            }
            
            throw new TimeoutException($"Condition not met within {timeoutMs}ms");
        }

        
    }
}
