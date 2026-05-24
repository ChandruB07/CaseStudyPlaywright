using Microsoft.Playwright;
using AtomicCRM.Component;
using AtomicCRM.Utilities;
using AtomicCRM.ObjectRepository;

namespace AtomicCRM.Tests
{
    public class AtomicCRMCommonMethods
    {
        protected readonly IPage Page;
        private readonly WebActionHelper _actionHelper;
        private readonly WaitHelper _waitHelper;
        private readonly WebElementHelper _elementHelper;
        protected readonly Logger Logger;
        private readonly Locator _locator;

        public AtomicCRMCommonMethods(IPage page)
        {
            Page = page;
            _actionHelper = new WebActionHelper(page);
            _waitHelper = new WaitHelper(page);
            _elementHelper = new WebElementHelper(page);
            Logger = new Logger();
            _locator = Locator.Instance;
        }

        // Dynamic locator generation based on field label using JSON patterns
        private string GenerateLocatorByLabel(string fieldLabel, FieldType fieldType)
        {
            return fieldType switch
            {
                FieldType.TextBox => _locator.DynamicTextBox.Replace("{0}", fieldLabel),
                FieldType.DropDown => _locator.DynamicDropDown.Replace("{0}", fieldLabel),
                FieldType.TextArea => _locator.DynamicTextArea.Replace("{0}", fieldLabel),
                FieldType.RadioButton => _locator.DynamicRadioButton.Replace("{0}", fieldLabel),
                FieldType.Toggle => _locator.DynamicToggle.Replace("{0}", fieldLabel),
                FieldType.Button => _locator.DynamicButton.Replace("{0}", fieldLabel),
                _ => throw new ArgumentException($"Unsupported field type: {fieldType}")
            };
        }

        // Dynamic FormFilling method using Dictionary
        public async Task FormFillingAsync(Dictionary<string, (string value, FieldType type)> formData)
        {
            foreach (var field in formData)
            {
                string fieldLabel = field.Key;
                string fieldValue = field.Value.value;
                FieldType fieldType = field.Value.type;

                if (string.IsNullOrEmpty(fieldValue) && fieldType != FieldType.Button)
                    continue;

                string locator = GenerateLocatorByLabel(fieldLabel, fieldType);

                switch (fieldType)
                {
                    case FieldType.TextBox:
                    case FieldType.TextArea:
                        await _actionHelper.FillAsync(locator, fieldValue);
                        Logger.Info($"Filled {fieldType}: {fieldLabel} = {fieldValue}");
                        break;

                    case FieldType.DropDown:
                        await _actionHelper.ClickAsync(locator);
                        await _waitHelper.WaitForTimeoutAsync(500);
                        string dropdownOption = _locator.DynamicDropDownOption.Replace("{0}", fieldValue);
                        await _actionHelper.ClickAsync(dropdownOption);
                        Logger.Info($"Selected dropdown: {fieldLabel} = {fieldValue}");
                        break;

                    case FieldType.RadioButton:
                        await _actionHelper.ClickAsync(locator);
                        Logger.Info($"Selected radio: {fieldLabel}");
                        break;

                    case FieldType.Toggle:
                        var currentState = await _elementHelper.GetElementAttributeAsync(locator, "aria-checked") == "true";
                        bool desiredState = fieldValue.Equals("Yes", StringComparison.OrdinalIgnoreCase) || 
                                          fieldValue.Equals("True", StringComparison.OrdinalIgnoreCase);
                        
                        if (currentState != desiredState)
                        {
                            await _actionHelper.ClickAsync(locator);
                            Logger.Info($"Toggled switch: {fieldLabel} = {desiredState}");
                        }
                        break;

                    case FieldType.Button:
                        await _actionHelper.ClickAsync(locator);
                        await _waitHelper.WaitForTimeoutAsync(2000);
                        Logger.Info($"Clicked button: {fieldLabel}");
                        break;
                }
            }
        }

        // Existing common methods below...

        // Common Text Input Methods
        public async Task FillTextBoxAsync(string locator, string value)
        {
            await _actionHelper.FillAsync(locator, value);
        }
        // Common Click Methods
        public async Task ClickElementAsync(string locator)
        {
            await _actionHelper.ClickAsync(locator);
        }

        // Common Toggle/Switch Methods
        public async Task SetToggleSwitchAsync(string locator, bool desiredState)
        {
            var currentState = await _elementHelper.GetElementAttributeAsync(locator, "aria-checked") == "true";
            
            if (currentState != desiredState)
            {
                await _actionHelper.ClickAsync(locator);
                Logger.Info($"Toggle switch set to: {desiredState}");
            }
        }
        // Common Navigation Methods
        public async Task NavigateToPageAsync(string locator)
        {
            await _actionHelper.ClickAsync(locator);
            await _waitHelper.WaitForNavigationAsync();
        }

        public async Task WaitForNavigationAsync()
        {
            await _waitHelper.WaitForNavigationAsync();
        }

        // Common Search Methods
        public async Task SearchAsync(string searchInputLocator, string searchText)
        {
            await _actionHelper.FillAsync(searchInputLocator, searchText);
            await _waitHelper.WaitForTimeoutAsync(1000);
        }

        public async Task<bool> VerifyElementExistsInListAsync(string listItemLocator, string searchText)
        {
            var elements = await _elementHelper.GetAllElementsAsync(listItemLocator);
            
            foreach (var element in elements)
            {
                var text = await element.TextContentAsync();
                if (text != null && text.Contains(searchText))
                {
                    Logger.Info($"Element found: {searchText}");
                    return true;
                }
            }
            
            Logger.Warning($"Element not found: {searchText}");
            return false;
        }

        public async Task<int> GetElementCountAsync(string locator)
        {
            return await _elementHelper.GetElementCountAsync(locator);
        }

        public async Task WaitAsync(int milliseconds)
        {
            await _waitHelper.WaitForTimeoutAsync(milliseconds);
        }

        // Common File Upload Methods
        public async Task UploadFileAsync(string locator, string filePath)
        {
            await _actionHelper.UploadFileAsync(locator, filePath);
        }

    }
}
