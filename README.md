# Atomic CRM Playwright Automation - Case Study

## 📋 Project Overview

This is a comprehensive Playwright automation framework built with C# for testing the Atomic CRM Demo application. The project demonstrates modern automation engineering concepts including:

- **Playwright Framework Design** with C# and .NET 8
- **Data-Driven Automation** using CSV files and JSON configuration
- **Page Object Model (POM)** with Object Repository pattern
- **BDD approach** using Reqnroll (SpecFlow successor)
- **Comprehensive Reporting** with ExtentReports HTML and automatic PDF generation
- **Dynamic XPath** locators with label-based selectors
- **Component-based Architecture** with helper classes
- **Secure Credential Handling** with configuration management
- **Screenshot & Trace Evidence** capture for every step

## 🎯 Objective

Automate the following end-to-end workflow:
1. Open the Atomic CRM demo application (https://marmelab.com/atomic-crm-demo/)
2. Navigate to Companies and create a new Company with dynamic data
3. Navigate to Contacts and create a new Contact linked to the created Company
4. Search and verify both Company and Contact
5. Capture evidence (screenshots/traces)
6. Generate comprehensive test execution report

## 🏗️ Framework Architecture

```
AtomicCRM/
├── Configuration/
│   ├── ConfigSettings.json          # Application configuration
│   ├── TestData/
│   │   └── TestData.csv            # CSV test data with dynamic timestamps
│   ├── AppConfigReader.cs           # Configuration reader utility
│   └── TestDataReader.cs            # Test data reader utility
├── Component/
│   ├── WebActionHelper.cs           # Core Playwright actions
│   ├── WaitHelper.cs                # Wait operations
│   └── WebElementHelper.cs          # Element state checking
├── Drivers/
│   └── Driver.cs                    # Browser initialization and management
├── ObjectRepository/
│   ├── Locator.json                 # XPath locators with dynamic patterns
│   └── Locator.cs                   # Singleton locator accessor
├── Tests/
│   ├── AtomicCRMCommonMethods.cs    # Base class with FormFillingAsync
│   ├── PageComponent/
│   │   ├── CompaniesPage.cs         # Companies page with CSV workflow
│   │   └── ContactsPage.cs          # Contacts page with CSV workflow
│   ├── Features/
│   │   └── AtomicCRMWorkflow.feature # Gherkin feature files
│   └── StepDefinitions/
│       └── AtomicCRMWorkflowSteps.cs # Step definitions (single-line calls)
├── Hooks/
│   └── Hooks.cs                     # Before/After hooks + PDF generation
├── Utilities/
│   ├── Logger.cs                    # Serilog-based logging
│   ├── ScreenshotHelper.cs          # Screenshot capture utility
│   ├── ExtentReportHelper.cs        # ExtentReports HTML generation
│   ├── PdfReportGenerator.cs        # PDF report generator (NEW)
│   ├── CsvDataReader.cs             # CSV parsing with timestamp replacement
│   ├── DataGenerator.cs             # Dynamic data generation
│   └── Constants.cs                 # Centralized constants
├── Reports/                         # Auto-generated PDF reports (NEW)
├── TestResults/                     # HTML ExtentReports
├── Screenshots/                     # Test execution screenshots
└── reqnroll.json                    # Reqnroll configuration
```

## 🔧 Setup Steps

### Prerequisites

- **Visual Studio 2022** or later
- **.NET 8 SDK** installed
- **Node.js** (for Playwright browsers)

### Installation

1. **Clone or Download the Project**
   ```bash
   cd C:\Users\chandrub\source\repos\CaseStudy\CaseStudy
   ```

2. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

3. **Install Playwright Browsers**
   ```bash
   pwsh bin\Debug\net8.0\playwright.ps1 install
   ```
   
   Or on Windows without PowerShell:
   ```bash
   dotnet build
   playwright install
   ```

4. **Build the Project**
   ```bash
   dotnet build
   ```

## ▶️ Execution Steps

### Run All Tests

```bash
dotnet test
```

### Run Specific Tests by Tag

```bash
# Run only Smoke tests
dotnet test --filter "Category=Smoke"

# Run E2E tests
dotnet test --filter "Category=E2E"

# Run Regression tests
dotnet test --filter "Category=Regression"
```

### Run from Visual Studio

1. Open `AtomicCRM.sln` in Visual Studio
2. Build the solution (Ctrl + Shift + B)
3. Open Test Explorer (Test > Test Explorer)
4. Run All Tests or select specific scenarios

### Run with Different Browsers

Edit `Configuration\ConfigSettings.json` and change the `Browser` value:
```json
{
  "Browser": "Chrome"  // Options: Chrome, Firefox, MSEdge, Webkit
}
```

### Headless/Headed Mode

```json
{
  "HeadlessCondition": "No"  // "Yes" for headless, "No" for headed
}
```

## 📊 Test Reports

### ExtentReports (HTML)

After test execution, HTML reports are generated in:
```
TestResults/AtomicCRM_TestReport_<timestamp>.html
```

Features:
- ✅ Detailed step-by-step execution logs
- 📸 Screenshots attached to each step
- 🎨 Dark theme with modern UI
- 📈 Test statistics and charts
- 🔍 Filterable by status (Pass/Fail/Skip)

### PDF Reports (AUTO-GENERATED) 📄

**After every test run, a PDF report is automatically generated!**

PDF reports are saved in:
```
Reports/AtomicCRM_TestReport_<timestamp>.pdf
```

**How it works:**
1. Test execution completes and HTML report is generated
2. PdfReportGenerator automatically converts HTML to PDF using Playwright
3. PDF is saved with the same filename in the Reports folder
4. PDF includes all screenshots, logs, and test statistics

**Benefits:**
- ✅ **Shareable** - Easy to email or share with stakeholders
- ✅ **Professional** - Clean, formatted PDF output
- ✅ **Archivable** - Store as permanent record
- ✅ **No Manual Steps** - Fully automated conversion
- ✅ **High Fidelity** - Preserves all formatting and images from HTML

**Console Output:**
```
Test execution completed. HTML report generated at: TestResults/AtomicCRM_TestReport_20260524_143052.html
Generating PDF report...
PDF report generated successfully at: Reports/AtomicCRM_TestReport_20260524_143052.pdf
```

### Test Execution Logs

Logs are stored in:
```
Logs/TestLog_<date>.txt
```

### Screenshots

All screenshots are saved in:
```
Screenshots/
```

### Playwright Traces

For advanced debugging, Playwright traces are saved in:
```
Traces/
```

To view traces:
```bash
playwright show-trace Traces/<scenario>_<timestamp>.zip
```

## 🗂️ Configuration Management

### ConfigSettings.json

```json
{
  "Browser": "Chrome",
  "Environment": "DEMO",
  "HeadlessCondition": "No",
  "BaseUrl": "https://marmelab.com/atomic-crm-demo/",
  "isScenarioScreenshotEnabled": true,
  "isTraceViewEnabled": true,
  "SuccessStepsScreenshotEnabled": true
}
```

### TestData - Data-Driven Approach

Test data is externalized in **CSV format** at `Configuration/TestData/TestData.csv`:

**Features:**
- **CSV-based**: Easy to edit in Excel or any text editor
- **Dynamic Timestamps**: `{timestamp}` placeholder auto-replaced with `yyyyMMddHHmmss`
- **Multiple Rows**: Supports data-driven testing with multiple test datasets
- **25 Columns**: Comprehensive test data including Company and Contact fields

**Sample CSV Structure:**
```csv
TestType,CompanyName,Website,LinkedInURL,PhoneNumber,Sector,Size,Revenue,TaxIdentifier,Address,City,ZipCode,State,Country,Description,AccountManager,FirstName,LastName,Gender,Title,Email,ContactPhone,ContactLinkedIn,BackgroundInfo,HasNewsletter
CSV,AutoCompany_{timestamp},https://autocompany.com,https://linkedin.com/company/auto,+1-555-1234,Information Technology,51-250,5000000,TAX-2024-001,123 Main St,San Francisco,94105,CA,USA,Automated test company,John Doe,Test,User,He/Him,QA Engineer,test@auto.com,+1-555-5678,https://linkedin.com/in/testuser,QA professional,Yes
```

**Timestamp Replacement Example:**
- Input: `AutoCompany_{timestamp}`
- Output: `AutoCompany_20260524143052`

### Object Repository Pattern

Locators are centralized in `ObjectRepository/Locator.json`:

**Benefits:**
- **Separation of Concerns**: Locators separated from code
- **Easy Maintenance**: Update XPath in one place
- **Dynamic Patterns**: Reusable XPath templates with placeholders
- **Singleton Access**: `Locator.Instance` provides global access

**Dynamic Locator Example:**
```json
{
  "Common": {
    "DynamicTextBox": "//label[span[text()='{0}']]/following-sibling::input",
    "DynamicDropDown": "//div[@role='group']//label[.//span[text()='{0}']]/following-sibling::div//button[@role='combobox']"
  }
}
```

**Usage in Code:**
```csharp
string locator = _locator.DynamicTextBox.Replace("{0}", "Company name");
// Result: //label[span[text()='Company name']]/following-sibling::input
```

## 🔐 Secure Handling of Credentials

- **Configuration Files**: Sensitive data stored in `ConfigSettings.json` and `TestData/*.json`
- **Environment-based**: Different files for DEV/QA/UAT environments
- **GitIgnore**: Credential files can be added to `.gitignore`
- **Azure Key Vault Integration**: Can be extended to fetch secrets from Azure Key Vault
- **Encryption**: Sensitive data can be encrypted using built-in .NET libraries

**Best Practice**: Never commit actual credentials to source control. Use environment variables or secure vaults in CI/CD pipelines.

## 🧪 Framework Design Concepts

### 1. Page Object Model (POM)

Each page is represented as a class with:
- Locators defined as constants
- Methods representing user actions
- Abstraction of UI interactions

**Example**:
```csharp
public class CompaniesPage : BasePage
{
    private const string CompanyNameInput = "input[name='name']";
    
    public async Task FillCompanyNameAsync(string name)
    {
        await FillAsync(CompanyNameInput, name);
    }
}
```

### 2. BDD with Reqnroll

- **Feature Files**: Business-readable scenarios in Gherkin
- **Step Definitions**: C# implementation of Gherkin steps
- **Living Documentation**: Tests serve as documentation

### 3. Driver Pattern

- **Driver**: Centralized browser management
- **Configuration-driven**: Browser type, headless mode, timeouts
- **Context isolation**: Each scenario gets a fresh browser context

### 4. Component Architecture

**Helper Classes** provide reusable actions:

**WebActionHelper.cs** - Core Playwright actions
```csharp
public async Task ClickAsync(string locator)
public async Task FillAsync(string locator, string value)
public async Task PressSequentiallyAsync(string locator, string text)
```

**WaitHelper.cs** - All wait operations
```csharp
public async Task WaitForTimeoutAsync(int milliseconds)
public async Task WaitForNavigationAsync()
public async Task WaitForElementClickableAsync(string locator)
```

**WebElementHelper.cs** - Element state checking
```csharp
public async Task<bool> IsVisibleAsync(string locator)
public async Task<string> GetElementTextAsync(string locator)
public async Task<int> GetElementCountAsync(string locator)
```

### 5. Dynamic Form Filling

**AtomicCRMCommonMethods** base class provides `FormFillingAsync` method:

```csharp
var formData = new Dictionary<string, (string value, FieldType type)>
{
    { "Company name", (name, FieldType.TextBox) },
    { "Sector", (sector, FieldType.DropDown) },
    { "Description", (description, FieldType.TextArea) }
};
await FormFillingAsync(formData);
```

**Benefits:**
- Reduces code duplication
- Label-based field identification
- Automatic field type handling (TextBox, DropDown, TextArea, Toggle, Button)
- Single method for all form interactions

### 6. Encapsulated Page Workflows

**Page classes contain complete workflows:**

```csharp
// Step Definition (1 line)
_companiesPage.CreateCompanyFromCSVAsync(rowIndex, _reportHelper, _screenshotHelper);

// Page class handles:
// - CSV reading
// - Form filling
// - Screenshot capture
// - Report logging
// - Verification
// - Navigation
```

### 7. Utilities & Helpers

- **Logger**: Structured logging with Serilog
- **ScreenshotHelper**: Automated evidence capture
- **PdfReportGenerator**: Converts HTML reports to PDF
- **CsvDataReader**: CSV parsing with timestamp replacement
- **DataGenerator**: Dynamic test data generation
- **ExtentReportHelper**: Rich HTML reporting

### 8. Hooks & Lifecycle Management

```csharp
[BeforeTestRun]      // Initialize HTML report
[BeforeFeature]      // Create feature in report
[BeforeScenario]     // Setup browser, navigate to app, capture initial screenshot
[AfterStep]          // Capture screenshots, log steps (Pass/Fail)
[AfterScenario]      // Teardown browser, save trace, capture final screenshot
[AfterTestRun]       // Flush HTML report, Generate PDF report automatically
```

**PDF Generation Flow:**
1. `AfterTestRun` hook executes
2. HTML report is flushed to disk
3. `PdfReportGenerator` reads the HTML file path
4. Playwright browser converts HTML → PDF with preserved formatting
5. PDF saved in `Reports/` folder with same filename
6. Console displays both HTML and PDF paths

## 🤖 AI / MCP Integration

### Current Implementation

1. **Dynamic Data Generation**: AI-inspired data patterns using timestamps and randomization
2. **Intelligent Waiting**: Playwright's auto-waiting mechanisms
3. **Self-healing Locators**: CSS selectors with fallback strategies

### Future Enhancements

- **AI-powered Visual Testing**: Using Playwright's screenshot comparison
- **MCP Server Integration**: 
  - GitHub MCP for test result tracking
  - Memory MCP for storing test execution history
  - Playwright MCP for enhanced browser automation
- **GPT Integration**: For test case generation from requirements
- **Anomaly Detection**: ML models to detect unusual application behavior

### AI Usage in This Project

- **Copilot-assisted Development**: Framework structure designed with AI assistance
- **Pattern Recognition**: Reusable patterns from reference project
- **Best Practices**: Incorporated industry-standard automation practices
- **Code Generation**: Boilerplate code and repetitive patterns generated efficiently

## 📈 Test Execution Results

### Sample Execution

```
Test Run Summary:
  Total: 3
  Passed: 3
  Failed: 0
  Skipped: 0
  Duration: 2m 34s
```

### Key Metrics

- **Average Execution Time**: ~50 seconds per E2E scenario
- **Screenshot Capture**: Automatic on each step and failure
- **Trace Generation**: Enabled for debugging
- **Parallel Execution**: Supports up to 4 parallel threads

## 🚀 CI/CD Integration

### Azure DevOps Pipeline

```yaml
trigger:
  - main

pool:
  vmImage: 'windows-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '8.x'

- script: dotnet restore
  displayName: 'Restore packages'

- script: dotnet build
  displayName: 'Build project'

- script: pwsh bin\Debug\net8.0\playwright.ps1 install
  displayName: 'Install Playwright browsers'

- script: dotnet test --logger trx
  displayName: 'Run tests'

- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/*.trx'
```

### GitHub Actions

```yaml
name: Playwright Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.x'
      - run: dotnet restore
      - run: dotnet build
      - run: pwsh bin/Debug/net8.0/playwright.ps1 install
      - run: dotnet test
      - uses: actions/upload-artifact@v3
        with:
          name: test-results
          path: TestResults/
```

## 📝 Key Features

✅ **Cross-browser Testing** - Chrome, Firefox, Edge, Webkit  
✅ **Headless & Headed Modes** - Configurable execution  
✅ **Screenshot Evidence** - Auto-capture on steps and failures  
✅ **Playwright Traces** - Advanced debugging capability  
✅ **ExtentReports HTML + PDF** - Rich reports with automatic PDF generation  
✅ **CSV Data-Driven Testing** - Dynamic test data with timestamp replacement  
✅ **Object Repository Pattern** - Centralized locator management in JSON  
✅ **Component Architecture** - Helper classes for actions, waits, and element checks  
✅ **Dynamic XPath Generation** - Label-based locators from JSON patterns  
✅ **Structured Logging** - Serilog with file and console output  
✅ **Dynamic Data** - Timestamp-based unique test data in CSV  
✅ **BDD Approach** - Business-readable Gherkin scenarios  
✅ **Page Object Model** - Maintainable and scalable with FormFillingAsync  
✅ **Configuration-driven** - Easy environment management  
✅ **Single-line Step Definitions** - All logic encapsulated in Page classes  

## 🔍 Troubleshooting

### Common Issues

**Issue**: Playwright browsers not installed
```bash
Solution: Run `pwsh bin\Debug\net8.0\playwright.ps1 install`
```

**Issue**: Tests fail with timeout
```bash
Solution: Increase timeout in ConfigSettings.json
{
  "Timeout": "180000"  // 3 minutes
}
```

**Issue**: Locators not found
```bash
Solution: Check if application structure changed. Update locators in Tests/PageComponent.
```

## 📚 References

- [Playwright for .NET Documentation](https://playwright.dev/dotnet/)
- [Reqnroll Documentation](https://reqnroll.net/)
- [ExtentReports Documentation](https://www.extentreports.com/)
- [Atomic CRM Demo](https://marmelab.com/atomic-crm-demo/)

## 👨‍💻 Author

Developed as part of Playwright Automation Case Study demonstrating enterprise-grade automation framework design.

## 📄 License

This is a case study project for demonstration purposes.

---

**Last Updated**: May 2026  
**Framework Version**: 1.0  
**Playwright Version**: 1.52.0  
**.NET Version**: 8.0
