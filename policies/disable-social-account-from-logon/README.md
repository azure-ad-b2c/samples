# A B2C IEF Custom Policy which allows disabling accounts that are from External IDPs

## Disclaimer
The sample policy is developed and managed by the open-source community in GitHub. This policy is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The policy is provided AS IS without warranty of any kind.

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