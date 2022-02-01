# A B2C IEF Custom Policy - A Sign In policy which prompts for a user to Consent to Scopes for your API service

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
For scenarios where you provide a plug and play service to other partners. When the user chooses to use your service through a partner application, the user must login with their account with your service, and consent to various scopes which allow your service to share information with the partner application.

For example, if you are a payment service who can integrate with e-commerce applications, your API may expose various user information to the e-commerce application. When the user chooses to pay using your service, they can consent to share only particular information to the e-commerce application.

Some consent scopes will be mandatory for your service to integrate properly with the partner application, this is also handled in this sample.

It is assumed that users already have accounts with the consuming downstream service, therefore in this example only Sign In is handled.

## Prerequisites
- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance on [storing the extension properties](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [adding the application objectID](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

## How it works
The partner application send the authentication request to Azure AD B2C, with a scope parameter for the API registered in the AAD B2C directory. A single scope can be used, such as `https://contoso.onmicrosoft.com/api/default`. This is used to make sure the `access token` returned has the accepted audience for your API.

The policy defines a set of checkboxes which denote the various consent options the user can consent too. The `extension_consentRequired` claim lists all required scopes a user must consent to. And `extension_consentOptional` claim lists all optional scopes a user can consent to.

During the sign in, `AAD-UserReadUsingObjectId` technical profile is enhanced to read the `extension_currentConsent` attribute on the user. 
If `extension_currentConsent` does not exist on the user, then the user must be presented with the consent page `SelfAsserted-Consent-Page`; this is handled by a `precondition` on orchestration step 3.

When the user reaches `SelfAsserted-Consent-Page`, they are presented with the consent options, as per the configuration of `extension_consentRequired` and `extension_consentOptional`.


When the user submits their consent, the output claims transformation `MergeRequiredAndOptionalConsentRequest` is run, which merges the values of `extension_consentRequired` and `extension_consentOptional` into a comma seperated list and stores this in `extension_currentConsent`.
It also sets a claim to flag the user made a consent - `newConsent` is set to `true`.

Then, if `newConsent` is set to `true`, the `AAD-UserWriteConsentUsingObjectId` technical profile is executed, which writes the value of `extension_currentConsent` to the user attribute `extension_currentConsent`.

On subsequent logons, the `extension_currentConsent` is read to determine what the user has consented too.

In the issued Json Web Token, there will be a claim called `scope` with the value of `extension_currentConsent`.
This scope can be used at the API to determine what information can be accessed and returned back to the partner application that makes the API calls.

## Unit Tests
1. Sign In with a user, the consent page should be shown.
2. Validate the `scope` claim in the token matches the consent given in 1.
3. Sign in with the same user, the consent page should be skipped, and the `scope` claim in the token should match the consent given in 1.

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 