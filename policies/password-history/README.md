# Password Reset - Prevent Previous X Passwords From Being Used

This policy enables the storing and checking of a user's previous set of passwords in order to prevent them from using a previous password during a Password Reset flow. The number of previous passwords is configurable in this example, and will default to 3 if not set.

The policy makes a REST API call to an `Azure App Service`, passing a JSON payload containing the user's object id along with the hashed new password. The `Azure App Service`, using a Managed Identity, will connect to an `Azure Key Vault`, retrieve the last X passwords for that user object id, and then compare those hashed passwords with the user's new hashed password. If the password has not been used before, the service will update the user's secret in `Azure Key Vault` with the new hashed password. If the password has been used before, then the service will not persist the new password in `Azure Key Vault`, and instead return a `409 Bad Request` to B2C, which will then display the error message to the user.

## Policy Features

### Claims Transformations

  - `HashPasswordWithEmail`

    This transformation takes the new password entered by the user and hashes it, using the user's email as a salt value. The output of this transformation is used in `GeneratePassHashBody`.

  - `GeneratePassHashBody`

    This transformation takes the hashed new password along with the user's object id and creates a JSON document which will be used to pass to the Azure App Service to perform password history checking.

### Claims Providers

  - `REST-PasswordHistoryCheck`: REST Validation Technical Profile that creates the JSON document (calling `GeneratePassHashBody`) and passes it to the Azure App Service to perform password history checking.

    Currently, this technical profile calls the App Service anonymously. **For Production** it is highly recommended to use certificate-based authentication.

  - `LocalAccountWritePasswordUsingObjectId-ForHistory`: An override of LocalAccountWritePasswordUsingObjectId, except that it calls `REST-PasswordHistoryCheck` as a validation technical profile before the AAD Password Write Validation Technical Profile.

## Quick Start


  1. Review the [README for the Azure App Service](https://github.com/azure-ad-b2c/samples/blob/master/policies/password-history/source-code/dotnet5/README.md). In this step, you'll create:
        a. Azure App Service
        b. Azure Key Vault
        c. Managed Identity (either System Assigned or User Assigned)

  2. Modify the policy by replacing all instances of yourtenant.onmicrosoft.com with your tenant name.
  
  3. Modify the policy by replacing the `<!--Sample action here: -->` section in `REST-PasswordHistoryCheck` with the URL of your Azure App Service.
  
  4. Go to the Azure AD B2C blade in the Azure Portal.  
      a. Go to the `Identity Experience Framework menu` -> `Policy Keys menu`
      b. Create a new `Policy key`  
      c. Give it the name `AccountTransformSecret`, choose `secret`, and `generate` options  

  5. Upload the policy files into your tenant.
  6. Enjoy!!
