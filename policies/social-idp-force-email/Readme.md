# Azure AD B2C social identity provider force email verification

When sign-in with social account, in some scenarios,  the identity provider doesn't share the email address. This sample demonstrates how to force the user to provide and validate email address.

Examples when identity provider may not share the user's email address:

* A user sign-in with facebook account via phone number (instead of email address).
* During the sign-in (on the consent page), user choose not to share the email address with Azure AD B2C.
* Some of the social identity providers don't share the email address.

In those scenarios, the policy checks if the email address is empty. If yes, user is asked to provide and validate the email address.

## Live demo

To check out the user experience when the email address is not shared with Azure AD B2C, follow these steps:

1. [Sign-up or sign-in](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SignUp_SignIn_FederationForceEmail/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) with the *B2C_1A_Demo_SignUp_SignIn_FederationForceEmail* policy. Select to sign-in with *Facebook*. The app that is used in the demo environment configured not to share the email address. So, after you completed the sign-up or sign-in with the Facebook account, you will be asked to provide and verify your email address.
1. [Sign-up or sign-in](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SignUp_SignIn_FederationForceEmail/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) with the *B2C_1A_Demo_SignUp_SignIn_FederationForceEmail* policy. This time, select to sign-in with *Google*. The Google identity provider by default shares the email address (unless in the Google consent page you select not to share). In this case you will not be asked to verify your email.

## Notes

* You can remove the orchestration step's precondition and force the user to validate the email address, even if the social identity provider shares the email address.
* This sample policy is based on [SocialAndLocalAccounts starter pack](../../../SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files.
