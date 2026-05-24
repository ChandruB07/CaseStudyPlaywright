using Reqnroll;
using NUnit.Framework;
using Microsoft.Playwright;
using AtomicCRM.Tests.PageComponent;
using AtomicCRM.Configuration;
using AtomicCRM.Utilities;
using FluentAssertions;

namespace AtomicCRM.Tests.StepDefinitions
{
    [Binding]
    public class AtomicCRMWorkflowSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IPage _page;
        private readonly AppConfigReader _appConfigReader;
        private readonly TestDataReader _testDataReader;
        private readonly PdfReportHelper _reportHelper;
        private readonly ScreenshotHelper _screenshotHelper;
        private readonly Logger _logger;
        
        private CompaniesPage? _companiesPage;
        private ContactsPage? _contactsPage;
        private CsvDataReader? _csvDataReader;
        
        private string _createdCompanyName = string.Empty;
        private string _createdContactName = string.Empty;
        private readonly List<string> _createdCompanies = new();

        public AtomicCRMWorkflowSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _page = _scenarioContext.Get<IPage>("Page");
            _appConfigReader = _scenarioContext.Get<AppConfigReader>("AppConfigReader");
            _testDataReader = _scenarioContext.Get<TestDataReader>("TestDataReader");
            _reportHelper = _scenarioContext.Get<PdfReportHelper>("PdfReportHelper");
            _screenshotHelper = _scenarioContext.Get<ScreenshotHelper>("ScreenshotHelper");
            _logger = new Logger();
        }

        [Given(@"I am on the Atomic CRM application")]
        public void GivenIAmOnTheAtomicCRMApplication()
        {
            string currentUrl = _page.Url;
            _logger.Info($"Current URL: {currentUrl}");
            
            currentUrl.Should().Contain("marmelab.com/atomic-crm-demo", 
                "User should be on the Atomic CRM application");
            
            _reportHelper.LogInfo($"Successfully loaded Atomic CRM application: {currentUrl}");
        }

        [When(@"I navigate to the Companies page")]
        public async Task WhenINavigateToTheCompaniesPage()
        {
            _companiesPage = new CompaniesPage(_page);
            await _companiesPage.NavigateToCompaniesAsync();
            
            _reportHelper.LogInfo("Navigated to Companies page");
        }

        [Then(@"the company should be created successfully")]
        public async Task ThenTheCompanyShouldBeCreatedSuccessfully()
        {
            _companiesPage ??= new CompaniesPage(_page);
            
            // Navigate back to companies list
            await _companiesPage.NavigateToCompaniesAsync();
            
            string screenshotPath = await _screenshotHelper.CaptureScreenshotAsync("CompanyCreated");
            _reportHelper.LogPass("Company created successfully", screenshotPath);
        }

        [Then(@"I should be able to search and verify the created company")]
        public async Task ThenIShouldBeAbleToSearchAndVerifyTheCreatedCompany()
        {
            _companiesPage ??= new CompaniesPage(_page);
            
            bool companyExists = await _companiesPage.VerifyCompanyExistsAsync(_createdCompanyName);
            companyExists.Should().BeTrue($"Company '{_createdCompanyName}' should exist in the system");
            
            string screenshotPath = await _screenshotHelper.CaptureScreenshotAsync("CompanyVerified");
            _reportHelper.LogPass($"Company verified: {_createdCompanyName}", screenshotPath);
        }

        [When(@"I navigate to the Contacts page")]
        public async Task WhenINavigateToTheContactsPage()
        {
            _contactsPage = new ContactsPage(_page);
            await _contactsPage.NavigateToContactsAsync();
            
            _reportHelper.LogInfo("Navigated to Contacts page");
        }

        [Then(@"the contact should be created successfully")]
        public async Task ThenTheContactShouldBeCreatedSuccessfully()
        {
            _contactsPage ??= new ContactsPage(_page);
            
            // Navigate back to contacts list
            await _contactsPage.NavigateToContactsAsync();
            
            string screenshotPath = await _screenshotHelper.CaptureScreenshotAsync("ContactCreated");
            _reportHelper.LogPass("Contact created successfully", screenshotPath);
        }

        [Then(@"I should be able to search and verify the created contact")]
        public async Task ThenIShouldBeAbleToSearchAndVerifyTheCreatedContact()
        {
            _contactsPage ??= new ContactsPage(_page);
            
            bool contactExists = await _contactsPage.VerifyContactExistsAsync(_createdContactName);
            contactExists.Should().BeTrue($"Contact '{_createdContactName}' should exist in the system");
            
            string screenshotPath = await _screenshotHelper.CaptureScreenshotAsync("ContactVerified");
            _reportHelper.LogPass($"Contact verified: {_createdContactName}", screenshotPath);
        }

        [Then(@"I capture evidence with screenshots")]
        public async Task ThenICaptureEvidenceWithScreenshots()
        {
            string screenshotPath1 = await _screenshotHelper.CaptureScreenshotAsync("Evidence_Final");
            _reportHelper.LogInfo("Evidence captured with screenshots", screenshotPath1);
            
            _logger.Info("All evidence captured successfully");
        }

        #region CSV-Based Step Definitions

        [When(@"I create a new company from CSV row (.*)")]
        public async Task WhenICreateANewCompanyFromCSVRow(int rowIndex)
        {
            _companiesPage ??= new CompaniesPage(_page);
            _createdCompanyName = await _companiesPage.CreateCompanyFromCSVAsync(rowIndex, _reportHelper, _screenshotHelper);
            _scenarioContext.Set(_createdCompanyName, "CreatedCompanyName");
        }

        [When(@"I create a new contact from CSV row (.*)")]
        public async Task WhenICreateANewContactFromCSVRow(int rowIndex)
        {
            // Get company name from context
            try
            {
                _createdCompanyName = _scenarioContext.Get<string>("CreatedCompanyName");
            }
            catch (KeyNotFoundException)
            {
                // If not in context, read from CSV
                if (_csvDataReader == null)
                {
                    string csvPath = Path.Combine(Directory.GetCurrentDirectory(), "Configuration", "TestData", "TestData.csv");
                    _csvDataReader = new CsvDataReader(csvPath);
                }
                var testData = _csvDataReader.GetTestDataByRow(rowIndex);
                _createdCompanyName = testData.CompanyName ?? throw new ArgumentNullException("CompanyName cannot be null");
            }

            _contactsPage ??= new ContactsPage(_page);
            _createdContactName = await _contactsPage.CreateContactFromCSVAsync(rowIndex, _createdCompanyName, _reportHelper, _screenshotHelper);
            _scenarioContext.Set(_createdContactName, "CreatedContactName");
        }

        #endregion
    }
}
