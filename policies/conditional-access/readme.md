# Azure AD B2C sign-in with Conditional access

Azure Active Directory (Azure AD) Conditional Access is the tool used by Azure AD B2C to bring signals together, make decisions, and enforce organizational policies. Automating risk assessment with policy conditions means risky sign-ins are at once identified and remediated or blocked. For more information, see [Define a Conditional Access technical profile in an Azure Active Directory B2C custom policy](https://docs.microsoft.com/azure/active-directory-b2c/conditional-access-technical-profile).

## Live demo

To check out the user experience of conditional access, follow these steps:

1. Open [TOR Browser](https://www.torproject.org/download/) to simulate a risky sign-in
1. [Sign-up or sign-in](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_signup_signin_ConditionalAccess/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with the *B2C_1A_Demo_SignUp_SignIn_ConditionalAccess* policy. Complete the sign-up or sign-in flow. Since you use Tor browser, you should be triggered by the conditional access to run through multi-factor authentication. Complete the MFA step by registering and verifying your phone number. 
1. Open your favorite browser, such as Edge, Chrome, Safari or FireFox to simulate a legitimate sign-in.
1. [Sign-up or sign-in](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_signup_signin_ConditionalAccess/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with the *B2C_1A_Demo_SignUp_SignIn_ConditionalAccess* policy. Complete the sign-up or sign-in flow. This time the MFA step will be skipped (unless your sign-in is still considered illegitimate).

Note, you can also [sign-up or sign-in](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_signup_signin_ConditionalAccess_WhatIf/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with the *B2C_1A_Demo_SignUp_SignIn_ConditionalAccess_WhatIf* policy. The what if type of policy will always skip the MFA. Instead, the return ID token contains the conditional access recommendation. Whether a user should be block, run though MFA. If you don't see these claims, the sign-in is considered legitimate. The following example shows an access token for a risked sign-in:

```json
{
  "exp": 1646207426,
  "nbf": 1646203826,
  "ver": "1.0",
  "iss": "https://b2clivedemo.b2clogin.com/3a29c594-12be-476f-bb6f-0a787b348639/v2.0/",
  "sub": "31ee61cf-521f-4132-8e19-11a3b6562666",
  "aud": "cfaf887b-a9db-4b44-ac47-5efff4e2902c",
  "acr": "B2C_1A_Demo_SignUp_SignIn_ConditionalAccess_WhatIf",
  "nonce": "defaultNonce",
  "iat": 1646203826,
  "auth_time": 1646203826,
  "signInName": "emily@contoso.com",
  "conditionalAccessClaimCollection": [
    "mfa"
  ],
  "CAChallengeIsMfa": true,
  "CAChallengeIsChgPwd": false,
  "CAChallengeIsBlock": false,
  "IP-Address": "43.214.54.97",
  "trustFrameworkPolicy": "B2C_1A_Demo_SignUp_SignIn_ConditionalAccess_WhatIf",
  "tid": "3a29c594-12be-476f-bb6f-0a787b348639"
}
```

## Important notes

This sample policy uses the starter-pack [B2C_1A_TrustFrameworkExtensions](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/blob/master/SocialAndLocalAccounts/TrustFrameworkExtensions.xml) as a base policy. 

The claim used inside the *IsMfaRegisteredCT* claims transformation must not be empty to ensure *IsMfaRegistered* evaluates to *True*. If it evaluates to *False*, the evaluate always results in a *Block* grant type. Also, you can use any valid claim within *IsMfaRegisteredCT* which carries a MFA value (Email or Phone). 

```XML
 <ClaimsTransformation Id="IsMfaRegisteredCT" TransformationMethod="DoesClaimExist">
  <InputClaims>
    <InputClaim ClaimTypeReferenceId="strongAuthenticationEmailAddress" TransformationClaimType="inputClaim" />
  </InputClaims>
  <OutputClaims>
    <OutputClaim ClaimTypeReferenceId="IsMfaRegistered" TransformationClaimType="outputClaim" />
  </OutputClaims>
 </ClaimsTransformation>
```

## How it works

The policy contains the following components:

### Claims definition

- Evaluation and remediation input:
  - **AuthenticationMethodsUsed** - The list of authentication methods used.
  - **AuthenticationMethodUsed** - The authentication method used. Possible values: *Password*, and *OneTimePasscode*.
  - **IsFederated** - Indicates whether the user authenticated via an external identity provider, or a local account.
  - **estsRequestId** - The request identifier that is created during non-interactive login for local accounts. This identifier should be set as an output claim from the technical profile that represents non-interactive login.

- Evaluation output:
  - **conditionalAccessClaimCollection** - The list of claims which are the result of conditional access evaluation phase
  - **IsMfaRegistered** - Indicates whether the user registered for MFA. If not, the user will be blocked in MFA is required.
  - **CAChallengeIsMfa** - Indicates whether the conditional access evaluation allows MFA as a remediation method
  - **CAChallengeIsChgPwd** - Indicates whether the conditional access evaluation allows password change as a remediation method. This method is not supported yet.
  - **CAChallengeIsBlock** - Indicates whether the conditional access evaluation to terminate the sign-in flow. 
- Miscellaneous 
  - **isLocalAccountSignUp** -  indicates whether the user is sign-in or sign-up. If the user is sign-up, the policy doesn't run the conditional access evaluation.
  - **ConditionalAccessStatus** - not in used
  - **responseMsg** - contains the conditional access clock message.
   
### Claims transformations

- **CreatePasswordAuthenticationMethodClaim** - Sets the value of the AuthenticationMethodUsed claim to *Password*.
- **CreateOneTimePasscodeAuthenticationMethodClaim** - Sets the value of the AuthenticationMethodUsed claim to *OneTimePasscode*.
- **AddToAuthenticationMethodsUsed** - Adds the AuthenticationMethodUsed value to the AuthenticationMethodsUsed string collection claim type.
- **IsMfaRegisteredCT** - Checks if the user registered for MFA.
- **SetCAChallengeIsMfa** - Checks if the conditional access returns *mfa*, MFA as a remediation method.
- **SetCAChallengeIsChgPwd** - Checks if the conditional access returns *chg_pwd*, password change as a remediation method.
- **SetCAChallengeIsBlock** - Checks if the conditional access returns *block*. In this case the sign-in flow should be terminated.
- **ShowBlockPage** - Presents the "Due to recent activity associated with your account, your login has been temporarily blocked." message.

### Technical profiles 

- **SelfAsserted-LocalAccountSignIn-Email** - Update the starter-pack with initiate claims to indicate how the user sign-in. Using the *isFederated* claim to *false*, and *AuthenticationMethodsUsed* to *Password*. This indicates that the current user sign-in with a username and password method.
- **ConditionalAccessEvaluation** - Conditional access evaluation.
- **GenerateCAClaimFlags** - Based on the ConditionalAccessEvaluation output claims, this technical profile set the flags of the required remediation methods, such as FAM, block, or password rest. 
- **ConditionalAccessRemediation** - Conditional access remediation.

### User journeys

- **SignUpOrSignInWithCA** - Sign-in with local or social account, with conditional access, including following steps:
  1. Start the sign-up or sign-in flow. 
  1. After the user completes the sing-in or sign-up, the conditional access evaluation is executed.  
  1. If block is the recommended remediation method, terminate the sign-in flow.
  1. Run the conditional access remediation.
  1. Issue an access token
  
- **SignUpOrSignInWithCA-WhatIf** - Sign-in with local or social account, with only the evaluation step. This flow allows you to check the user experience without being blocked or required MFA. Use this user journey for testing purposes only.

The *IdentityProviderSelection_SignUpSignIn* sub journey is similar to the starter pack [SignUpOrSignIn user journey](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/blob/master/SocialAndLocalAccounts/TrustFrameworkBase.xml#L1031). You can copy your own user journey, and add the orchestration steps to the IdentityProviderSelection_SignUpSignIn sub journey. Make sure to remove the last orchestrion step which issues the JWT token. 
