# Azure AD B2C: Password Reset only

This example policy prevents the user form issuing an access token after resetting the password. The policy contains additional orchestration step **SelfAssertedPasswordResetUserMessage** that calls the **SelfAsserted-PasswordResetUserMessage** self-asserted technical profile. The [self-asserted technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/self-asserted-technical-profile#metadata) uses the metadata to hide the **continue** and **cancel** buttons. Also the technical profile renders the **userMessage** paragraph claim type. Use the **GetPasswordResetUserMessage** claims transformation to set the value of the error message.

Note: You should add an HTML link to the self-asserted technical porifle's content definition. This link redirect the user back to your application. The application should initiate new authorization request, asking the user to sign-in with the new password.

It’s important that the self-asserted technical profile SelfAsserted-PasswordResetUserMessage contains at least one output claim. if you try to remove the claim from the technical profile, Azure AD B2C will NOT display the self asserted page. Follow the Demo comments inside the extension policy. 

To merge the policy into your policy, you need:
1.	Add the **userMessage** claim type and set the display name
2.	Add the **GetPasswordResetUserMessage** claims transformation and set the value
3.	Add the **SelfAsserted-PasswordResetUserMessage** technical profile
4.	Add the extra orchestration step **SelfAssertedPasswordResetUserMessage** before the last orchestration step

## Test the policy by using Run Now
1. From Azure Portal select **Azure AD B2C Settings**, and then select **Identity Experience Framework**.
1. Open **B2C_1A_PasswordReset_Only**, the relying party (RP) custom policy that you uploaded, and then select **Run now**.
1. Verify your email address
1. Reset your password
1. Make sure Azure AD B2C renders the password reset message and not issuing an access token

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.
