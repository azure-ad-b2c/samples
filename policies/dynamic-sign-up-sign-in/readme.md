# Dynamically sign in or sign up the user

This sample allows dynamically detecting whether a user can sign in or sign up. The user enters their email and selects *Sign-in*. If the account exists, the user is asked to verify their password. Otherwise, if the account does not exist, the user goes through a sign up flow.

## Live demo

To test the policy, complete the following steps:

1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_signup_signin_Dynamic/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_signup_signin_Dynamic* policy. In the sign-up or sign-in page provide an account that doesn't exists in the directory. Select *Sign-in* and Azure AD B2C will take you to the sign-up page (the email is read only).
1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_signup_signin_Dynamic/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_signup_signin_Dynamic* policy again. In the sign-up or sign-in page provide the email address that you used in the previous step. Select *Sign-in* and Azure AD B2C will ask you to provide the password.
1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_signup_signin_Dynamic/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_signup_signin_Dynamic* policy again. In the sign-up or sign-in page select the *Sign-up now* link. Azure Azure AD B2C will take you to the sign-up page where you provide and verify your email address.

## Prerequisites

- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. This sample can be [Quick deployed](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=dynamic-sign-up-sign-in) after the pre requisites are automated.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

## Notes

This sample policy is based on [Social and LocalAccounts with MFA starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections.
