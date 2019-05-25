# Azure AD B2C: Remote profile

Demonstrates how to store and read user profile in a remote database. This sample use Azure Blob Storage to store and read the user profile.

## User flow
-  **Sign-up with local account** the LocalAccountSignUpWithLogonEmail technical profile invokes the REST-SignUpOrEditProfile validation technical profile. This technical profile reads the user objectId (return by the AAD-UserWriteUsingLogonEmail validation technical profile) and persist the profile to Azure Blob Table.
- **Sign in with local or social account** Additional orchestration step RESTSignIn invokes the REST-SignIn technical profile, which reads the user profile from the Azure Blob Table and return the profile back to Azure AD B2C.
- **Edit Profile** 
    - Read the profile: Additional orchestration step RESTSignIn invokes the REST-SignIn technical profile, which reads the user profile from the Azure Blob Table and return the profile back to Azure AD B2C. 
    - Update the profile:  The SelfAsserted-ProfileUpdate technical profile invokes the REST-SignUpOrEditProfile validation technical profile that updates the user profile in Azure Blob TAble

Policy files stored in [Remote-profile-policies directory](Remote-profile-policies)

## User flow with email in HASH format
In this flow, Azure AD B2C stores the email in HASH format. For example: `575E8926D4894C7057C6052357FE51B8C82C5E3C@b2c.com`. To support such flow, the policy needs to HASH the email before persisting or reading the account in the directory. This policy runs the flow mentioned above and also adding following functionality:
- **Sign-in with local account** and edit profile with local account
    - SelfAsserted-LocalAccountSignin-Email technical profile, calls the REST-HashSignInName technical profile to HASH the signInName
    - login-NonInteractive technical profile validates the account with the `emailHash` claim 
- **Sign-up with local account**
    - LocalAccountSignUpWithLogonEmail technical profile, calls the REST-HashEmail technical profile to HASH the eamil before creating the new account
    - AAD-UserWriteUsingLogonEmail technical profiles create the account with the `emailHash` claim 
- **Password reset (local account)**
    - LocalAccountDiscoveryUsingEmailAddress technical profile, calls the REST-HashEmail technical profile to HASH the eamil before reading the account
    - AAD-UserReadUsingEmailAddress technical profile reads the account with the `emailHash` claim 

Policy files stored in [Remote-profile-with-hash-policies](Remote-profile-with-hash-policies)

## REST API
Deploy your the REST API and update the the Azure Blob storage connection string in the appsettings.json file.
```JSON
  "AppSettings": {
    "BlobStorageConnectionString": "You Azure Blob storage account connection string"
  }
``` 

## Disclaimer
The sample is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The sample (Azure AD B2C policy and any companion code) is provided AS IS without warranty of any kind.

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.
