# Password Reset - Via either Email or Phone verification

Demonstrate how to use a [display control](https://docs.microsoft.com/azure/active-directory-b2c/display-controls) to conditionally process on the users decision to verify their account via Email or SMS verification code.

## Live demo

To test the policy, follow these steps:

1. If you don't have an account, [create a local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SignUp_SignIn_PasswordReset_EmailOrPhone/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with your email address, using the *B2C_1A_Demo_SignUp_SignIn_PasswordReset_EmailOrPhone* policy. This policy is based on the [SocialAndLocalAccountsWithMfa](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/main/SocialAndLocalAccountsWithMfa) that required users to register their phone number.
1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_PasswordReset_EmailOrPhone/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_PasswordReset_EmailOrPhone* policy to reset the password.
1. Perform the following test:
    1. Type your email and select *Continue*. Then, select to send the verification code to your email. Provide the verification code that you received and select continue to reset the password. 
    1. Repeat the process, but this time select to send the verification code to your phone.

## Prerequisites

- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

## How it works

1. Read the users profile once they provide their email address. This will provide their phone number used to Sign Up.
1. Use a displayControl to display the user a radio box selection on whether to verify their account via Email or Phone.
1. The displayControl uses preconditions on the *SendCode* and *VerifyCode* actions to control the *ValidationClaimsExchangeTechnicalProfile* based on the users selection on whether to use phone or email to verify their account. That decision is held in the claim *mfaType*, which acts as the radio box.


## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Notes

This sample policy is based on [SocialAndLocalAccountsWithMFA starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
