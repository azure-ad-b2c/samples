# A B2C IEF Custom Policy - Sign Up and Sign In with 'Terms of Use' prompt

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
In this scenario, we force users to check a box to accept the "terms of use", and upon sign in, we check whether the user has accepted the latest "terms of use", and if not they are presented with a screen to re-consent to the "terms of use", otherwise they are not authenticated.

## How it works
When the user signs up, the `LocalAccountSignUpWithLogonEmailCustom` technical profile implements a checkbox which the user must "check" to submit the form. This acts as the "terms of use" accepted signal. If the user does not select the check box, the user cannot create an account.  

When the account is created, since the user must have accepted the 'terms of use' to do so, the current time is written to the extension attribute `extension_termsOfUseConsentDateTime`.

The current time is computed within `LocalAccountSignUpWithLogonEmailCustom` technical profile, which calls the `GetCurrentDateTime` claims transformation.

At each sign in, the `Check-TOU-Status` validation technical profile is run (after credential validation with `login-noninteractive`). This validation technical profile reads the `extension_termsOfUseConsentDateTime` attribute to determine when the user last consented to the 'terms of use'. Then the `IsTermsOfUseConsentRequiredForDateTime` output claim transformation is run against `extension_termsOfUseConsentDateTime`. It returns `True` of the the date of consent is prior to the date configured in `IsTermsOfUseConsentRequiredForDateTime`, and false otherwise.

Then within the orchestration steps, the `SelfAsserted-Input-ToU-SignIn` technical profile is triggered if the consent is out of date. This page re-prompts the user for consent. If the user consents by clicking the check box, the `Update-TOU-Status` validation technical profile runs which writes the current time into `extension_termsOfUseConsentDateTime`. The next time the user logs on, they are not prompted for consent.

## Unit Tests
1. Sign up and verify the account cannot be created without selecting the 'terms of use' check box.
2. Sign up and select the 'terms of use' check box, verify that `extension_termsOfUseConsentDateTime` attribute is populated.
3. Sign in with an account that has not got `extension_termsOfUseConsentDateTime` attribute populated, they should be prompted for consent. Verify `extension_termsOfUseConsentDateTime` attribute is written to the account.
4. Change the date within `IsTermsOfUseConsentRequiredForDateTime` to the future, accounts that sign in should now be prompted to re-consent to the terms of use. Verify that `extension_termsOfUseConsentDateTime` attribute is updated. 