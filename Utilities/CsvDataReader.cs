using AtomicCRM.Configuration;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace AtomicCRM.Utilities
{
    public class CsvDataReader
    {
        private readonly string _csvFilePath;

        public CsvDataReader(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
            
            if (!File.Exists(_csvFilePath))
            {
                throw new FileNotFoundException($"CSV file not found at: {_csvFilePath}");
            }
        }

        public List<TestDataRow> ReadTestData()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim
            };

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, config);
            
            var records = csv.GetRecords<TestDataRow>().ToList();
            
            // Replace timestamp placeholders with actual timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            foreach (var record in records)
            {
                record.CompanyName = record.CompanyName?.Replace("{timestamp}", timestamp);
                record.Email = record.Email?.Replace("{timestamp}", timestamp);
            }
            
            return records;
        }

        public TestDataRow GetTestDataByRow(int rowIndex)
        {
            var allData = ReadTestData();
            if (rowIndex < 0 || rowIndex >= allData.Count)
            {
                throw new IndexOutOfRangeException($"Row index {rowIndex} is out of range. Total rows: {allData.Count}");
            }
            return allData[rowIndex];
        }
    }

    public class TestDataRow
    {
        public string? TestType { get; set; }
        
        // Company fields
        public string? CompanyName { get; set; }
        public string? Website { get; set; }
        public string? LinkedInURL { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Sector { get; set; }
        public string? Size { get; set; }
        public string? Revenue { get; set; }
        public string? TaxIdentifier { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Description { get; set; }
        public string? AccountManager { get; set; }
        
        // Contact fields
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? Title { get; set; }
        public string? Email { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactLinkedIn { get; set; }
        public string? BackgroundInfo { get; set; }
        public string? HasNewsletter { get; set; }
    }
}
