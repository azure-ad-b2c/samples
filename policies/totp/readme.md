# Limited Preview: Azure AD B2C MFA with TOTP using any Authenticator app

To provide an higher assurance multi-factor option we are enabling "time based one time passcode" using Authenticator as an MFA choice for B2C customers. This option will allow customers  to save on the telephony charges associated with every step-up and still provide higher security than ever before for their end users accessing critical applications.

End users can download the Microsoft Authenticator app or any other authenticator app of their liking that supports the TOTP protocol. 


## Disclaimer

This is a limited preview feature and the tenants need to be allowlisted in order to use this feature. Until the feature is in public preview we recommend that you only use this feture in your non-prod tenants. 


The "Delete" device technical profiles are exposed here but the flows for those will be added in the next few months. 


## Usage

### Userflows

- All V3 userflows (Recommended) wil have a new option for TOTP enabled under the "Type of Method" MFA section in the properties blade. [Userflow](media/userflow.png)
- You can customize the TOTP screens by providing the branded HTML files like you do for other pages by going to the "Page Layouts" blade of the userflows. [PageLayout](media/userflows-pagelayout.png)

### Custom Policies
-  For custom policies see details below and the sample in the policies folder

### Using the Authenticator App 
- Microsoft Authenticator app can be downloaded from here:  https://www.microsoft.com/en-us/account/authenticator. Susbequent release of this feautre will have download links on the QR scan page for end users to download the app. 

- You can have your end users also use any other app of your choosing. You can add the download links to this app on the QR Code page. 

- When end users are adding/scanning the QR code to add the new account to the Microsoft Authenticator app, choose "Other (Google, facebook, etc.)" option to add your B2C account. [AuthApp](media/AuthApp.jpg)

## Custom Policy Overview

The `SignUpOrSignInTOTP-V2` policy uses the starterpack [B2C_1A_TrustFrameworkExtensions](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/blob/master/SocialAndLocalAccounts/TrustFrameworkExtensions.xml) as a base policy. 

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
