# A B2C IEF Custom Policy which links a Federated login against a pre-created Local Account

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
This scenario is helpful when requiring to pre-create accounts for users who will use a federated logon option.
This sample allows a user to be created up front, adding any extension attributes to the user, ready for their first logon. This could include any Id's required to login to the Application that already exist from a system that you maybe migrating from. It may also include doing any group assignments up front.

The user would only need to be sent a link to the B2C logon page, where they will be sent to their federated provider by use of the [domain_hint](https://docs.microsoft.com/en-us/azure/active-directory-b2c/direct-signin#redirect-sign-in-to-a-social-provider) parameter to automatically choose the IdP. Once the user authenticates at their federated IdP, B2C will use the claims to lookup the user in the B2C directory. If the user is found, the AlternativeSecuirtyId is written to the account and allows the user to logon with the pre-created account.

## Prerequisites
- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance on [storing the extension properties](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [adding the application objectID](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.


## How it works

1.	Create the account using PowerShell (Graph API) with the required attributes and the extension attribute, requiresMigrationBool=true

```powershell
$ClientID      = "2d6e17d4-aaaa-aaaa-aaaa-8626cead8bdb"                # Should be a ~35 character string insert your info here
$ClientSecret  = ""     # Should be a ~35 character string insert your info here
$loginURL      = "https://login.microsoftonline.com"
$tenant = "contoso.onmicrosoft.com"      # For example, contoso.onmicrosoft.com
$resource = "https://graph.microsoft.com"

$user = @"
{
  "displayName": "Link Me",
  "passwordProfile": {
    "password": "Password!&*^806DSAdf060%£ASDFd0ad",
    "forceChangePasswordNextSignIn": false
  },
  "passwordPolicies": "DisablePasswordExpiration",
  "identities": [
    {
      "signInType": "oidToLink",
      "issuer": "contoso.onmicrosoft.com",
      "issuerAssignedId": "90847c2a-e29d-4d2f-9f54-c5b4d3f26471"
    }
  ]
}
"@

$body       = @{grant_type="client_credentials";client_id=$ClientID;client_secret=$ClientSecret;resource=$resource}
$oauth      = Invoke-RestMethod -Method Post -Uri $loginURL/$tenant/oauth2/token -Body $body

$authHeader = @{"Authorization"= $oauth.access_token;"Content-Type"="application/json";"ContentLength"=$body.length }
$url = "$resource/beta/users"
Invoke-WebRequest -Headers $authHeader -Uri $url -Method Post -Body $user
```

The `signInNames.oidToLink` is the identifier you will use from the federated IdP to match to the pre-created B2C user account. In this case the `objectId` (oid) is written to the `signInNames` collection.
And the `objectId` is obtained from the IdP's `oid` claim at runtime to do the matching.

You can create any mapping you like, for example the users email from the external IdP. 

```xml
<TechnicalProfile Id="O365AADProfile">
...
    <OutputClaim ClaimTypeReferenceId="issuerUserId" PartnerClaimType="oid"/>
...
```

2.	Login through the B2C Sign In link via AAD as the federated IdP
3.	B2C takes the `oid` from the the federated IdP id_token

    a. B2C finds a B2C account with a `signInNames.oidToLink` that matches the AAD ObjectId using the `AAD-FindB2CUserWithAADOid` technical profile. B2C throws an error if the account does not exist. The account should exist as part of the pre-creation.
    ```xml
    <Item Key="RaiseErrorIfClaimsPrincipalDoesNotExist">true</Item>
    ```

    b. B2C reads the `objectId` and `extension_requiresMigration` of the B2C user object

    c. B2C then makes sure that another federated identity is not already in the directory using `AAD-UserReadUsingUserIdentityToLink-NoError` and presents an error with `SelfAsserted-Error-DupeAccount` if one already exists. This is accomplished by generating the unique userIdentities object as part of the federated login technical profile and checking that it is not already present in the B2C directory. For debug, the error screen displays the offending B2C user objectId. This user needs to be deleted.

    c. B2C will check if `extension_requiresMigrationBool = true`. And then the `userIdentities` collection is merged with the pre-created account using `AAD-UserUpdateWithUserIdentities` technical profile and changes `extension_requiresMigrationBool` to `false`.

5.	Account is now linked
6.	Any other extension attributes can be read by adding output claims to `AAD-UserReadUsingUserIdentity-NoError`` for the federated IdP users.
