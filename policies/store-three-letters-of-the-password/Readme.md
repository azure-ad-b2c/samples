# HASH three characters of the password 

A call center uses Azure AD B2C to validate a customer phoning in. To do this call center takes three characters from the password and ask the customer calling in to provide the three characters plus some other known facts as part of the authentication process.  

With the Identity Experience Framework, which underlies Azure Active Directory B2C (Azure AD B2C) custom policy, you can integrate with a RESTful API in a user journey. This sample .Net core web API, demonstrate how to extract the three characters of the password, by calling [Restful technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/restful-technical-profile) in  a [validation technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/validation-technical-profile). During sign-up or password reset, the policy calls a REST API to HASH three letters of the password and store the values in the user profile. 

## Call the REST API during sign-up and password reset flow
The **LocalAccountSignUpWithLogonEmail** and **LocalAccountWritePasswordUsingObjectId** technical profiles are configured to add the **REST-HashPassword** validation technical profile (before storing the password to the user account in Azure AD B2C). 

The validation technical profile sends the **newPassword** claim to the REST API. The REST API extracts the first three characters of the password, the second three characters, and the third three characters, then HASH the values extraced and returns them back to Azure AD B2C.

The **AAD-UserWriteUsingLogonEmail** (sign-up) and **AAD-UserWritePasswordUsingObjectId** (password reset) technical profile are configured to store the passwords fragments in HASH format into extension attribute that can be later use to mach value provided during the call to the call center.

## Prepare the policy
1. Find and replace the `tenant-name` with your own tenant name
1. In `login-NonInteractive` technical profile, set the value of `client_id` and `resource_id` input claims, with your **ProxyIdentityExperienceFramework** application Id. For more information, see [Get started with custom policies in Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom#register-applications)
1. In `AAD-Common` technical profile, update the objectId and appId with your own values. For more information, see [Use custom attributes in a custom profile edit policy](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom)
1. Deploy the REST API to a public web server, and change the `ServiceUrl` metadata of the `REST-HashPassword` technical profile

## Source code
The custom REST API source code for .Net core is available under the [source code folder](source-code/dot-net-core)

## Custom policy
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.

### Base policy
In the **TrustFrameworkBase.xml** file, we move the validation technical profiles of the `LocalAccountSignUpWithLogonEmail` (sign-up) and `LocalAccountWritePasswordUsingObjectId` (password reset) technical profiles to the extension polity. 

### Extension policy
1. The `ClaimsSchema` section contains the claims to store the password fragments in HASH format. Those claims must start with `extension_` prefix.
1. Customized the `LocalAccountSignUpWithLogonEmail` (sign-up) and `LocalAccountWritePasswordUsingObjectId` (password reset) technical profile to call the REST-HashPassword validation technical profile
1. The `REST-HashPassword` technical profile is a custom Restful service to HASH the password. This technical profile sends the password as input        claim and return the passwords in HASH format, including the first three letters, second three letters, and third three letters as output claims. The input and output claims map the name use in the policy with the name defined in the REST API code 
1. The `AAD-Common` technical profile is configured to support [extension attributes](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom)
1. The `AAD-UserWriteUsingLogonEmail` (sign-up) and `AAD-UserWritePasswordUsingObjectId` (password reset) technical profile are configured to store the passwords fragments in HASH format.
1. 

## Disclaimer
The sample is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The sample (Azure AD B2C policy and any companion code) is provided AS IS without warranty of any kind.

