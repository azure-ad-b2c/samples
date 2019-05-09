# Impersonation Flow for Azure AD B2C
Impersonation flows are common for many business needs. We see this for users that manage other users either through functions within an application or services such as customer support. Whatever the reason may be, it can be easily achieved using Azure AD B2C flexible Identity Experience Framework with a few simple changes. Below are the minimum items required for this scenario. 

## Common Scenarios
1. **Customer Service Representative** requires assisting a user and requires to log into the application on the behalf of the user. The CSR wants to see exactly what the user sees.
2. **Financial Investor** manages on the behalf of their customers. As such, a bank or an investment firm may require these individuals to log into their service/application to manage their customers information.

### Impersonation flow
When you run the impersonation policy, you first need to sign-in with your own credentials. On the second page, provide the email address of the user you want to impersonate. B2C checks whether the user exists in the directory. If yes, B2C issues and access token with the email address of the impersonated user `impersonatedUser`.

![Impersonation flow](media/flow.png)

1. User (a customer service representative) sign-in with local or social account
1. After the user sign-in, the `AAD-UserReadUsingObjectId` technical profile reads the `extension_can_impersonate` extension attribute from the directory
1. The next orchestration step (step number 6) checks whether the user is allowed to impersonate (extension_can_impersonate value is equesl to 1). If not, `SelfAsserted-ErrorMessage` technical profile displays an error, preventing the user from issuing an access token.
1. Next orchestration step asks the user to provide the email address of the user to be impersonated, by calling the `SelfAsserted-TargetEmailExchange` technical profile. This technical profile also checks if the impersonated user exists in the directory, and return the user's email address
1. On the last step Azure AD B2C issues the access token, with the claims specified in the RelyingParty XML element, including the impersonated user's email address - `impersonatedUser` claim type

## Authorization
The authorization is based on the value of the `extension_can_impersonate` claim type. If the value is `1`, the user is allowed to impersonate. You should use Azure AD Graph API to set the value of the extension attribute. For example, send an HTTP `PATCH` request to update the user's profile:

```HTTP
PATCH https://graph.windows.net/your-tenant.onmicrosoft.com/users/user-id
{
    "extension_clientId_can_impersonate": "1"
}
```

Replase:
- **your-tenant**, with your tenant name
- **user-id**, with the user object id, or UPN
- **clientId**, with the extension attribute application client id

For more infomation, see:
- [Azure AD B2C: Use the Azure AD Graph API](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-devquickstarts-graph-dotnet)
- [Use custom attributes in a custom profile edit policy](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom)
- [Extend Azure Active Directory Schema using Graph API](https://blogs.msdn.microsoft.com/aadgraphteam/2014/03/05/extend-azure-active-directory-schema-using-graph-api-preview/)

You can also use other authorization method, such as calling a REST API to check is user is allowed to impersonate.

## Disclaimer
The sample is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The sample (Azure AD B2C policy and any companion code) is provided AS IS without warranty of any kind.

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the nessacery changes in the **Sample action required** sections. 

A standard to manage impersonation flows is currently in development and has not been finalized. This sample does not reflect the final product or guidance. You should monitor on this progress and plan to make changes to your design as standards of the industry changes. You can read more [here](https://tools.ietf.org/html/draft-ietf-oauth-token-exchange-10 "OAuth 2.0 Token Exchange draft-ietf-oauth-token-exchange-10").