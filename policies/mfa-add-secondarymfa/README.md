# AAD B2C IEF/Custom policy to allow users to store and select two phone numbers at SignIn or SignUp

This sample policy stores two strong phone numbers in AAD B2C securely:

 1. The user has forgotten or lost one of their phone numbers and still need access.
 2. Select between any of the two phone numbers at the time of signIn.

## Live demo

To check out the experience with secondary MFA phone number, follow these steps:

1. If you don't have an account, [create a local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_signup_signin_SecondaryPhone/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) using the *B2C_1A_Demo_signup_signin_SecondaryPhone* policy. With this flow, you will be asked to create an account, add verify the primary phone, then select whether you want to register another one.
1. If you have an account, [sign-in](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_signup_signin_SecondaryPhone/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) using the *B2C_1A_Demo_signup_signin_SecondaryPhone* policy. Of the first time you will be asked to provide the second phone number. Otherwise (after you registered both phone numbers), will be asked to verify the preferred one. For more information read the following user flows in this article.

## Sign-Up flow (new account)

1. Collect signUp attributes and first MFA phone number.
2. Prompt the user if they want to store an additional phone number for MFA.
    1. If user selected "Yes" - Ask for another phone number.
        1. Validate the other phone number via text or phone call.
        1. Store the secondary phone number.
        1. Issue token.
    1. If user selected "No".
        1. Issue token.

## Sign-In flow (existing account)

1. User enters username and password as the first step.

    1. User does not have two phone numbers on file.

        1. Gets prompted for first MFA and completes MFA.
        1. Gets prompted to store another MFA phone number and follows step from 2. from previous section.

    1. User has two phone numbers on file:

        1. User gets prompted for MFA with an option to select between any two phone numbers.

## Notes

This sample policy is based on [SocialAndLocalAccountsWithMfa starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).