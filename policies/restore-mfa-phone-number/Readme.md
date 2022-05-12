# Azure AD B2C: Restore MFA phone number

This sample policy demonstrates how to allow user to change the phone number in case they lost their phone. The user first needs to validate their email address. Then, provide and verify the new phone number.  After user change the MFA phone number, on the next login, user needs to provide the new phone number instead of the old one.

## Live demo

To check out the user experience of the TOTP multi-factor authenticator, follow these steps:

1. [Sign-up or sign-in with MFA](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_SignUp_SignIn_WithMFA/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login). Note, this step is not part of this sample.
1. After you have an account that is registered with MFA, run the [B2C_1A_Demo_RestorePhoneNumber](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_RestorePhoneNumber/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) policy to restore the phone number you entered in the first step.
1. After you changed the phone number for your account, [sign-in with MFA](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_SignUp_SignIn_WithMFA/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login). This time you will be asked to verify the new phone number. Note, this step is not part of this sample.

## How it works

The solution is based on new *B2C_1A_Demo_RestorePhoneNumber* relying party policy located in the [ProfileEdit_PhoneNumber.xml file](policy/ProfileEdit_PhoneNumber.xml). The policy invokes *EditMFAPhoneNumber* user journey located in the [TrustFrameworkExtensions_EditPhoneNumber.xml file](policy/TrustFrameworkExtensions_EditPhoneNumber.xml) which:

1. Asks the user to provider and verify their email address
1. Reads the user data from the Azure Active Directory
1. Provides and validate new phone number
1. Persists the new phone number to the directory

To change user's MFA phone number, call the *B2C_1A_Demo_RestorePhoneNumber* relying party policy.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

> Note:  This sample policy is based on [SocialAndLocalAccountsWithMfa starter pack](../../../SocialAndLocalAccountsWithMfa). Changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.
