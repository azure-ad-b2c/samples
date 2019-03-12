# Azure AD B2C: Username discovery

This example shows how to discover username by email address. The assumption is that the username and the companion email address are nuniqu. The email address can't be shared between multiple users. Using this policy, when a user sign-up and provides the username and the email address. After the email verification process, Azure AD B2C persists both username and email address to the signInNames collection. Follwing is an example of such account:

```JSON
"signInNames": [
    {
        "type": "emailAddress",
        "value": "david@contoso.com"
    },
    {
        "type": "username",
        "value": "david"
    }
]
```

**RestoreUsername** policy, calls the **SelfAsserted-UsernameDiscovery**  technical profile that: 
1. Collects the email address form the user
1. Run the **AAD-UserReadUsingEmailAddress-NoError** validation technical profile. This technical profile inlueds the **AAD-UserReadUsingEmailAddress** technical profile, but doesn't rise an error if an account could not be found for the provided email address. We Don't' raise the error bacuse we want to prevent users guessing email accounts (email harvesting attacks)
1. If the  **AAD-UserReadUsingEmailAddress-NoError** returns the **LogonName** claim, then next validation technical profile **REST-RestoreUsername** is executed. This technical profile receives the email address and the username, and sends the restore username email.

To merge the policy into your policy, you need:
1. Add the **userMessage** claim type and set the display name
1. Add the **GetPasswordResetUserMessage** claims transformation and set the value
1. Add the **SelfAsserted-UsernameDiscovery**, **SelfAsserted-UserMessage**, **AAD-UserReadUsingEmailAddress-NoError**, and **REST-RestoreUsername** technical profiles
1. Add the user journey **RestoreUsername** 
1  Add the **B2C_1A_LogonName_RestoreUsername** policy

## Test the policy by using Run Now
1. From Azure Portal select **Azure AD B2C Settings**, and then select **Identity Experience Framework**.
1. Create a local account with username
1. Open **B2C_1A_LogonName_RestoreUsername**, the relying party (RP) custom policy that you uploaded, and then select **Run now**.
1. Type the email address to the account you created and click continue
1. Open you mailbox and check if the username sent to you is the one associated to the email you provided
1. Run the policy again, this time provide any email that is not associated to any account. Make sure you don't get any error message.

## Disclaimer
The sample is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The sample (Azure AD B2C policy and any companion code) is provided AS IS without warranty of any kind.

> Note:  This sample policy is based on logon with username policy. All changes are marked with **Demo:** comment inside the policy XML files. Make the nessacery changes in the **Demo action required** sections.
