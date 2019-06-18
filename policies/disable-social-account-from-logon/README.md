# A B2C IEF Custom Policy which allows disabling accounts that are from External IDPs

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
For scenarios where you would like to block logins for social accounts or external IdP accounts that have been marked disabled in Azure AD B2C.

At sign in, read the `extension_accountEnabled` attribute as part of `AAD-UserReadUsingAlternativeSecurityId`. If this value is set to `false`, then run the `AAD-DisabledUserPage` orchestration step. 

Use an extension attribute rather than the native AccountEnabled attribute since AccountEnabled is set to false for accounts created without passwords - that applies to external IdP Accounts.

This step follows a validation technical profile as part of a dummy self asserted page which will only allow a user to proceed if extension_accountEnabled=true. 

This occurs via the outputClaimsTransoformation `AssertAccountEnabledIsTrue` called by the `AAD-AssertAccountEnabled` technical profile.

Since only users in a disabled state reach this page, this will always result in an error defined by the metadata key item `UserMessageIfClaimsTransformationBooleanValueIsNotEqual`. The user will not be issued a JWT Token.

Email is returned from Azure AD by adding the optional claims section into the [Application Registration Manifest](
https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-optional-claims
)

````
"optionalClaims": {
    "idToken": [
      {
        "name": "email",
        "source": null,
        "essential": false,
        "additionalProperties": []
      }
````

Email is persisted into `signInNames.emailAddress` using the `AAD-UserWriteUsingAlternativeSecurityId` technical profile.