# Azure AD B2C: Edit MFA phone number

This sample policy demonstrates how to allow user to provide and validate new MFA phone number. After user change the MFA phone number, on the next login, user needs to provide the new phone number instead of the old one.

The solution is based on new `B2C_1A_Edit_MFA` relying party policy. The policy invokes `EditMFAPhoneNumber` user journey which:
1. Asks the user to sign-in with local account or social account
1. Reads the user data from the Azure Active Directory
1. Asks the user to validate the existing phone number
1. Provides and validate new phone number
1. Persists the new phone number to the directory

To change user's MFA phone number, call the `B2C_1A_Edit_MFA` relying party policy.

## Disclaimer
The sample is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The sample (Azure AD B2C policy and any companion code) is provided AS IS without warranty of any kind.

> Note:  This sample policy is based on [SocialAndLocalAccountsWithMfa starter pack](../../../SocialAndLocalAccountsWithMfa). Changes are marked with **Demo:** comment inside the policy XML files. Make the nessacery changes in the **Demo action required** sections.
