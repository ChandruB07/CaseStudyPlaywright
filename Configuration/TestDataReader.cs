using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtomicCRM.Configuration
{
    public class TestDataReader
    {
        private JObject? _testData;
        private readonly AppConfigReader _appConfigReader;

        public TestDataReader(AppConfigReader appConfigReader)
        {
            _appConfigReader = appConfigReader;
            LoadTestData();
        }

        private void LoadTestData()
        {
            string testDataPath = Path.Combine(Directory.GetCurrentDirectory(), _appConfigReader.GetTestDataPath());
            if (File.Exists(testDataPath))
            {
                string jsonContent = File.ReadAllText(testDataPath);
                _testData = JObject.Parse(jsonContent);
            }
        }

        public string GetBaseUrl() => _testData?["BaseUrl"]?.ToString() ?? string.Empty;
        
        public string GetUserName() => _testData?["UserName"]?.ToString() ?? string.Empty;
        
        public string GetPassword() => _testData?["Password"]?.ToString() ?? string.Empty;

        // Company related methods
        public string GetCompanyName() => ReplaceTimestamp(_testData?["Company"]?["Name"]?.ToString() ?? "");
        
        public string GetCompanySector() => _testData?["Company"]?["Sector"]?.ToString() ?? "";
        
        public string GetCompanySize() => _testData?["Company"]?["Size"]?.ToString() ?? "";
        
        public string GetCompanyLinkedIn() => _testData?["Company"]?["LinkedInUrl"]?.ToString() ?? "";
        
        public string GetCompanyAddress() => _testData?["Company"]?["Address"]?.ToString() ?? "";
        
        public string GetCompanyCity() => _testData?["Company"]?["City"]?.ToString() ?? "";
        
        public string GetCompanyState() => _testData?["Company"]?["StateAbbr"]?.ToString() ?? "";
        
        public string GetCompanyZipCode() => _testData?["Company"]?["ZipCode"]?.ToString() ?? "";
        
        public string GetCompanyCountry() => _testData?["Company"]?["Country"]?.ToString() ?? "";
        
        public string GetCompanyPhone() => _testData?["Company"]?["PhoneNumber"]?.ToString() ?? "";
        
        public string GetCompanyWebsite() => _testData?["Company"]?["Website"]?.ToString() ?? "";

        // Contact related methods
        public string GetContactFirstName() => ReplaceTimestamp(_testData?["Contact"]?["FirstName"]?.ToString() ?? "");
        
        public string GetContactLastName() => ReplaceTimestamp(_testData?["Contact"]?["LastName"]?.ToString() ?? "");
        
        public string GetContactGender() => _testData?["Contact"]?["Gender"]?.ToString() ?? "";
        
        public string GetContactTitle() => _testData?["Contact"]?["Title"]?.ToString() ?? "";
        
        public string GetContactEmail() => ReplaceTimestamp(_testData?["Contact"]?["Email"]?.ToString() ?? "");
        
        public string GetContactPhone1() => _testData?["Contact"]?["PhoneNumber1"]?.ToString() ?? "";
        
        public string GetContactPhone2() => _testData?["Contact"]?["PhoneNumber2"]?.ToString() ?? "";
        
        public string GetContactBackground() => _testData?["Contact"]?["Background"]?.ToString() ?? "";
        
        public string GetContactStatus() => _testData?["Contact"]?["Status"]?.ToString() ?? "";

        private string ReplaceTimestamp(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            return input.Replace("{timestamp}", timestamp);
        }

        public JObject? GetTestData() => _testData;
    }
}
