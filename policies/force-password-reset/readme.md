# Force password reset flow

As an administrator, you can reset a user's password if the user forgets their password or you would like to force them to reset the password. In this policy sample, you'll learn how to force a password reset in these scenarios. It's the custom policy described at [Set up a force password reset flow in Azure Active Directory B2C](https://docs.microsoft.com/azure/active-directory-b2c/force-password-reset).

## Live demo

To checkout the user experience of this policy, follow these steps:

1. If you don't have an account, [create a local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_signup_signin/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with your email address.

1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SignUp_SignIn_ForcePasswordReset/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_SignUp_SignIn_ForcePasswordReset* policy to check the value of the *forceChangePasswordNextLogin* claim. At this point it should be false. So, you don't need to reset the password.

1. At this step you need to simulate a force password reset flow. Since you don't have access to the live demo tenant. We have prepared an helper policy that changes the value of the *forceChangePasswordNextLogin* to true. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_Helper_SignUp_SignIn_ForcePasswordReset/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_**Helper**_SignUp_SignIn_ForcePasswordReset* policy. Sign-up or sign-in and provide a new password. This policy will simulate an admin changing the password for the user. Sign-in with the same account from step 1, and provide a new password. After you run this step, that account will be force to reset the password.

1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SignUp_SignIn_ForcePasswordReset/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_SignUp_SignIn_ForcePasswordReset* policy again. It will check the value of the *forceChangePasswordNextLogin* claim. This time it should be true and you must reset the password.

## How it works

- Claims:
  - *continueOnPasswordExpiration* - input claims of the login-NonInteractive technical profile. It instructs Azure AD B2C to continue upon password expiration.
  - *forceChangePasswordNextLogin* - output claims of the login-NonInteractive technical profile. It indicates whether the user needs to reset the password. If the value of this claim is true, other claims aren't return by the login-NonInteractive technical profile.

- Technical profiles:
  -*login-NonInteractive* - The technical profile that validates the credentials. It customizes the one in the [base policy](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/blob/main/SocialAndLocalAccounts/TrustFrameworkBase.xml), adding the input and output claims.
  - *SelfAsserted-LocalAccountSignIn-Email* - The technical profile that renders the sign-up and sign-in page. It customizes the one in the [base policy](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/blob/main/SocialAndLocalAccounts/TrustFrameworkBase.xml), adding a call to the *AAD-UserReadUsingSignInName* technical profile to get the user attributes.
  - *AAD-UserReadUsingSignInName*  - If the forceChangePasswordNextLogin claim is true, other claims don't return. So, it gets the user objectId by the sign-in name.
  - *SelfAsserted-ForcePasswordReset-ExpiredPassword* - Renders the password reset page. It calls the *AAD-UserWritePasswordUsingObjectId-ResetNextLogin* technical profile which persists the password to the use's profile.

- User journey
  - *SignUpOrSignIn_Custom* - the precondition checks the value of the forceChangePasswordNextLogin claim and renders the password reset step. 

## For more information

- Check out the [Force password reset](https://github.com/azure-ad-b2c/unit-tests/tree/main/technical-profiles/login-NonInteractive#force-password-reset) unit test.
