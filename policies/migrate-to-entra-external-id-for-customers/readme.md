# Upgrade ready architecture: AAD B2C to Entra External Id for Customers

## Summary

This sample will use AAD B2C as the journey orchestrator, whilst creating and authenticating users in the Entra External Id tenant. This makes it easier in the future to move apps to Entra External Id without disruption to your users. This sample performs sign up/in with MFA using Azure AD B2C, whilst maintaining user profiles in the Entra External Id tenant.

## How it works

Users are sent to the AAD B2C authentiaction endpoint. An Azure function orchestrates all Read/Write operations to the Entra External Id directory.

![High level design](media/high-level-design.png)

![Sign up with MFA](media/signup.png)

![Sign in with MFA](media/signin.png)

## How to set it up

### Create application registrations
1. Create an Application registration in the Entra External Id tenant, named **RopcFromB2C**. Choose Native App. Copy the AppId/ClientId
1. In the Authentication menu, enable **Allow public client**
1. In the manifest, set

1. Create an Application registration in the Entra External Id tenant, named **GraphCallsFromB2CTenant**. Choose Web App. Copy the AppId/ClientId
1. Under API permissions, add MS Graph API **Application** permissions: `User.ReadWrite.All` and `UserAuthenticationMethod.ReadWrite.All`
1. Under **Certificates & secrets**, generate a new secret. Copy this secret to be placed in the Azure function code

### Modifications in Azure Function
1. Change all instances of `your-tenant-id-guid` to your tenant guid or name eg `contoso.onmicrosoft.com`
1. Change all instances of `your-clientId-RopcFromB2C` to the AppId of the **RopcFromB2C** App registration
1. Change all instances of `your-client-id-to-call-graph` to the AppId of the **GraphCallsFromB2CTenant** App registration
1. Change all instances of `your-client-secret` to the secret generated on the **GraphCallsFromB2CTenant** App registration

### Host the Azure function
1. In any production scenario, enable authentication in the Azure function. Eg, OAuth.

### Modifications in policy file
1. Change all instances of `your-tenant` to your tenant name, eg: `contoso.onmicrosoft.com`
1. Change all instances of `your-api-endpoint` to your Azure function endpoint.
1. In any production scenario, enable authentication in the REST API technical profiles.

## Testing
Scenarios to test:
1. Sign up via this AAD B2C custom policy with an account that does not already exist
1. Sign in via this AAD B2C custom policy with an account that does exist
1. Sign up via this AAD B2C custom policy with an account that does already exist
1. Sign in via this AAD B2C custom policy with an account that does not exist
1. Assign the user to a group/role in Entra External Id tenant, then perform a sign in
1. Create a User Flow in Entra External Id tenant, and perform a Sign In via Entra External Id tenant with an account that was created via this AAD B2C custom policy
1. Sign in via this AAD B2C custom policy with an account that was created using the User Flow from Entra External Id tenant
1. View the User Profile after Sign Up in the Entra External Id tenant. Ensure the `Authentication Methods` menu is correctly populated with the users phone number visible, after sign up via this AAD B2C custom policy