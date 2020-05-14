# Migrating CIAM solution from AWS Cognito to B2C - B2C Configuration

This section assumes that you have a working B2C tenant already setup you haven't, please follow the documentation on how to [create an Azure Active Directory B2C tenant](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).  

---
## Creating an App Registration that can perform the migration
In order to migrate all users, we need to register an Application that has ***Directory.ReadWrite.All*** permission in the B2C tenant. We will also use this app to create the custom attributes needed.

### Creating the App Registration via poral.azure.com
Steps:
1. Open the Azure portal and navigate to the B2C directory [portal.azure.com/yourtenant.onmicrosoft.com](https://portal.azure.com/yourtenant.onmicrosoft.com)
2. Open the ***App registrations (Preview)*** blade in ***Azure Active Directory*** [or use this link](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredAppsPreview)
3. ***+ New registration***, add ``http://localhost`` as Redirect URIs > Register
4. Copy the ***Application (client) ID*** as you will need it later 
5. ***API Permissions*** > ***+ Add a permission*** > ***Azure Active Directory Graph*** > ***Application permissions*** > ***Directory*** > check ***Directory.ReadWrite.All*** and Add permissions. 
5. ***API Permissions*** > ***+ Add a permission*** > ***Microsoft Graph*** > ***Application permissions*** > ***Applications*** > check ***Applications.ReadWrite.All*** and Add permissions. 
6. Grant admin consent so that status changes to "Granted"
7. ***Certificates & secrets*** > ***+ New client secret*** > give description and Add. Copy this ***client secret value**

### Creating the App Registration via scripts
There is a script you can run that will create the App Registration for you. It uses the standard AzureAD powershell cmdlets

```Powershell
.\connect-azureadb2c.ps1 -t yourtenant.onmicrosoft.com # interactive login using your Azure AD credentials

.\new-azureadb2c-appreg.ps1 -n "B2C-migration-app-name" -w $True -r @("http://localhost")

Getting Tenant info...
yourtenant.onmicrosoft.com
Creating App...
ObjectID:       7b...f3
ClientID:       89...c2
Secret:         de...8RCw=
Adding RequiredResourceAccess...
Creating ServicePrincipal...
```

You need to Grant permission for this app i portal.azure.com under ***API Permissions***

The script sets the applications AppId and secret as two environment variables called $env:client_id and $env:client_secret

### Test App Regitration
Update the default value in script [client-cred-login.ps1](scripts/client-cred-login.ps1) and enter the copied values for the $client_id and the $client_secret parameters. Test that the [client-cred-login.ps1](scripts/client-cred-login.ps1) script works by running it in the Powershell command prompt. The output be as below and you now have a valid access token in the environment variable ***OAUTH_access_token*** that is valid for 60 minutes. The other scripts rely on this envvar.

```Powershell
.\client-cred-login.ps1 -t yourtenant.onmicrosoft.com -c $env:client_id -s $env:client_secret
Environment variable set $env:OAUTH_access_token
Access Token valid until  YYYY-MM-DDTHH:MM:SS
```
---
## Create custom attributes
This migration example uses two custom attributes, ``requiresMigration`` and ``phoneNumberVerified``, both of datatype bool. The ``requiresMigration`` is a flag that is used the first time the user signs in after the switch to B2C and indicates that we should complete the migration by updating the password. The ``phoneNumberVerified`` attribute is brought over from AWS Cognito and if the value is true, the mobile phone number is updates as the phone number used for MFA. If it is false and unverified, we just leave the value in the B2C standard attribute ``mobile``. 

To create the custom attributes we need to run the script [create-ext-attribute.ps1](scripts/create-ext-attribute.ps1) twice - once per attribute. 
The parameters to pass to the script are
* -t : your tenant name
* -n : ``phoneNumberVerified`` and ``requiresMigration`` for the names
* -d : boolean for the datatype

```Powershell
.\create-ext-attribute.ps1 -t "yourtenant.onmicrosoft.com" -n "phoneNumberVerified" -d "boolean"
extension_89...c2_phoneNumberVerified

.\create-ext-attribute.ps1 -t "yourtenant.onmicrosoft.com" -n "requiresMigration" -d "boolean"
extension_89...c2_requiresMigration
```

Custom attributes are registered on an application and the script uses the ***b2c-extensions-app. Do not modify***. You can specify another objectId via the -o parameter. 

Note two things: 1) You will not see these attributes listed in the portal as those attributes are for User Flows, not Custom Policies. 2) These attributes only will be visible on a user when they have a value. Null valued custom attributes don't show.

---
## Migrate the users
The migration script [migrate-users-from-aws-cognito.ps1](scripts/migrate-users-from-aws-cognito.ps1) uses AWS CLI to retrieve users from the AWS Cognito UserPool and Azure AD GraphAPI to create them in the B2C tenant. The AWS CLI has a pagination feature so we can iterate as many times as needed. 

The parameters for running the migration script are:
* -t : your tenant name
* -p : AWS Cognito UserPool Id

```Powershell
.\client-cred-login.ps1 -t yourtenant.onmicrosoft.com -c $env:client_id -s $env:client_secret
Environment variable set $env:OAUTH_access_token
Access Token valid until  YYYY-MM-DDTHH:MM:SS

# if you need to get your AWS Cognito UserPool 
$pools = (aws cognito-idp list-user-pools --max-results 60) | ConvertFrom-json

.\migrate-users-from-aws-cognito.ps1 -t "yourtenant.onmicrosoft.com" -p $pools.UserPools.Id 
```

Note that the migration script generates a random password for the users that is never outputed somewhere. This means that no one knows the temporary password the B2C users where created with and it is not possible to login without going through the password migration part in the Custom Policy. 

After you have the migration script you can verify that the custom attributes were set ok via running

```Powershell
.\get-userbyemail.ps1 -t "yourtenant.onmicrosoft.com" -e "youruser@domain.com"
...
userType                              : Member
extension_89...c2_phoneNumberVerified : True
extension_89...c2_requiresMigration   : True
```

You can do the same with the Azure AD Powershell cmdlets

```Powershell
$user = Get-AzureADUser -Filter "signInNames/any(x:x/value eq 'youruser@domain.com')"
Get-AzureADUserextension -objectid $user.ObjectId
```

---
## Deploy the Azure Function
For users that have the ``requiresMigration`` flag set to True, the Custom Policy will make a REST API call to the Azure Function to validate the userid and password. The Azure Function uses AWS Cognito API [***AWSCognitoIdentityProviderService.InitiateAuth***](https://docs.aws.amazon.com/cognito-user-identity-pools/latest/APIReference/API_InitiateAuth.html) to do the authentication and the Function will return HTTP status 200 when AWS Cognito could authenticate the user and 409 with the error message "Could not verify migrated user in old system." which will be shown to the user. 

In portal.azure.com, create and Azure Function app and create a new HttpTrigger function. Then replace all the code in ``run.csx`` with the code in this repo for [run.csx](source-code/run.csx). Update the Configuration on the Azure Function App and add
1. ``AWS_Region`` your AWS region, like ``eu-west-1``
2. ``AWS_CognitoUserPoolAppClientId`` should be the client id value of the AWS Cognito App client that has a name that ends with "Ropc"
Make sure you save the configuration.

To test that the Azure Function works, copy the full URL for calling it and update script [user-migrate-auth-azfunc.ps1](scripts/user-migrate-auth-azfunc.ps1). Run it twice; Once with a userid/password that is good and once where the password is bad and you should get output like this. Don't be alarmed that the migrationRequired flag is False - it is the value we will update the user object with when migration is complete.

```Powershell
.\user-migrate-auth-azfunc.ps1
@{tokenSuccess=True; migrationRequired=False}

.\user-migrate-auth-azfunc.ps1
409: @{version=1.0.0; status=400; userMessage=Could not verify migrated user in old system.}
```

---
## Edit the Custom Policies
The B2C Custom Policy files comes from the Azure AD B2C Starter Pack [SocialAndLocalAccountsWithMfa](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa). You need do download the [TrustFrameworkBase.xml](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/blob/master/SocialAndLocalAccounts/TrustFrameworkBase.xml) file and modify the header values for TenantId and PolicyId.

The [TrustFrameworkExtensions.xml](https://github.com/azure-ad-b2c/samples/blob/master/policies/signin-migration/b2c/policy/TrustFrameworkExtensions.xml) and the [SignUpOrSignin.xml](https://github.com/azure-ad-b2c/samples/blob/master/policies/signin-migration/b2c/policy/SignUpOrSignin.xml) files you get from this github repo. You need to do the same headr modifications of the PolicyId and TenantId to replace ***yourtenant.onmicrosoft.com*** with your B2C tenant name:

Then, there are a number of edits you need to do before uploading the policies.

### AAD-Common Technical Profile in TrustFrameworkExtension.xml
In the file [TrustFrameworkExtension.xml](policy/TrustFrameworkExtension.xml) update the Technical Profile AAD-Common to include the ObjectID and the ClientID of the App Registration named ***b2c-extensions-app***. This is the way to get the Custom Policy to "see" our custom attributes. AAD-Common is defined in TrustFrameworkBase.xml and it is only additions we do in the Extension file for AAD-Common. 

```Xml
    <ClaimsProvider>
      <DisplayName>Azure Active Directory</DisplayName>
      <TechnicalProfiles>
        <TechnicalProfile Id="AAD-Common">
          <Metadata>
            <Item Key="ApplicationObjectId">7b...f3</Item>
            <Item Key="ClientId">89...c2</Item>
          </Metadata>
        </TechnicalProfile>
      </TechnicalProfiles>
    </ClaimsProvider>
```
### login-NonInteractive Technical Profile in TrustFrameworkExtension.xml
The second change is also in file [TrustFrameworkExtension.xml](policy/TrustFrameworkExtension.xml) where we need to provide our values for the IdentityExperienceFramework and ProxyIdentityExperienceFramework as explained in the [Custom Policy Get Started](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-get-started?tabs=applications#register-identity-experience-framework-applications) guide.

```Xml
        <TechnicalProfile Id="login-NonInteractive">
          <Metadata>
            <Item Key="client_id">ec43...7b7f</Item> <!-- ProxyIdentityExperienceFramework -->
            <Item Key="IdTokenAudience">b40f...14e6</Item> <!-- IdentityExperienceFramework -->
          </Metadata>
          <InputClaims>
            <InputClaim ClaimTypeReferenceId="client_id" DefaultValue="ec43...7b7f" />
            <InputClaim ClaimTypeReferenceId="resource_id" PartnerClaimType="resource" DefaultValue="b40f...14e6" />
          </InputClaims>
        </TechnicalProfile>
```

### REST API endpoint
The endpoint for calling the Azure Function needs to be updated. Copy the full URL and replace the ServiceURL value. 

```Xml
			<TechnicalProfile Id="UserMigrationViaLegacyIdp">
			<DisplayName>REST API call to communicate with Legacy IdP</DisplayName>
			<Protocol Name="Proprietary" Handler="Web.TPEngine.Providers.RestfulProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
			<Metadata>
				<Item Key="ServiceUrl">https://your-azfunc-appname.azurewebsites.net/api/ValidateUserAwsCognito?code=A...A==</Item>            
				<Item Key="AuthenticationType">None</Item>
				<Item Key="SendClaimsIn">Body</Item>
			</Metadata>
```

### Update tenant name and upload
Finally, make sure you have done the search-and-replace in all xml policy files from ``yourtenant.onmicrosoft.com`` to your tenant name.

Then, upload Base, Extension and SignUpOrSignin xml files to your B2C tenant.

