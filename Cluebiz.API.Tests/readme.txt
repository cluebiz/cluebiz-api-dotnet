To set up the test project in your environment, follow these steps:


1. In a terminal, navigate to this project's root directory and run:
	dotnet user-secrets init

Then run
	dotnet user-secrets set "ServerAddress" "your_server_address"
	dotnet user-secrets set "UserId" "your_user_id"
	dotnet user-secrets set "Key" "your_key"

with your url/user/key.


2. Open the appsettings.Local.json file and set the values that should be used in your tests.

	"CustomerName" is the name of the customer that should be used for tests needing a customer client (practically all of them)
	"GuidelineTitle" is the name of the guideline that should be used for tests needing a guideline (Must be a guideline belonging to 'CustomerName')