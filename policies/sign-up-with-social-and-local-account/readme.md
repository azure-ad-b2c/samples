# Azure AD B2C: Sing-up with social and local account

With Azure AD B2C a user can have multiple identities. Sign-in with local account, and link a social account to an existing local account. This Azure AD B2C sample demonstrates how to create a single account with social and local identities.

This is scenario may help in case you provide the user the ability to sign-in with your organization account (such as ADFS, Azure AD, or Salcefoce account), but also want to allow users to continue sign-in with their local account after they leave the organization. For example, a user may not belong anymore to the organization, but he/she should still be able to access the application. With this demo, when user sign-in with Facebook account, you ask the user to provide and verify the email address and password. When B2C creates the account, the account is created with:
- **signInName** containing the email address for the local identity
- **password** for local identity
- **userIdentities** with the social or enterprise id, for the social identity
- User profile, such as first name, last name, and display name
 
Following is an example of such account:

```JSON

{
    "displayName": "Yoel Hor",
    "givenName": "Yoel",
    "jobTitle": null,
    "surname": "Hor",
    "signInNames": [
        {
            "type": "emailAddress",
            "value": "your@contoso.com"
        }
    ],
    "userIdentities": [
        {
            "issuer": "facebook.com",
            "issuerUserId": "RTEwAjQwBTM7Mjg4NERv"
        }
    ]
}
```

## Test the policy by using Run Now
1. From Azure Portal select **Azure AD B2C Settings**, and then select **Identity Experience Framework**.
1. Open **B2C_1A_signup_signin**, the relying party (RP) custom policy that you uploaded, and then select **Run now**.
1. Sign-in with Facebook. Make sure you sign-in with an account you never sign-in before (or delete your Azure AD B2C Facebook account)
1. After you sign-in with your Facebook account. Type and verify your email address. Type your password and your profile, and click **Continue**
1. Sign-out form your application, or open new browser in private mode (incognito)
1. Open **B2C_1A_signup_signin** again, the relying party (RP) custom policy that you uploaded, and then select **Run now**.
1. Sign-in with the local identity (email address) you specified
1. You can also check the account you crated by using [Azure AD Graph Explorer](https://graphexplorer.azurewebsites.net/).

## Disclaimer
The sample is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The sample (Azure AD B2C policy and any companion code) is provided AS IS without warranty of any kind.

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.
