# Public Preview: Azure AD B2C MFA with TOTP using any Authenticator app

To provide an higher assurance multi-factor option we are enabling "time based one time passcode" using Authenticator as an MFA choice for B2C customers. This option will allow customers  to save on the telephony charges associated with every step-up and still provide higher security than ever before for their end users accessing critical applications.

End users can download the Microsoft Authenticator app or any other authenticator app of their liking that supports the TOTP protocol. 





## Usage

See the latest documentation for this here: 

### Display Contorls
https://docs.microsoft.com/en-us/azure/active-directory-b2c/display-control-time-based-one-time-password


### Userflows

- All V3 userflows (Recommended) wil have a new option for TOTP enabled under the "Type of Method" MFA section in the properties blade. ![B2C Userflow Screen](media/userflow.png)
- You can customize the TOTP screens by providing the branded HTML files like you do for other pages by going to the "Page Layouts" blade of the userflows. ![B2C User Flows Page Layout screen](media/userflows-pagelayout.png)
https://docs.microsoft.com/en-us/azure/active-directory-b2c/multi-factor-authentication?pivots=b2c-user-flow

### Custom Policies
https://docs.microsoft.com/en-us/azure/active-directory-b2c/multi-factor-authentication?pivots=b2c-custom-policy

-  For custom policies see details below and the sample in the 'Policy' folder. 

### Using the Authenticator App 
- Microsoft Authenticator app can be downloaded from here:  https://www.microsoft.com/en-us/account/authenticator. Susbequent release of this feautre will have download links on the QR scan page for end users to download the app. 

- You can have your end users also use any other app of your choosing. You can add the download links to this app on the QR Code page. 

- When end users are adding/scanning the QR code to add the new account to the Microsoft Authenticator app, choose "Other (Google, Facebook, etc.)" option to add your B2C account. ![Microsoft Authenticator screen](media/AuthApp.jpg)

## Custom Policy Overview

The `SignUpOrSignInWithTOTP` policy uses the starterpack [B2C_1A_TrustFrameworkExtensions](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/blob/master/SocialAndLocalAccounts/TrustFrameworkExtensions.xml) as a base policy. 

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

## Just In Time TOTP Migration

If you were previously using the [TOTP multi-factor authentication](/../custom-mfa-totp) policy sample to implement TOTP then your users Secrets will be stored in the 'extension_StrongAuthenticationAppSecretKey' extension attribute.

To migrate these to the new method you can implement the [TOTP Migration Extension](policy/TrustFrameworkExtensionsTOTP_JIT.xml#L9) policy as the parent policy for either [SignUpOrSigninWithTOTP](policy/SignUpOrSigninWithTOTP.xml#L9) or [PaswordResetWithTOTP.xml](policy/PaswordResetWithTOTP.xml#L9) 

The below diagram depicts how the Just In Time TOTP migration works:
![Just In Time Flow Diagram](media/TOTPJITFlow.png)
