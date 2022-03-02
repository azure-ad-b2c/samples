# A B2C IEF Custom Policy - Sign Up and Sign In with 'Terms of Use' prompt

## Live demo

To test the policy, follow these steps:

1. For sign up experience, [create a local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SUSI_TOU/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with your email address.
1. Select the *terms of use* check box when signing up.
1. For password rest experience,[create a new local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_signup_signin/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with another email address.
1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_PasswordReset_TOU/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_PasswordReset_TOU* policy.
1. Reset password and then select *terms of use* checkbox.

## Scenario

In this scenario, we force users to check a box to accept the "terms of use", and upon sign in, we check whether the user has accepted the latest "terms of use", and if not they are presented with a screen to re-consent to the "terms of use", otherwise they are not authenticated.

## Reference

[Capture terms of use agreement](https://docs.microsoft.com/en-us/azure/active-directory-b2c/manage-user-access#capture-terms-of-use-agreement) - Microsoft AAD B2C Documentation as reference material.

## Prerequisites

- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance on [storing the extension properties](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [adding the application objectID](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

## How it works

When the user signs up, the `LocalAccountSignUpWithLogonEmailCustom` technical profile implements a checkbox which the user must "check" to submit the form. This acts as the "terms of use" accepted signal. If the user does not select the check box, the user cannot create an account.  

When the account is created, since the user must have accepted the 'terms of use' to do so, the current time is written to the extension attribute `extension_termsOfUseConsentDateTime`.

The current time is computed within `LocalAccountSignUpWithLogonEmailCustom` technical profile, which calls the `GetCurrentDateTime` claims transformation.

At each sign in, the `Check-TOU-Status` validation technical profile is run (after credential validation with `login-noninteractive`). This validation technical profile reads the `extension_termsOfUseConsentDateTime` attribute to determine when the user last consented to the 'terms of use'. Then the `IsTermsOfUseConsentRequiredForDateTime` output claim transformation is run against `extension_termsOfUseConsentDateTime`. It returns `termsOfUseConsentRequired:true` if the the date of consent is prior to the date configured in `IsTermsOfUseConsentRequiredForDateTime`, and false otherwise.

Then within the orchestration steps, the `SelfAsserted-Input-ToU-SignIn` technical profile is triggered if the consent is out of date. This would occur if the value of `termsOfUseConsentRequired` was `true`.
This page re-prompts the user for consent. If the user consents by clicking the check box, the `Update-TOU-Status` validation technical profile runs which writes the current time into `extension_termsOfUseConsentDateTime`. The next time the user logs on, they are not prompted for consent.

## Unit Tests

1. Sign up and verify the account cannot be created without selecting the 'terms of use' check box.
2. Sign up and select the 'terms of use' check box, verify that `extension_termsOfUseConsentDateTime` attribute is populated.
3. Sign in with an account that has not got `extension_termsOfUseConsentDateTime` attribute populated, they should be prompted for consent. Verify `extension_termsOfUseConsentDateTime` attribute is written to the account.
4. Change the date within `IsTermsOfUseConsentRequiredForDateTime` to the future, accounts that sign in should now be prompted to re-consent to the terms of use. Verify that `extension_termsOfUseConsentDateTime` attribute is updated. 