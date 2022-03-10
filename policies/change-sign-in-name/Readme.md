

# Azure AD B2C: Change local account sign-in name (email address)

When sign-in with local account, a user may want to change the sign-in name (email address). This sample policy demonstrates how to allow user to provide and validate new email address, and store the new email address to Azure Active Directory user account. After user change the email address, on the next login, user needs to provide the new email address as sign-in name.

[![Change local account sign-in name video.](media/link-to-youtube.png)](https://youtu.be/7NOtT3B_OVM)

## Live demo

To checkout the user experience of this policy, follow these steps:

1. If you don't have an account, [create a local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_signup_signin/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with your email address.

1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_ChangeSignInName/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_ChangeSignInName* policy to change the email address.

1. Provide and verify your new email address and select *continue*.

1. [Sign in with your new email adress](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_signup_signin/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login)

## How it works

The solution is base on new *B2C_1A_ChangeSignInName* relying party policy. The policy invokes *ChangeSignInName* user journey which:

1. Asks the user to sign-in with a local account
1. Reads the user data from the Azure Active Directory
1. Collects and validate the new email address
1. Persists the new sign-in name to the directory

![An image of the Email verification screen.](media/email-verificaton.png)

To change user's email address, call the *B2C_1A_ChangeSignInName* relying party policy.