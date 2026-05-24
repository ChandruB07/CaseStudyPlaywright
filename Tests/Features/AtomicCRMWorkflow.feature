Feature: Atomic CRM End-to-End Workflow
	As a QA Automation Engineer
	I want to automate the complete workflow of creating companies and contacts in Atomic CRM
	So that I can ensure the application works correctly and generate comprehensive test reports

@AtomicCRM @E2E @Smoke @CSV
Scenario Outline: Create Company and Contact from CSV Data
	Given I am on the Atomic CRM application
	When I navigate to the Companies page
	And I create a new company from CSV row <RowIndex>
	Then the company should be created successfully
	And I should be able to search and verify the created company
	When I navigate to the Contacts page
	And I create a new contact from CSV row <RowIndex>
	Then the contact should be created successfully
	And I should be able to search and verify the created contact
	And I capture evidence with screenshots

Examples:
	| RowIndex |
	| 0        |