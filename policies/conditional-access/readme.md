# Azure AD B2C sign-in with Conditional access

Azure Active Directory (Azure AD) Conditional Access is the tool used by Azure AD B2C to bring signals together, make decisions, and enforce organizational policies. Automating risk assessment with policy conditions means risky sign-ins are at once identified and remediated or blocked. For more information, see [Define a Conditional Access technical profile in an Azure Active Directory B2C custom policy](https://docs.microsoft.com/azure/active-directory-b2c/conditional-access-technical-profile).

## Overview

The `SignUpOrSigninWithCA` policy uses the starterpack [B2C_1A_TrustFrameworkExtensions](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/blob/master/SocialAndLocalAccounts/TrustFrameworkExtensions.xml) as a base policy. 

### Known issues

In this version the policy block high risk sign-in. MFA is not included as a remediation in this policy.  

## Solution artifacts

The policy contains the following components:

### Claims definition

- Evaluation and remediation input:
  - **AuthenticationMethodsUsed** - The list of authentication methods used.
  - **AuthenticationMethodUsed** - The authentication method used. Possible values: `Password`, and `OneTimePasscode`.
  - **IsFederated** - Indicates whether the user authenticated via an external identity provider, or a local account.

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

- **CreatePasswordAuthenticationMethodClaim** - Sets the value of the AuthenticationMethodUsed claim to `Password`.
- **CreateOneTimePasscodeAuthenticationMethodClaim** - Sets the value of the AuthenticationMethodUsed claim to `OneTimePasscode`.
- **AddToAuthenticationMethodsUsed** - Adds the AuthenticationMethodUsed value to the AuthenticationMethodsUsed string collection claim type.
- **IsMfaRegisteredCT** - Checks if the user registered for MFA.
- **SetCAChallengeIsMfa** - Checks if the conditional access returns `mfa`, MFA as a remediation method.
- **SetCAChallengeIsChgPwd** - Checks if the conditional access returns `chg_pwd`, password change as a remediation method.
- **SetCAChallengeIsBlock** - Checks if the conditional access returns `block`. In this case the sign-in flow should be terminated.
- **ShowBlockPage** - Presents the "Due to recent activity associated with your account, your login has been temporarily blocked." message.

### Technical profiles 

- **SelfAsserted-LocalAccountSignin-Email** - Update the starterpack with initiate claims to indicate how the user sign-in. Using the `isFederated` claim to `false`, and `AuthenticationMethodsUsed` to `Password`. This indicates that the current user sign-in with a username and password method.
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

The `IdentityProviderSelection_SignUpSignIn` sub journey is similar to the starter pack [SignUpOrSignIn user journey](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/blob/master/SocialAndLocalAccounts/TrustFrameworkBase.xml#L1031). You can copy your own user journey, and add the orchestration steps to the IdentityProviderSelection_SignUpSignIn sub journey. Make sure to remove the last orchestrion step which issues the JWT token. 