# A B2C IEF Custom Policy which links a Federated login against a pre-created Local Account

## Disclaimer
The sample policy is developed and managed by the open-source community in GitHub. This policy is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The policy is provided AS IS without warranty of any kind.

## Scenario
This scenario is helpful when requiring to pre-create accounts for users who will use a federated logon option.
This sample allows a user to be created up front, adding any extension attributes to the user, ready for their first logon. This could include any Id's required to login to the Application that already exist from a system that you maybe migrating from. It may also include doing any group assignments up front.

The user would only need to be sent a link to the B2C logon page, where they will be sent to their federated provider by use of the [domain_hint](https://docs.microsoft.com/en-us/azure/active-directory-b2c/direct-signin#redirect-sign-in-to-a-social-provider) parameter to automatically choose the IdP. Once the user authenticates at their federated IdP, B2C will use the claims to lookup the user in the B2C directory. If the user is found, the AlternativeSecuirtyId is written to the account and allows the user to logon with the pre-created account.

## How it works

1.	Create the account using PowerShell (Graph API) with the required attributes and the extension attribute, requiresMigration=true
2.	Login through the B2C Sign In link via AAD as the federated IdP
3.	B2C takes the UPN from the AAD id_token

    a. B2C finds a B2C account with a signInName.emailAddress that matches the AAD UPN (AAD-UserReadUsingUPN)

    b. B2C reads the objectId and extension_requiresMigration

    c. B2C throws an error if the account does not exist

4.	B2C will check if extension_requiresMigration = true (AAD-MergeAccount), if so:

    a. Write the AltSecId to the account

    b.	Change extension_requiresMigration to false
5.	Account is now linked
6.	Any other extension attributes can be read by adding output claims to AAD-UserReadUsingAlternativeSecurityId for AAD users.
