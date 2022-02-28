# Sign In and Sign Up with Username or Email

This sample combines the UX of both the Email and Username based journeys. The sample policy is based on [SocialAndLocalAccounts starter pack](../../../SocialAndLocalAccounts). 

## Live demo

To check out the user experience of using **Email**, follow these steps:

1. [Create an account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SignUp_UsernameOrEmail/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) using the *B2C_1A_Demo_SignUp_UsernameOrEmail* policy. For the sign-in name, type a valid email address, such as *emily@contoso.com*. Select *Continue* to complete the sign-up process. In the next page you will be asked to verify your email address (you can't change the email), and fill out the password and your name.
1. [Sign-in](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SignIn_UsernameOrEmail/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) using the *B2C_1A_Demo_SignIn_UsernameOrEmail* policy, with the Email you registered. Note, the user experience for this policy is identical to the username.
1. [Reset your password](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_PasswordReset_UsernameOrEmail/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) using the *B2C_1A_Demo_PasswordReset_UsernameOrEmail* policy. In the first page type your email, then select *continue*. On the next page you will be asked to verify your email.

To check out the user experience of using **Username**, follow these steps:

1. [Create an account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SignUp_UsernameOrEmail/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) using the *B2C_1A_Demo_SignUp_UsernameOrEmail* policy. For the sign-in name, type a valid email address, such as *emily*. Select *Continue* to complete the sign-up process. In the next page you will be asked to provide and verify your email address and fill out the password and your name. The username is presented on the top of the sign-up page (you can change it).
1. [Sign-in](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SignIn_UsernameOrEmail/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) using the *B2C_1A_Demo_SignIn_UsernameOrEmail* policy, with the Email you registered. Note, the user experience for this policy is identical to the Email.
1. [Reset your password](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_PasswordReset_UsernameOrEmail/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) using the *B2C_1A_Demo_PasswordReset_UsernameOrEmail* policy. In the first page type your username, then select *continue*. On the next page you will be asked to type your email address and verify it.

## Prerequisites

- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).


## How it works

### Sign Up

During sign up, the user is presented with a page to enter their Username. Upon submitting this field, the *regexAnalysisUsername* validation technical profile will call the claims transformation *isEmail*.

*isEmail* claims transformation uses a regex to return a boolean *isEmailBoolean* if it detects an email format.

*isEmailBoolean* then is used to determine if the user will go through the Username based sign up or Email sign up.
During Email based sign up, the user only needs to verify the email and provide any further details.
During Username based sign up, the user will be created with the username as the identifier, and the verified email stored in the *strongAuthenticationEmail* field.

### Sign In

During sign in, the user is presented with a page to enter their Username and Password as normal. B2C will lookup the account with either and authenticate the user as normal.

### Password Reset

During password reset the user is presented with a page to enter their Username. Upon submitting this field, the *regexAnalysisUsername* validation technical profile will call the claims transformation *isEmail*.

*isEmail* claims transformation uses a regex to return a boolean *isEmailBoolean* if it detects an email format.

*isEmailBoolean* then is used to determine if the user will go through the Username based password reset or Email password reset.
During Email based password reset, the user only verifies the email and after can change the password.
During Username based password reset, the user will verify the email, and if the email matches that which was stored in *strongAuthenticationEmail* during sign up, then the user can reset the password.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].

If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).

To provide product feedback, visit the [Azure Active Directory B2C Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).