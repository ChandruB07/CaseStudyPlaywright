using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtomicCRM.ObjectRepository
{
    /// <summary>
    /// Object Repository class to manage all page locators from JSON file
    /// </summary>
    public class Locator
    {
        private static Locator? _instance;
        private static readonly object _lock = new object();
        private JObject? _locatorData;
        private readonly string _locatorFilePath;

        // Private constructor for Singleton pattern
        private Locator()
        {
            _locatorFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ObjectRepository", "Locator.json");
            LoadLocators();
        }

        /// <summary>
        /// Gets the singleton instance of Locator
        /// </summary>
        public static Locator Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Locator();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Loads locators from JSON file
        /// </summary>
        private void LoadLocators()
        {
            try
            {
                if (!File.Exists(_locatorFilePath))
                {
                    throw new FileNotFoundException($"Locator.json file not found at: {_locatorFilePath}");
                }

                string json = File.ReadAllText(_locatorFilePath);
                _locatorData = JObject.Parse(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading Locator.json: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a locator value by page name and locator key
        /// </summary>
        /// <param name="pageName">Name of the page (e.g., "CompaniesPage", "ContactsPage")</param>
        /// <param name="locatorKey">Key of the locator</param>
        /// <returns>XPath string</returns>
        public string GetLocator(string pageName, string locatorKey)
        {
            try
            {
                var locator = _locatorData?[pageName]?[locatorKey]?.ToString();
                if (string.IsNullOrEmpty(locator))
                {
                    throw new KeyNotFoundException($"Locator not found: {pageName}.{locatorKey}");
                }
                return locator;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving locator {pageName}.{locatorKey}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a dynamic locator with parameter replacement
        /// </summary>
        /// <param name="pageName">Name of the page</param>
        /// <param name="locatorKey">Key of the locator</param>
        /// <param name="parameters">Parameters to replace in the locator</param>
        /// <returns>XPath string with parameters replaced</returns>
        public string GetLocator(string pageName, string locatorKey, params string[] parameters)
        {
            string locator = GetLocator(pageName, locatorKey);
            return string.Format(locator, parameters);
        }

       

        #region CompaniesPage Locators

        public string CompaniesMenuLink => GetLocator("CompaniesPage", "CompaniesMenuLink");
        public string CreateCompanyButton => GetLocator("CompaniesPage", "CreateCompanyButton");
        public string CompanyNameInput => GetLocator("CompaniesPage", "CompanyNameInput");
        public string SectorSelect => GetLocator("CompaniesPage", "SectorSelect");
        public string SectorDropdown(string sectorName) => GetLocator("CompaniesPage", "SectorDropdown", sectorName);
        public string SizeSelect => GetLocator("CompaniesPage", "SizeSelect");
        public string SizeDropdown(string sizeName) => GetLocator("CompaniesPage", "SizeDropdown", sizeName);
        public string LinkedInInput => GetLocator("CompaniesPage", "LinkedInInput");
        public string AddressInput => GetLocator("CompaniesPage", "AddressInput");
        public string CityInput => GetLocator("CompaniesPage", "CityInput");
        public string StateInput => GetLocator("CompaniesPage", "StateInput");
        public string ZipCodeInput => GetLocator("CompaniesPage", "ZipCodeInput");
        public string PhoneInput => GetLocator("CompaniesPage", "PhoneInput");
        public string WebsiteInput => GetLocator("CompaniesPage", "WebsiteInput");
        public string RevenueInput => GetLocator("CompaniesPage", "RevenueInput");
        public string TaxIdentifierInput => GetLocator("CompaniesPage", "TaxIdentifierInput");
        public string CountryInput => GetLocator("CompaniesPage", "CountryInput");
        public string DescriptionInput => GetLocator("CompaniesPage", "DescriptionInput");
        public string AccountManagerSelect => GetLocator("CompaniesPage", "AccountManagerSelect");
        public string AccountManagerDropdown(string managerName) => GetLocator("CompaniesPage", "AccountManagerDropdown", managerName);
        public string CompanySaveButton => GetLocator("CompaniesPage", "SaveButton");
        public string CompanySearchInput => GetLocator("CompaniesPage", "SearchInput");
        public string CompanyListItem => GetLocator("CompaniesPage", "CompanyListItem");
        public string CompanyCardTitle(string companyName) => GetLocator("CompaniesPage", "CompanyCardTitle", companyName);
        public string CompanyCard(string companyName) => GetLocator("CompaniesPage", "CompanyCard", companyName);

        #endregion

        #region ContactsPage Locators

        public string ContactsMenuLink => GetLocator("ContactsPage", "ContactsMenuLink");
        public string CreateContactButton => GetLocator("ContactsPage", "CreateContactButton");
        public string FirstNameInput => GetLocator("ContactsPage", "FirstNameInput");
        public string LastNameInput => GetLocator("ContactsPage", "LastNameInput");
        public string GenderRadio(string gender) => GetLocator("ContactsPage", "GenderRadio", gender);
        public string TitleInput => GetLocator("ContactsPage", "TitleInput");
        public string CompanySelect => GetLocator("ContactsPage", "CompanySelect");
        public string CompanyDropdown(string companyName) => GetLocator("ContactsPage", "CompanyDropdown", companyName);
        public string EmailInput => GetLocator("ContactsPage", "EmailInput");
        public string EmailTypeSelect => GetLocator("ContactsPage", "EmailTypeSelect");
        public string PhoneNumber1Input => GetLocator("ContactsPage", "PhoneNumber1Input");
        public string PhoneNumber2Input => GetLocator("ContactsPage", "PhoneNumber2Input");
        public string AddPhoneButton => GetLocator("ContactsPage", "AddPhoneButton");
        public string BackgroundInput => GetLocator("ContactsPage", "BackgroundInput");
        public string ContactLinkedInInput => GetLocator("ContactsPage", "LinkedInInput");
        public string NewsletterSwitch => GetLocator("ContactsPage", "NewsletterSwitch");
        public string ContactSaveButton => GetLocator("ContactsPage", "SaveButton");
        public string ContactSearchInput => GetLocator("ContactsPage", "SearchInput");
        public string ContactListItem => GetLocator("ContactsPage", "ContactListItem");
        public string ContactRow(string contactName) => GetLocator("ContactsPage", "ContactRow", contactName);

        #endregion

        #region Common Locators

        public string DashboardLink => GetLocator("Common", "DashboardLink");
        public string NotificationRegion => GetLocator("Common", "NotificationRegion");
        public string LoadingSpinner => GetLocator("Common", "LoadingSpinner");
        public string ErrorMessage => GetLocator("Common", "ErrorMessage");
        public string SuccessMessage => GetLocator("Common", "SuccessMessage");
        
        // Dynamic Locators with placeholders
        public string DynamicTextBox => GetLocator("Common", "DynamicTextBox");
        public string DynamicDropDown => GetLocator("Common", "DynamicDropDown");
        public string DynamicDropDownOption => GetLocator("Common", "DynamicDropDownOption");
        public string DynamicButton => GetLocator("Common", "DynamicButton");
        public string DynamicToggle => GetLocator("Common", "DynamicToggle");
        public string DynamicTextArea => GetLocator("Common", "DynamicTextArea");
        public string DynamicRadioButton => GetLocator("Common", "DynamicRadioButton");

        #endregion
    }
}
