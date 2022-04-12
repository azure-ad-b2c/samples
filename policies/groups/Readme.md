# Azure AD B2C: Security groups

Demonstrates how to returns the user security groups. The policy first acquire an access token to call the MS Graph API. Then it calls the [transitiveMemberOf](https://docs.microsoft.com/graph/api/user-list-transitivememberof?view=graph-rest-1.0&tabs=http) command to get groups, directory roles that the user is a member of. This API request is transitive, and will also return all groups the user is a nested member of.

The following XML demonstrates how MS Graph returns the list of groups to Azure AD B2C custom policy. 

```xml
{
    "@odata.context": "https://graph.microsoft.com/v1.0/$metadata#directoryObjects(id,displayName)",
    "value": [
        {
            "@odata.type": "#microsoft.graph.group",
            "id": "1de4788d-441f-4544-9dab-8d788c29c61c",
            "displayName": "Admins"
        },
        {
            "@odata.type": "#microsoft.graph.group",
            "id": "b2a1fcfc-e165-4b9b-bb08-a7e3e7feefa0",
            "displayName": "Teachers"
        }
    ]
}
```

The first 3 groups are return by the *REST-GetGroupMembership* technical profile:

```xml
<OutputClaims>
  <OutputClaim ClaimTypeReferenceId="group_1" PartnerClaimType="value[0].displayName" />
  <OutputClaim ClaimTypeReferenceId="group_2" PartnerClaimType="value[1].displayName" />
  <OutputClaim ClaimTypeReferenceId="group_3" PartnerClaimType="value[2].displayName" />
</OutputClaims>
```

You can change the code to get the group Id instead of the display name, using the following XML:

```xml
<OutputClaims>
  <OutputClaim ClaimTypeReferenceId="group_1" PartnerClaimType="value[0].id" />
  <OutputClaim ClaimTypeReferenceId="group_2" PartnerClaimType="value[1].id" />
  <OutputClaim ClaimTypeReferenceId="group_3" PartnerClaimType="value[2].id" />
</OutputClaims>
```

## Register the MS Graph application

Before the policy can interact with the Microsoft Graph API, you need to create an application registration in your Azure AD B2C tenant and grants the required API permissions.

1. Sign in to the [Azure portal](https://portal.azure.com).
1. Make sure you're using the directory that contains your Azure AD B2C tenant. Select the **Directories + subscriptions** icon in the portal toolbar.
1. On the **Portal settings | Directories + subscriptions** page, find your Azure AD B2C directory in the **Directory name** list, and then select **Switch**.
1. In the Azure portal, search for and select **Azure AD B2C**.
1. Select **App registrations**, and then select **New registration**.
1. Enter a **Name** for the application. For example, *managementapp1*.
1. Select **Accounts in this organizational directory only**.
1. Under **Permissions**, clear the *Grant admin consent to openid and offline_access permissions* check box.
1. Select **Register**.
1. Record the **Application (client) ID** that appears on the application overview page. You use this value in a later step.

### Grant API access

1. Under **Manage**, select **API permissions**.
1. Under **Configured permissions**, select **Add a permission**.
1. Select the **Microsoft APIs** tab, then select **Microsoft Graph**.
1. Select **Application permissions**.
1. Expand the appropriate permission group and select the check box of **Directory.Read.All**.
1. Select **Add permissions**. As directed, wait a few minutes before proceeding to the next step.
1. Select **Grant admin consent for (your tenant name)**.
1. If you are not currently signed-in with Global Administrator account, sign in with an account in your Azure AD B2C tenant that's been assigned at least the *Cloud application administrator* role and then select **Grant admin consent for (your tenant name)**.
1. Select **Refresh**, and then verify that "Granted for ..." appears under **Status**. It might take a few minutes for the permissions to propagate.

## Create client secret

1. Under **Manage**, select **Certificates & secrets**.
1. Select **New client secret**.
1. Enter a description for the client secret in the **Description** box. For example, *clientsecret1*.
1. Under **Expires**, select a duration for which the secret is valid, and then select **Add**.
1. Record the secret's **Value**. You use this value for configuration in a later step.

## Configure your policy

Follow this steps to configure your policy:

1. Replace the `yourtenant` with your tenant name. Make user to replace it also in the *REST-AcquireAccessTokenForGraph* technical profile.
1. Create a policy key name `B2C_1A_MSGraphClientId` with the client ID your created earlier in this article.
1. Create a policy key name `B2C_1A_MSGraphClientSecret` with the client secret your created earlier in this article.