# A B2C IEF Custom Policy - Password reset without being able to use the last password

This policy demonstrates how to prevent the user from using the last password when changing the password. For scenarios where you need to implement a password reset/change flow where the user cannot use their currently set password.

## Live demo

To test the policy, follow these steps:

1. If you don't have an account, [create a local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_signup_signin/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with your email address.
1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_PasswordReset_NotLastPassword/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_PasswordReset_NotLastPassword* policy to reset the password
1. Perform the following tests:

    1. Enter valid current password, and the same password for new password and re-enter password, then select *continue*. You should get an error message that the old and new password are identical.
    1. Enter invalid current password. You should get an error message that the current password is invalid.
    1. Enter valid current password, and a different password for new password and re-enter password. You should be able to completed the process successfully.
    1. Try to [sign-in](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_signup_signin/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with your updated password.

## How it works

This sample presents the user with a screen to enter their current password, new password and re-enter password field using the *LocalAccountWritePasswordUsingObjectId* technical profile. This technical profile calls validation technical profiles in the following order:

1. *login-NonInteractive-PasswordChange* to validate the current password.
1. *ComparePasswords* to ensure that the new password is not the same as the current password. It invokes the claims transformation in the following order:
    1. *CheckPasswordEquivalence* - does a string comparison operation against the new password and the current password. If they match, it outputs `True`, otherwise `False`.
    1. *AssertSamePasswordIsFalse* - asserts that the boolean from step 1 came back as `False`, indicating the password was different.
1. Finally the password is written using *AAD-UserWritePasswordUsingObjectId* technical profile.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).