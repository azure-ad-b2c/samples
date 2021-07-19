# Azure AD B2C MFA with TOTP using any Authenticator app

To provide a higher assurance we are enabling "time based one time passcode" using Authenticator as an MFA choice for B2C customers. This option will allow customers  to save on the telephony charges associated with every step-up and still provide higher security than ever before for their end users accessing critical applications.

For custom policy users, you can add the provided extension and relying party file to your existing tree. For built-in customers, you will see a new option under the MFA methods section (under properties) in the userflow blades. 


## Overview

The `SignUpOrSignInTOTP-V2` policy uses the starterpack [B2C_1A_TrustFrameworkExtensions](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/blob/master/SocialAndLocalAccounts/TrustFrameworkExtensions.xml) as a base policy. 

### Known issues

This is a private preview feature and the tenants need to be allowlisted in order to use this feature. Until the feature is in public preview we recommend that you only use this feture in your non-prod tenants. 

The "Delete" device technical profiles are exposed here but the flows for those will be added in the next few months. 

## Important notes

The claim  `totpIdentifier` is used to copy the differnet type of signIn options and variable names that exist e.g. email, emails, username, userId, signInName, etc. If your policy uses any other claim to collect the signIn name then you would need to handle that by copying your claim in the `totpIdentifier` claim as we have done for other claims in the `SetTotpInitialValue` technical profile. 

```XML
<TechnicalProfile Id="CreateTotpIdentifier-UserId">
          <DisplayName>Set Totp Default Values</DisplayName>
          <Protocol Name="Proprietary" Handler="Web.TPEngine.Providers.ClaimsTransformationProtocolProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
          <OutputClaims>
            <OutputClaim ClaimTypeReferenceId="totpIdentifier" />
          </OutputClaims>
          <OutputClaimsTransformations>
            <OutputClaimsTransformation ReferenceId="UserIdToLogonIdentifier" />
          </OutputClaimsTransformations>
        </TechnicalProfile>
```
