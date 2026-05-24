using Microsoft.Playwright;
using AtomicCRM.ObjectRepository;
using AtomicCRM.Utilities;
using AtomicCRM.Configuration;
using FluentAssertions;

namespace AtomicCRM.Tests.PageComponent
{
    public class ContactsPage : AtomicCRMCommonMethods
    {
        private readonly Locator _locator;
        private PdfReportHelper? _reportHelper;
        private ScreenshotHelper? _screenshotHelper;
        private CsvDataReader? _csvDataReader;

        public ContactsPage(IPage page) : base(page)
        {
            _locator = Locator.Instance;
        }

        public void SetHelpers(PdfReportHelper reportHelper, ScreenshotHelper screenshotHelper)
        {
            _reportHelper = reportHelper;
            _screenshotHelper = screenshotHelper;
        }

        public async Task NavigateToContactsAsync()
        {
            await WaitAsync(3000);
            await Page.ClickAsync(_locator.ContactsMenuLink);
            await WaitForNavigationAsync();
            Logger.Info("Navigated to Contacts page");
        }

        public async Task ClickCreateContactAsync()
        {
            await NavigateToPageAsync(_locator.CreateContactButton);
            Logger.Info("Clicked Create Contact button");
        }

        public async Task CreateContactAsync(
            string firstName,
            string lastName,
            string gender,
            string title,
            string companyName,
            string email,
            string phoneNumber1,
            string phoneNumber2,
            string background,
            string linkedInUrl = "",
            bool hasNewsletter = true)
        {
            // Fill initial fields using dynamic FormFillingAsync
            var formData = new Dictionary<string, (string value, FieldType type)>
            {
                { "First name", (firstName, FieldType.TextBox) },
                { "Last name", (lastName, FieldType.TextBox) },
                { gender, ("", FieldType.RadioButton) }, // Gender as radio button label
                { "Title", (title, FieldType.TextBox) }
            };

            await FormFillingAsync(formData);

            // Handle Company dropdown separately using specific XPath
            if (!string.IsNullOrEmpty(companyName))
            {
                await ClickElementAsync(_locator.CompanySelect);
                await WaitAsync(1000);
                await ClickElementAsync(_locator.CompanyDropdown(companyName));
                await WaitAsync(500);
                Logger.Info($"Selected company: {companyName}");
            }

            // Handle Email and Phone fields separately using specific XPath
            await FillTextBoxAsync(_locator.EmailInput, email);
            await WaitAsync(500);
            Logger.Info($"Filled email: {email}");

            await FillTextBoxAsync(_locator.PhoneNumber1Input, phoneNumber1);
            await WaitAsync(500);
            Logger.Info($"Filled phone number: {phoneNumber1}");

            // Handle LinkedIn URL and Background fields separately using specific XPath
            if (!string.IsNullOrEmpty(linkedInUrl))
            {
                await FillTextBoxAsync(_locator.ContactLinkedInInput, linkedInUrl);
                await WaitAsync(500);
                Logger.Info($"Filled LinkedIn URL: {linkedInUrl}");
            }

            await FillTextBoxAsync(_locator.BackgroundInput, background);
            await WaitAsync(500);
            Logger.Info($"Filled background: {background}");

            // Handle Newsletter toggle separately using specific XPath
            await SetToggleSwitchAsync(_locator.NewsletterSwitch, hasNewsletter);
            await WaitAsync(500);
            Logger.Info($"Set newsletter toggle: {hasNewsletter}");

            // Click Save button
            await ClickElementAsync(_locator.ContactSaveButton);
            await WaitAsync(1000);

            Logger.Info($"Contact created: {firstName} {lastName}");
        }

        public async Task SearchContactAsync(string contactName)
        {
            await SearchAsync(_locator.ContactSearchInput, contactName);
            Logger.Info($"Searched for contact: {contactName}");
        }

        public async Task<bool> VerifyContactExistsAsync(string contactName)
        {
            await WaitAsync(2000);
            await SearchContactAsync(contactName);
            return await VerifyElementExistsInListAsync(_locator.ContactListItem, contactName);
        }

        public async Task ClickOnContactAsync(string contactName)
        {
            await SearchContactAsync(contactName);
            
            try
            {
                await ClickElementAsync(_locator.ContactRow(contactName));
                await WaitForNavigationAsync();
                Logger.Info($"Clicked on contact: {contactName}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error clicking contact: {ex.Message}");
                throw new Exception($"Contact not found to click: {contactName}", ex);
            }
        }

        /// <summary>
        /// Complete workflow: Read CSV data, create contact, verify, capture screenshots, and log
        /// </summary>
        public async Task<string> CreateContactFromCSVAsync(int rowIndex, string companyName, PdfReportHelper reportHelper, ScreenshotHelper screenshotHelper)
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
            string firstName = testData.FirstName ?? throw new ArgumentNullException("FirstName cannot be null");
            string lastName = testData.LastName ?? throw new ArgumentNullException("LastName cannot be null");
            string contactName = $"{firstName} {lastName}";

            Logger.Info($"Starting contact creation from CSV row {rowIndex}: {contactName}");
            _reportHelper.LogInfo($"Creating contact from CSV (Row {rowIndex}): {contactName}");

            // Navigate and click create
            await ClickCreateContactAsync();

            // Parse HasNewsletter from CSV (Yes/No to boolean)
            bool hasNewsletter = testData.HasNewsletter?.Equals("Yes", StringComparison.OrdinalIgnoreCase) ?? true;

            // Create contact
            await CreateContactAsync(
                firstName,
                lastName,
                testData.Gender ?? "He/Him",
                testData.Title ?? "",
                companyName,
                testData.Email ?? "",
                testData.ContactPhone ?? "",
                "",  // phone2
                testData.BackgroundInfo ?? "",
                testData.ContactLinkedIn ?? "",
                hasNewsletter);

            Logger.Info($"Contact created from CSV: {contactName} linked to company: {companyName}");

            // Navigate back to contacts list
            await NavigateToContactsAsync();

            // Capture screenshot after creation
            string screenshotPath = await _screenshotHelper.CaptureScreenshotAsync($"ContactCreated_Row{rowIndex}");
            _reportHelper.LogPass($"Contact created successfully: {contactName}", screenshotPath);

            // Verify contact exists
            bool contactExists = await VerifyContactExistsAsync(contactName);
            contactExists.Should().BeTrue($"Contact '{contactName}' should exist in the system");

            // Capture verification screenshot
            string verificationScreenshot = await _screenshotHelper.CaptureScreenshotAsync($"ContactVerified_Row{rowIndex}");
            _reportHelper.LogPass($"Contact verified: {contactName}", verificationScreenshot);

            Logger.Info($"Complete contact workflow finished for: {contactName}");
            return contactName;
        }
    }
}
