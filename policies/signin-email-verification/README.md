# Email Multi-factor authentication

For scenarios where you would like users to validate their email via verification code on all sign-ins.

## Live demo

To checkout the user experience of this policy, follow these steps:

1. If you don't have an account, [create a local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_TrustFrameworkExtensions_EmailMFA/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with your email address.

1. [Sign-in](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_SignUp_SignIn_EmailMFA/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) with the *B2C_1A_SignUp_SignIn_EmailMFA* policy. Provide your credentials and select *Sign-in*. Azure AD B2C will ask you to verify your email (as the second factor authentication). 

## How it works

At Sign In, the email address authenticated with is copied into a read only attribute *readOnlyEmail* via an input claim transformation in the *EmailVerifyOnSignIn* technical profile.

The *readOnlyEmail* claim is passed as an input claim to the *EmailVerifyOnSignIn* self asserted technical profile to validate the email address via verification code. This is made possible by using *PartnerClaimType="Verified.Email"* in the output claims section.

The user journey only calls the *EmailVerifyOnSignIn* self asserted technical profile if the user is not a new user. This bypasses this particular step if the user is signing up.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Notes

This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 