# Microsoft B2C IDP Initiated Login

Documenting up my findings as the Microsoft supplied SAML IDP Initiated Login is terrible. Especially if you want to have an encrypted SAML Assertion.

https://learn.microsoft.com/en-us/azure/active-directory-b2c/saml-service-provider-options?#configure-idp-initiated-flow
https://github.com/azure-ad-b2c/saml-sp/blob/master/saml-rp-spec.md

# Use Case

I wanted to login to a SAML IDP Initiated login using B2C. Having an OIDC Entra ID as the authentication source as my plan was to pass an optional query parameter to B2C to be incldued in the SAML Assertion.
Also I didn't want to depend on any B2C base policies so everything is in a single policy.

The SAML Assertion needed
- The preferred_username aka the email address of the user in Entra ID
- Redirect directly to Entra ID to login or SSO for seamless login journey.
- Pass Entra ID Groups assigned to registered app granting access to B2C to check group membership. 
- Able to support an optional custom claim that could be passed as a query parameter
- To be signed by a PFX private key that was uploaded as a Policy Key that is trusted by the SP AssertionConsumerService
- To be encrypted by a Public Key provided by the SP AssertionConsumerService

## Login Flow

1. Click link to Policy `/generic/login` with optional ?optional_claim=claim_value
2. Redirect to Entra ID for the user to be challenged for the Entra ID / Azure AD login
3. Redirect back to B2C with OIDC code on successful login
4. B2C to Entra ID exchange code for `id_token`
5. Sign the assertion using the private key uploaded to Policy Keys
6. Encrypt the assertion using the public key from the SP embedded into the Policy xml. This means an App isn't required nor is the EntityId required on the URL
7. Redirect user to AssertionConsumerService with signed and encrypted assertion to login

# Required Information

A checklist of things you will need.

## Entra B2C
- B2C Tenancy Name - Typically `b2ctenantname.onmicrosoft.com` so the Tenancy name would be `b2ctenantname`

## Entra ID
- Entra Tenant ID - This will always be a GUID like `bad00bad-cafe-cafe-cafe-badbadbadbad` and you can find it here https://portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/Overview
- Entra Tenant Name - Typically `aadtenantname.onmicrosoft.com`

# Setup Entra ID

First step is to setup a OIDC registered application in Entra ID / Azure AD. Follow these steps as they are correct as all you need to do is setup an App registration and then generate a Client Secret.

https://learn.microsoft.com/en-us/azure/active-directory-b2c/identity-provider-azure-ad-single-tenant?pivots=b2c-custom-policy

Go to: https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationsListBlade

Name: Entra B2C OIDC Application
Supported account types: Accounts in this organizational directory only (Single tenant) - As you don't want it to be used outside your tenant
Redirect URI:
 - Web - https://{B2C Tenancy Name}.b2clogin.com/{B2C Tenancy Name}.onmicrosoft.com/oauth2/authresp

In overview:
Copy the `Application (client) ID` - This will be a GUID and is the **`client_id`**
The `Directory (tenant) ID` is your Entra ID Tenant ID if you didn't have it already.

Select Certificates & secrets
New Client Secret
Description: Entra B2C Client Secret YYYY-MM-DD (This is useful to set when you created the client secret)
Expires: 730 Days (Set it to as long as possible unless you want to roll the client secret more frequently)

Copy the `Value` by clicking the `Copy to clipboard` button to the right of the Value. This is your **`client_secret`**.
The Secret ID GUID isn't required, so make sure you copy the Value field which will be a long alphanumeric random string with ~ or similar values in it.

Lastly enable sending Group Membership via the `groups` claim. It will always be the Group Entra ID GUID. Under the Application -> Manage -> Token configuration -> click on Add groups claim. Select `Groups assigned to the application (recommended for large enterprise companies to avoid exceeding the limit on the number of groups a token can emit)` and feel free to set the ID to sAMAccountName, but it doesn't change the value of the `id_token` `groups` claim.

# Setup B2C

The rest of the work is on B2C so you don't need to return to the Entra ID Tenancy

B2C Landing page: https://portal.azure.com/?feature.tokencaching=true&feature.internalgraphapiversion=true#blade/Microsoft_AAD_B2CAdmin/TenantManagementMenuBlade

## Import B2C Policy Keys

There are two Policy Keys you need to import. The `client_secret` and the SAML Signing Certificate private key

Policies -> Identity Experience Framework -> Manage -> Policy keys

### Add the Client Secret

Update yyyymmdd to the date the secret expires.

Options: Manual
Name: B2C_1A_IDPEntraIDClientSecretyyyy
Secret: Paste in the `client_secret` from above.
Don't tick on Activation Date
Set expiration date: 720 Days from now or for however long you set the Client Secret Expiry
Key Usage: Signature

### Add the SAML Private Key for message signing

Update yyyymmdd to the date the certificate expires.

Options: Upload
Name: B2C_1A_IDPLocalEntitySigningCertificateyyyy
File upload: Select the PFX or P12 PKCS12 file
Password: The PFX Password
Set expiration date: Hopefully the cert is a 3 year cert but set the expiry 10 days before it's due to expire.

## Modify Policy XML

| Setting | Current Value in XML to update |
| - | - | 
| Tenant Name | `yourtenant.onmicrosoft.com` |
| Entra ID Tenant GUID | `bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb` |
| Entra ID Application `client_id` GUID | `cccccccc-cccc-cccc-cccc-cccccccccccc` |
| Entra ID Application `client_secret` string. Upload to Policy Keys including year of expiry | `B2C_1A_IDPEntraIDClientSecretyyyy` |
| Entra ID Group GUID to grant access | `aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`|
| Local Entity ID for signing | `https://localentityid.com`|
| Local Private Key for signing in Policy Key | `B2C_1A_IDPLocalEntitySigningCertificate`|
| ApplicationInsights InstrumentationKey | `dddddddd-dddd-dddd-dddd-dddddddddddd`|
| Remote Service Provider entityID | `https://destinationentityid.com/entityidurl`|
| Remote Service Provider Public Key for Assertion encryption | `MII..pem.base64.certificate.goes.here.U=`|
| Remote Service Provider AssertionConsumerService Login URL | `https://destinationentityid.com/samlp/sso/assertionconsumer`|

A few important notes
 - Group access is checked based on if `InEntraIDGroup` is True.
 - Assertion token encryption uses the public key from the SP. This is optional and the whole KeyDescriptor can be removed if it's not required.
 - A few claims have been pulled from the base example template. This was so the whole template could stand on it's own without any dependancies.
 