# Azure AD B2C social identity provider force email verification

When sign-in with social account, in some scenarios,  the identity provider doesn't share the email address. This sample demonstrates how to force the user to provide and validate email address.

Examples when identity provider may not share the user's email address:
* A user sign-in with facebook account via phone number (instead of email address)
* During the sign-in (on the consent page), user choose not to share the email address with Azure AD B2C
* Some of the social identity providers don't share the email address

In those scenarios, the policy checks if the email address is empty. If yes, user is asked to provide and validate the email address.

![Email verification](media/email-verificaton.png)

You can remove the orchestration step's precondition and force the user to validate the email address, even if the social identity provider shares the email address.

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](../../../SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files.
