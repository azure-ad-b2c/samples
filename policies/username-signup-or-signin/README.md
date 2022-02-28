# A B2C IEF Custom Policy which uses Usernames as the sign in identifier

## Live demo

To checkout the user experience of this policy, follow these steps:

1. If you don't have an account, [create a local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SUSI_Username/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with the *B2C_1A_Demo_SUSI_Username* policy.

1. Provide a username and verify your  email address and select *continue*.

1. [Sign in with your username](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_signup_signin/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login)

1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_ProfileEdit_Username/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_ProfileEdit_Username* policy to edit your profile using your  username.

1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_PasswordReset_Username/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_PasswordReset_Username* policy to reset your password using your username.

## Scenario

For scenarios where you would like users to sign up and sign in with Usernames rather than Emails.

At Sign Up, the user is asked to validate an email address. This email address will be associated to the user account by writing to a protected attribute: *strongAuthenticationEmailAddress*. This attribute can only be read or written to by the B2C policy execution.

At Sign In, the Username provided is used as a lookup against all SignInNames that are present on user objects stored in the the directory. This is achieved by sending the paramater *nca=1* when making the authentication request via the *login-NonInteractive* technical profile in *TrustFrameworkBase*.

At Password Reset, the user will be asked to confirm the email address. It will be sent a TOTP and validated against the email address stored at sign up in the *strongAuthenticationEmailAddress* attribute. This ensures the user owns this Username.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).