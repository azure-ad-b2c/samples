# A B2C IEF Custom Policy - Sign up and Password reset with banned password list

For scenarios where you need to implement a sign up and password reset/change flow where the user cannot use a new password that is part of a banned password list.

## Live demo

To test the policy, follow these steps:

1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_signup_signin_BannedPasswordList/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_signup_signin_BannedPasswordList* policy to create a local account with your email address. Complete the sing-up process and perform the following test:
    1. Provide one of the passwords on the banned list (within *passwordToMatchBanList* claims transformation). For example, `Password!`, or `P@ssw0rd!`. Select *Continue*, and you should see an error message that the password is too weak.
    1. Type another password that is not on the banned list. This time you should be able to complete the process successfully.

1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_PasswordReset_BannedPasswordList/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_PasswordReset_BannedPasswordList* policy to reset the password. Complete the password reset process and perform the same tests as you run during the sign-up.


## Prerequisites

- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

## How it works

When the user signs up, the *LocalAccountSignUpWithLogonEmail_Custom* technical profile uses the *CheckPassword* validation technical profile to validate that the new password is not on the banned list. The *CheckPassword* technical profile uses multiple claims transformations to determine if the new password exists on the banned password list and throws an error if it does.

1. *passwordToMatchBanList* - Uses the [LookupValue Claims transformation](https://docs.microsoft.com/azure/active-directory-b2c/string-transformations#lookupvalue) to determine if the new password matches a password on the banned list. This will return *bannedPassword=true* if the value matches. Where there is no match, a claim is not output.
2. *CheckBannedPasswordValue* - Compares *bannedPassword* with dummy claim which has been set to *true*. If  *bannedPassword* matches *dummyTrue*, then we have a banned password. The *banned* claim is output as *true*.
If they don't match, then the *banned* claim is output as *false*. This is to counter the fact that the lookup claimsTransformation does not output a claim when there isn't a match.
3. *AssertBannedPasswordFalse* - Compares the value of *banned* claim to *false*. If *banned* claim is false, then no error is thrown, the user did not use a banned password. If *banned* claim is *true*, then it will not match to *false* and therefore the user used a password from the banned password list.

The *LocalAccountSignUpWithLogonEmail_Custom* technical profile uses the metadata item *UserMessageIfClaimsTransformationBooleanValueIsNotEqual* to throw an error when the password is on the banned password list. It is referring to when the *AssertBannedPasswordFalse* is not able to assert that the value of *banned* is false, ie that the password was on the banned list.

During Password Reset, a similar flow occurs. When *LocalAccountWritePasswordUsingObjectId* technical profile is called, a validation technical profile called *CheckPassword* is invoked. This runs through the same claims transformations as described above for the sign up process.

*AAD-UserWritePasswordUsingObjectId* technical profile is only run based on the [precondition](https://docs.microsoft.com/azure/active-directory-b2c/validation-technical-profile) that the password was not on the banned password list (*banned=false*).

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).