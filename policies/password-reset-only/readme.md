# Azure AD B2C: Password Reset only

This policy demonstrates how to prevent the user from issuing an access token after resetting the password.

## Live demo

To test the policy, follow these steps:

1. If you don't have an account, [create a local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_signup_signin/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with your email address.
1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_PasswordReset_Only/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the B2C_1A_Demo_PasswordReset_Only policy to reset the password.
1. Complete the password reset process.
1. After you reset the password Azure AD B2C shows you a message that your password successfully saved and will not allow you to continue to issue an access token.

## How it works

The policy contains additional orchestration step *SelfAssertedPasswordResetUserMessage* that calls the *SelfAsserted-PasswordResetUserMessage* self-asserted technical profile. The [self-asserted technical profile](https://docs.microsoft.com/azure/active-directory-b2c/self-asserted-technical-profile#metadata) uses the metadata to hide the continue and cancel buttons. Also the technical profile renders the `userMessage` paragraph claim type.

Note: You should add an HTML link to the self-asserted technical profile's content definition. This link redirect the user back to your application. The application should initiate new authorization request, asking the user to sign-in with the new password.

The SelfAsserted-PasswordResetUserMessage must contain  at least one output claim. If you remove the output claim, Azure AD B2C will not show the self asserted page. 

To merge the policy into your policy, you need:

1. Add the *userMessage* claim type and set the display name.
1. Add the *SelfAsserted-PasswordResetUserMessage* technical profile.
1. Add the extra orchestration step *SelfAssertedPasswordResetUserMessage* before the last orchestration step.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

