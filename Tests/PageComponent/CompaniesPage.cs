using Microsoft.Playwright;
using AtomicCRM.ObjectRepository;
using AtomicCRM.Utilities;
using AtomicCRM.Configuration;
using FluentAssertions;

namespace AtomicCRM.Tests.PageComponent
{
    public class CompaniesPage : AtomicCRMCommonMethods
    {
        private readonly Locator _locator;
        private PdfReportHelper? _reportHelper;
        private ScreenshotHelper? _screenshotHelper;
        private CsvDataReader? _csvDataReader;

        public CompaniesPage(IPage page) : base(page)
        {
            _locator = Locator.Instance;
        }

        public void SetHelpers(PdfReportHelper reportHelper, ScreenshotHelper screenshotHelper)
        {
            _reportHelper = reportHelper;
            _screenshotHelper = screenshotHelper;
        }

        public async Task NavigateToCompaniesAsync()
        {
            await NavigateToPageAsync(_locator.CompaniesMenuLink);
            Logger.Info("Navigated to Companies page");
        }

        public async Task ClickCreateCompanyAsync()
        {
            await NavigateToPageAsync(_locator.CreateCompanyButton);
            Logger.Info("Clicked Create Company button");
        }

        public async Task CreateCompanyAsync(
            string name,
            string sector,
            string size,
            string linkedIn,
            string address,
            string city,
            string state,
            string zipCode,
            string phoneNumber,
            string website,
            string revenue = "",
            string taxIdentifier = "",
            string country = "",
            string description = "",
            string accountManager = "")
        {
            // Using dynamic FormFillingAsync method with field labels
            var formData = new Dictionary<string, (string value, FieldType type)>
            {
                { "Company name", (name, FieldType.TextBox) },
                { "Website", (website, FieldType.TextBox) },
                { "LinkedIn URL", (linkedIn, FieldType.TextBox) },
                { "Phone number", (phoneNumber, FieldType.TextBox) },
                { "Address", (address, FieldType.TextBox) },
                { "City", (city, FieldType.TextBox) },
                { "Zip code", (zipCode, FieldType.TextBox) },
                { "State", (state, FieldType.TextBox) },
                { "Country", (country, FieldType.TextBox) },
                { "Sector", (sector, FieldType.DropDown) },
                { "Size", (size, FieldType.DropDown) },
                { "Revenue", (revenue, FieldType.TextBox) },
                { "Tax Identifier", (taxIdentifier, FieldType.TextBox) },
                { "Description", (description, FieldType.TextArea) },
                { "Account manager", (accountManager, FieldType.DropDown) },
                { "Create Company", ("", FieldType.Button) }
            };

            await FormFillingAsync(formData);

            Logger.Info($"Company created: {name}");
        }

        public async Task SearchCompanyAsync(string companyName)
        {
            await SearchAsync(_locator.CompanySearchInput, companyName);
            Logger.Info($"Searched for company: {companyName}");
        }

        public async Task<bool> VerifyCompanyExistsAsync(string companyName)
        {
            await SearchCompanyAsync(companyName);
            await WaitAsync(1500); // Wait for search filter to apply
            
            try
            {
                // Try to find the company card by title
                var count = await GetElementCountAsync(_locator.CompanyCardTitle(companyName));
                
                if (count > 0)
                {
                    Logger.Info($"Company verified: {companyName}");
                    return true;
                }
                
                // Fallback: Check all cards if specific title not found
                return await VerifyElementExistsInListAsync(_locator.CompanyListItem, companyName);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error verifying company: {ex.Message}");
            }
            
            Logger.Warning($"Company not found: {companyName}");
            return false;
        }

        public async Task ClickOnCompanyAsync(string companyName)
        {
            await SearchCompanyAsync(companyName);
            await WaitAsync(1500); // Wait for search filter to apply
            
            try
            {
                // Try to click the company card by title
                var count = await GetElementCountAsync(_locator.CompanyCardTitle(companyName));
                
                if (count > 0)
                {
                    await ClickElementAsync(_locator.CompanyCardTitle(companyName));
                    await WaitForNavigationAsync();
                    Logger.Info($"Clicked on company: {companyName}");
                    return;
                }
                
                // Fallback: Use the company card locator
                await ClickElementAsync(_locator.CompanyCard(companyName));
                await WaitForNavigationAsync();
                Logger.Info($"Clicked on company (fallback): {companyName}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error clicking company: {ex.Message}");
                throw new Exception($"Company not found to click: {companyName}", ex);
            }
        }

        /// <summary>
        /// Complete workflow: Read CSV data, create company, verify, capture screenshots, and log
        /// </summary>
        public async Task<string> CreateCompanyFromCSVAsync(int rowIndex, PdfReportHelper reportHelper, ScreenshotHelper screenshotHelper)
        {
            _reportHelper = reportHelper;
            _screenshotHelper = screenshotHelper;

            // Initialize CSV reader
            if (_csvDataReader == null)
            {
                string csvPath = Path.Combine(Directory.GetCurrentDirectory(), "Configuration", "TestData", "TestData.csv");
                _csvDataReader = new CsvDataReader(csvPath);
            }

            // Read test data from CSV
            var testData = _csvDataReader.GetTestDataByRow(rowIndex);
            string companyName = testData.CompanyName ?? throw new ArgumentNullException("CompanyName cannot be null");

            Logger.Info($"Starting company creation from CSV row {rowIndex}: {companyName}");
            _reportHelper.LogInfo($"Creating company from CSV (Row {rowIndex}): {companyName}");

            // Navigate and click create
            await ClickCreateCompanyAsync();

            // Create company
            await CreateCompanyAsync(
                testData.CompanyName!,
                testData.Sector ?? "Information Technology",
                testData.Size ?? "51-250",
                testData.LinkedInURL ?? "",
                testData.Address ?? "",
                testData.City ?? "",
                testData.State ?? "",
                testData.ZipCode ?? "",
                testData.PhoneNumber ?? "",
                testData.Website ?? "",
                testData.Revenue ?? "",
                testData.TaxIdentifier ?? "",
                testData.Country ?? "",
                testData.Description ?? "",
                testData.AccountManager ?? "");

            Logger.Info($"Company created from CSV: {companyName}");

            // Navigate back to companies list
            await NavigateToCompaniesAsync();
            
            // Capture screenshot after creation
            string screenshotPath = await _screenshotHelper.CaptureScreenshotAsync($"CompanyCreated_Row{rowIndex}");
            _reportHelper.LogPass($"Company created successfully: {companyName}", screenshotPath);

            // Verify company exists
            bool companyExists = await VerifyCompanyExistsAsync(companyName);
            companyExists.Should().BeTrue($"Company '{companyName}' should exist in the system");

            // Capture verification screenshot
            string verificationScreenshot = await _screenshotHelper.CaptureScreenshotAsync($"CompanyVerified_Row{rowIndex}");
            _reportHelper.LogPass($"Company verified: {companyName}", verificationScreenshot);

            Logger.Info($"Complete company workflow finished for: {companyName}");
            return companyName;
        }
    }
}
