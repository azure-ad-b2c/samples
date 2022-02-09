# Azure AD B2C Auto Account Link

This policy sample demonstrates how to link an account when a user arrives with the same email as an existing account. When the email is detected as being the same, the user is prompted to sign in with one of the methods already registered on the existing account. Once complete, the account is linked.

## Prerequisites

- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. You can **Quick Deploy** this [sample after running the setup tool](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=auto-account-linking). Otherwise, follow the below instructions to manually prepare your environment.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance on [storing the extension properties](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [adding the application objectID](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

## User Experience

### Scenario 1

- User has a Local Account (`bob@contoso.com`)
- User logs in with Facebook, and Facebook returns the email claim - `bob@contoso.com`
- User is presented with a screen to login with their Local Account
- User logs in with the Local Account
- Facebook account is merged with Local Account

### Scenario 2

- User has signed up with Facebook (`bob@contoso.com`)
- User logs in with Google, and Google returns the email claim - `bob@contoso.com`
- User is presented with a screen to login with their Facebook Account
- User logs in with the Facebook Account (check is performed to make sure email returned matches)
- Facebook account is merged with Google Account

### Scenario 3

- User has signed up with Facebook (`bob@contoso.com`), and linked their account with Google
- User logs in with Twitter, and Twitter returns the email claim - `bob@contoso.com`
- User is presented with a screen to login with their Facebook Account or Google Account
- User logs in with the Facebook/Google (check is performed to make sure email returned matches)
- Twitter account is merged with the Account

### Scenario 4
- User has signed up with Facebook (`bob@contoso.com`)
- User tries to sign up for a Local Account with `bob@contoso.com` 
- User is presented with error, stating the account exists
- User must perform forgot password flow
- Local account is already merged with Facebook Account - All social sign ups result in Local Account with random password. This is due to not being able to add a Local Account to a Federated-only account.

## How it works

TBC

## Notes

This sample policy is based on [SocialAndLocalAccountsWithMFA starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts).
