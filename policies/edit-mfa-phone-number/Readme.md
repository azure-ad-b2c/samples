# Azure AD B2C: Edit MFA phone number

This sample policy demonstrates how to allow user to provide and validate new MFA phone number. After user change the MFA phone number, on the next login, user needs to provide the new phone number instead of the old one.

The solution is based on new `B2C_1A_Edit_MFA` relying party policy. The policy invokes `EditMFAPhoneNumber` user journey which:
1. Asks the user to sign-in with local account or social account
1. Reads the user data from the Azure Active Directory
1. Asks the user to validate the existing phone number
1. Provides and validate new phone number
1. Persists the new phone number to the directory

To change user's MFA phone number, call the `B2C_1A_Edit_MFA` relying party policy.

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

> Note:  This sample policy is based on [SocialAndLocalAccountsWithMfa starter pack](../../../SocialAndLocalAccountsWithMfa). Changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.
