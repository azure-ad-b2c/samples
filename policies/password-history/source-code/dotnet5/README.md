# Password Hash Service

This service is hosted in an Azure App Service and is invoked by a B2C custom policy in order to compare a user's hashed "new password" against a list of previous hashed passwords stored in Azure Key Vault.

There are two configuration settings:

  - KeyVaultUri: This is the full URI to your Azure Key Vault, e.g. https://test-kv.vault.azure.net/
    
    This URI should have a trailing backslash at the end.

  - PreviousPasswordCount: This is the number of previous password hashes that this service will compare against the current "new password" hash to determine if this password is being reused. If the value is not set, default value of 3 is used.

There is a single controller method, a POST, that either returns a 200 OK or a 409 Bad Request.

  - 200 OK: indicates that the "new password" is not a repeat of the past `PreviousPasswordCount` passwords.
  - 409 Bad Request: indicates the "new password" is a repeat of the past `PreviousPasswordCount` passwords. The user error message will state that. The returned message will be in this JSON format:

  ```json
  {
    "version": "1.0.0",
    "status": 409,
    "code": "HISTORY001",
    "userMessage": "You cannot reuse the past 5 passwords",
    "developerMessage": "User password found in history list",
    "requestId": "1ea9a213-ad14-415a-9b3b-eebb992fc6da",
    "moreInfo": null
  }
  ```

To invoke this service from B2C, `POST` to:

  ```http
  https://<service-url>/passhash
  ```

with the following JSON payload:

```json
  {
      "username": "freeguy",
      "hash": "abc123"
  }
```

# Deployment

## Resources Needed

This sample uses the following Azure resources:

  - Azure Managed Identity (either System Managed or User Assigned)
  - Azure Key Vault
  - Azure App Service

### Azure App Service
You can create an Azure App Service using [these](https://docs.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore?tabs=net50&pivots=development-environment-vscode) steps. The documentation has options for various frameworks and IDE's -- the link uses .NET5 and VS Code. You can deploy this application based on your deployment method of choice.

Once created, you will need to set two application settings for your App Service:

  - KeyVaultUri: this is the URL to your Azure Key Vault that you will create later;
  - PreviousPasswordCount: this is the number of previous passwords retrieved for a user from Azure Key Vault by the service for checking.

### Azure Managed Identity

#### System Assigned Managed Identity
You can enable a System Assigned Managed Identity after you create the App Service. Go to the App Service's Settings -> Identity. Under "System Assigned", toggle the Status switch to 'On'.

#### User Assigned Managed Identity
If you want to use an existing Managed Identity, go to the App Service's Settings -> Identity section. Under User Assigned, click the '+ Add' menu and select the User Assigned Managed Identity you want to assign to this App Service.

### Azure Key Vault

You can create a new Azure Key Vault using [these](https://docs.microsoft.com/en-us/azure/key-vault/general/quick-create-portal) instructions. Once complete, your Managed Identity used needs the following Secret Permissions set in the Azure Key Vault Access Policies:

  - Secret Get
  - Secret List
  - Secret Set

You can also use Azure Role-Based Access Control for managing the Managed Identity's access to the Azure Key Vault.

### Have Azure App Service Use The Managed Identity

This App Service assumes an Azure Managed Identity is used to connect to Azure Key Vault. There are no connection strings being used in this sample. The `DefaultAzureCredential` set in the `KeyVaultManager` class assumes the App Service is running as a System Managed Identity.

If you are using a User Assigned Managed Identity, you will also need to set the `AZURE_CLIENT_ID` environment variable in the App Service's Application Settings to the Client ID of the User Assigned Managed Identity.

