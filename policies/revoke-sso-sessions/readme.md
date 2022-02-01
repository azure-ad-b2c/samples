# A B2C IEF Custom Policy which invalidates all SSO session across all devices after the users refresh token has been revoked

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
A users refresh token maybe revoked to prevent continued long term access to an application, across devices. In addition to refresh token revocation, the single sign on cookies can be revoked. This prevents a user on another device to be able to obtain a new set of tokens using the [Azure AD B2C web session cookies](https://docs.microsoft.com/en-us/azure/active-directory-b2c/session-behavior?pivots=b2c-custom-policy#configure-azure-ad-b2c-session-behavior).

Common scenarios include when a user uses a "change password" or "forgot password" journey. In these cases, both the refresh tokens and the session cookies should be invalidated, forcing all other devices to have to re-authenticate. The user on the device which made the operation, will also lose their web session SSO cookies. To allow the active device to maintain a session, you can bootstrap a new authentication journey using an [id_token_hint](https://docs.microsoft.com/azure/active-directory-b2c/id-token-hint).

The sample will work for both Local and Federated/Social accounts. 
The sample will also be applicable where Keep Me Signed In is used.

## Refresh token revocation
To revoke the users Refresh Token, use the [powershell command](https://docs.microsoft.com/en-us/powershell/module/azuread/revoke-azureaduserallrefreshtoken?view=azureadps-2.0):
```powershell
Revoke-AzureADUserAllRefreshToken
    -ObjectId <String>
    [<CommonParameters>]
```

This command sets the users `refreshTokensValidFromDateTime` attribute to the current time. All refresh tokens issued prior to this time will thereby be rejected by Azure AD B2C.

## Revoke the users Azure AD B2C web sessions
To revoke the users Azure AD B2C web sessions, a custom policy which compares the users initial login time, to the `refreshTokensValidFromDateTime` attribute can be used. If the last login time was prior to the  `refreshTokensValidFromDateTime` value, then Azure AD B2C returns an error message back to the application.

The users initial login time can be stored inside the Azure AD B2C web session cookie, and will be evaluated in all user journeys to ensure the session is still valid in respect to the `refreshTokensValidFromDateTime` attribute.

### Prerequisites
- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance on [storing the extension properties](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [adding the application objectID](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

## How it works

Generate the lastLogonTime at the initial sign in and sign up screen:
```xml
  <OrchestrationStep Order="1" Type="ClaimsExchange">
    <ClaimsExchanges>
      <ClaimsExchange Id="RecordFirstLogonTime" TechnicalProfileReferenceId="CT-RecordFirstLogonTime" />
    </ClaimsExchanges>
  </OrchestrationStep>

  <ClaimsTransformation Id="GetSystemDateTime" TransformationMethod="GetCurrentDateTime">
    <OutputClaims>
      <OutputClaim ClaimTypeReferenceId="lastLogonTime" TransformationClaimType="currentDateTime" />
    </OutputClaims>
  </ClaimsTransformation>
```

Store the `lastLogonTime` claim into the session cookie:
```xml
  <TechnicalProfile Id="SM-AAD">
    <PersistedClaims>
      <PersistedClaim ClaimTypeReferenceId="lastLogonTime" />
    </PersistedClaims>
  </TechnicalProfile>
```

Read the `refreshTokensValidFromDateTime` attribute:
```xml
  <OrchestrationStep Order="3" Type="ClaimsExchange">
    <ClaimsExchanges>
      <ClaimsExchange Id="AADUserReadWithObjectId" TechnicalProfileReferenceId="AAD-UserReadUsingObjectId" />
```

```xml
  <TechnicalProfile Id="AAD-UserReadUsingObjectId">
    <OutputClaims>
      <OutputClaim ClaimTypeReferenceId="refreshTokensValidFromDateTime" PartnerClaimType="refreshTokensValidFromDateTime"/>
    </OutputClaims>
```

Compare the lastLogin date claim with the refreshTokensValidFromDateTime date claim:
```xml
  <OrchestrationStep Order="4" Type="ClaimsExchange">
    <ClaimsExchanges>
      <ClaimsExchange Id="CompareSessionsTime" TechnicalProfileReferenceId="CT-CompareSessionTime" />


  <ClaimsTransformation Id="CompareLastLogonTimeToRTRevocationTime" TransformationMethod="DateTimeComparison">
    <InputClaims>
      <InputClaim ClaimTypeReferenceId="lastLogonTime" TransformationClaimType="firstDateTime" />
      <InputClaim ClaimTypeReferenceId="refreshTokensValidFromDateTime" TransformationClaimType="secondDateTime" />
    </InputClaims>
    <InputParameters>
      <InputParameter Id="operator" DataType="string" Value="earlier than" />
      <InputParameter Id="timeSpanInSeconds" DataType="int" Value="10" />
    </InputParameters>
    <OutputClaims>
      <OutputClaim ClaimTypeReferenceId="isSessionRevoked" TransformationClaimType="result" />
    </OutputClaims>
  </ClaimsTransformation>
```

Any web session created after the time at which the refresh token was revoked will be valid, and `isSessionRevoked` will return as `False`.
Any web session created before the time at which the refresh token was revoked will be invalid, and `isSessionRevoked` will return as `True`.
**Web sessions that are created within 10 seconds prior to revoking the refresh token will continue to be valid.**

This logic allows any **password change** journey to continue to provide the user a valid SSO session, and only other devices are logged out.

If `isSessionRevoked` has returned as `True`, call the OAuth2 error technical profile:
```xml
  <TechnicalProfile Id="ReturnOAuth2Error">
    <DisplayName>Return OAuth2 error</DisplayName>
    <Protocol Name="None" />
    <OutputTokenFormat>OAuth2Error</OutputTokenFormat>
    <CryptographicKeys>
      <Key Id="issuer_secret" StorageReferenceId="B2C_1A_TokenSigningKeyContainer" />
    </CryptographicKeys>
    <InputClaims>
      <InputClaim ClaimTypeReferenceId="errorCode" />
      <InputClaim ClaimTypeReferenceId="errorMessage"/>
    </InputClaims>
  </TechnicalProfile>
```

When the **OAuth2 error technical profile** (ReturnOAuth2Error) executes, it will also destroy the Azure AD B2C web session SSO cookie on this device. Therefore a logout request to Azure AD B2C is not necessary for further clean up.

You must include this logic to protect all user journeys that you offer. See how this is used in the profile edit journey to evaluate the session before presenting the profile edit page.

## Testing
To test the policy:
1. Use the **SignUpOrSignIn** policy to sign in or sign up.
1. Wait 10 seconds, then using powershell revoke the users refresh token: `Revoke-AzureADUserAllRefreshToken -ObjectId <GUID>`.
1. Launch the SignUpOrSignIn policy or the ProfileEdit policy (remove the prompt query parameter).
1. You should be presented with an error from Azure AD B2C: `AAD_Custom_Error_SessionRevoked: Session has been revoked due to refresh token revocation.`
 
## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
