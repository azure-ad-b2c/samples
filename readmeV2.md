# Azure Active Directory B2C: Custom CIAM User Journeys

In this repo, you will find samples for several enhanced Azure AD B2C Custom CIAM User Journeys.

## Getting started

- See our Custom Policy overview [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview).

- See our [Azure AD B2C Wiki articles](https://azure-ad-b2c.github.io/azureadb2ccommunity.io/docs/custom-policy-concepts/) here to help walkthrough the custom policy components.

- See our Custom Policy Schema reference [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-reference-trustframeworks-defined-ief-custom).

## Prerequisites
- You can automate the pre requisites by visiting this [site](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to create an Azure AD B2C directory, see the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to setup your AAD B2C environment for Custom Policies [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

## Sample scenarios

Samples are available for the following categories
- [Password Management](#password-management)
- [General Security](#general-security)
- [User Experience](#user-experience)
- [Terms of Use / Consent](#terms-of-useconsent)
- [Passwordless](#passwordless)
- [Multi Factor](#multi-factor)
- [Account Linking](#account-linking)
- [Identity Providers](#identity-providers)
- [User Interface](#user-interface)
- [Data Residency](#data-residency)
- [User Migration](#user-migration)
- [Web Test](#web-test)
- [CI / CD](#cicd)

## Password management

|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Password reset via Email or Phone verification](policies/pwd-reset-via-email-or-phone)   |This demonstrates how to verify a user via Email or SMS on a single screen.   | [Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=pwd-reset-via-email-or-phone)|
|[Force password reset](policies/force-password-reset)   |As an administrator, you can reset a user's password if the user forgets their password. Or you would like to force them to reset the password. In this policy sample, you'll learn how to force a password reset in these scenarios.   |[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=force-password-reset)|
|[Force password reset first logon](policies/force-password-reset-first-logon)|Demonstrates how to force a user to reset their password on the first logon.  |[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=force-password-reset-first-logon)|
|[Force password after 90 days](policies/force-password-reset-after-90-days)|Demonstrates how to force a user to reset their password after 90 days from the last time user set their password.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=force-password-reset-after-90-days)|
|[Password reset only](policies/password-reset-only)|This example policy prevents issuing an access token to the user after resetting their password.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=password-reset-only)|
|[Sign-up and sign-in with embedded password reset](policies/embedded-password-reset)|This policy demonstrates how to embed the password reset flow a part of the sign-up or sign-in policy without the AADB2C90118 error message.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=embedded-password-reset)|
|[Password Reset with Phone Number](policies/password-reset-with-phone-number) |An example policy to reset a users password using Phone Number (SMS or Phone Call).|NEED POLICY FIX|
|[Password reset without the ability to use the last password](policies/password-reset-not-last-password)|For scenarios where you need to implement a password reset/change flow where the user cannot use their currently set password.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=password-reset-not-last-password)|
|[Banned password list](policies/banned-password-list-no-API) |For scenarios where you need to implement a sign up and password reset/change flow where the user cannot use a new password that is part of a banned password list. This sample does not use an API.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=banned-password-list-no-API)|
|[Password Reset OTP only sent if Email is registered](policies/pwd-reset-email-exists) |Demonstrate how to use a displayControl to send One-Time-Passcodes to users only if the email is registered against a user in the directory.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=pwd-reset-email-exists)|
|[Password history](policies/password-history) |This policy enables the storing and checking of a user's previous set of passwords in order to prevent them from using a previous password during a Password Reset flow. |NA|


## General security

|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Revoke Azure AD B2C session cookies](policies/revoke-sso-sessions) |Demonstrates how to revoke the the single sign on cookies after a refresh token has been revoked.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=revoke-sso-sessions)|
|[Google Captcha on Sign In](policies/captcha-integration)|An example set of policies which integrate Google Captcha into the sign in journey.|NA|
|[Disable and lockout an account after a period of inactivity](policies/disable-inactive-account)|For scenarios where you need to prevent users logging into the application after a set number of days. The account will also be disabled at the time of the users login attempt in the case the user logs in after the time period.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=disable-inactive-account)|
|[Restrict B2C Policy to specific App Registration](policies/allow-list-applications)| Only permits certain application registrations to call certain B2C policy Id's.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=allow-list-applications)|
|[Impersonation Flow](policies/impersonation)|For scenarios where you require one user to impersonate another user. This is common for support desk or delegated administration of a user in an application or service. It is recommended to always issue the token of the original authenticated user and append additional information about the targeted impersonated user as part of the auth flow|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=impersonation)|
|[Social identity provider force email verification](policies/social-idp-force-email)|When a user signs in with a social account, in some scenarios, the identity provider doesn't share the email address. This sample demonstrates how to force the user to provide and validate an email address.|NA|
|[Sign-in with social identity provider and force email uniqueness](policies/force-unique-email-across-social-identities)|Demonstrates how to force a social account user to provide and validate their email address, and also checks that there is no other account with the same email address.|NA|
|[Preventing logon for Social or External IdP Accounts when Disabled in AAD B2C](policies/disable-social-account-from-logon)|For scenarios where you would like to prevent logons via Social or External IdPs when the account has been disabled in Azure AD B2C.|NA|
|[Relying party app Role-Based Access Control (RBAC)](policies/relying-party-rbac)|Enables fine-grained access management for your relying party applications. Using RBAC, you can grant only the amount of access that users need to perform their jobs in your application. This sample policy (along with the REST API service) demonstrates how to read user's group membership, add the groups to JWT token and also prevent users from sign-in if they aren't members of one of predefined security groups.|NA|
|[Sign-in with Conditional access](policies/conditional-access)|Azure Active Directory (Azure AD) Conditional Access is the tool used by Azure AD B2C to bring signals together, make decisions, and enforce organizational policies. Automating risk assessment with policy conditions means risky sign-ins are at once identified and remediated or blocked.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=conditional-access)
|[Allow/Deny based on Hostname](policies/conditional-access)|This sample provides an example of how to block access to particular B2C policy based on the [Hostname] of the request, e.g. allow requests made to the policy using login.contoso.com but block foo.b2clogin.com. Useful when using custom domain(s) with Azure AD B2C.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=check-host-name)
|[Call center validation](policies/store-three-letters-of-the-password)|A call center uses Azure AD B2C to validate a customer phoning in. To do this, the call center takes three characters from the password and asks the customer calling in to provide the three characters plus some other known facts as part of the authentication process.|NA|

## User Experience

|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Split Sign-up into separate steps for email verification and account creation](policies/split-email-verification-and-signup)|When you don't want to use the default Sign-up page which shows both email verification and user registration controls on the same page at once. This sample splits the default sign-up behavior into two separate steps. First step performs Email Verification only, avoiding all other default fields related to users registration. Second step (if email verification was successful) takes the users to a new screen where they can actually create their accounts. This uses Azure AD to send out emails, no separate email provider integrations needed.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=split-email-verification-and-signup)|
|[Sign In and Sign Up with Username or Email](policies/username-or-email)|This sample combines the UX of both the Email and Username based journeys.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=username-or-email)|
|[Local account change sign-in name email address](policies/change-sign-in-name)|During sign-in with a local account, a user may want to change the sign-in name (email address). This sample policy demonstrates how to allow a user to provide and validate a new email address, and store the new email address to the Azure Active Directory user account. After the user changes their email address, subsequent logins require the use of the new email address.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=change-sign-in-name)|
|[Username discovery](policies/username-discovery)|This example shows how to discover a username by email address. It's useful when a user has forgotten their username and remembers only their email address.|NA/NEED POLICY REWORK|
|[Sign-in with Home Realm Discovery and Default IdP](policies/default-home-realm-discovery)|Demonstrates how to implement a sign in journey, where the user is automatically directed to their federated identity provider based off of their email domain. And for users who arrive with an unknown domain, they are redirected to a default identity provider.|NA|
|[Email delivered account redemption link](policies/sign-in-with-email)|This sample demonstrates how to allow the user to sign up to a web application by providing their email which sends the user a magic link to complete their account creation to their email.|NA|
|[Sign-in with a magic link](policies/sign-in-with-magic-link)|This sample demonstrates how a user can sign in to your web application by sending them a sign-in link. A magic link can be used to pre-populate user information, or accelerate the user through the user journey.|NA|
|[Username based journey](policies/username-signup-or-signin) | For scenarios where you would like users to sign up and sign in with Usernames rather than Emails.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=username-signup-or-signin)|
|[Dynamic identity provider selection](policies/idps-filter)|Demonstrates how to dynamically filter the list of social identity providers rendered to the user based on the requests application ID. In the following screenshot user can select from the list of identity providers, such as Facebook, Google+ and Amazon. With Azure AD B2C custom policies, you can configure the technical profiles to be displayed based a claim's value. The claim value contains the list of identity providers to be rendered.|NA|
|[Home Realm Discovery page](policies/home-realm-discovery-modern)|Demonstrates how to create a home realm discovery page. On the sign-in page, the user provides their sign-in email address and clicks continue. B2C checks the domain portion of the sign-in email address. If the domain name is `contoso.com` the user is redirected to Contoso.com Azure AD to complete the sign-in. Otherwise the user continues the sign-in with username and password. In both cases (AAD B2C local account and AAD account), the user does not need to retype the user name.|NA|
|[Delete my account](policies/delete-my-account)|Demonstrates how to delete a local or social account from the directory|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=delete-my-account)|
|[Integrate REST API claims exchanges and input validation](https://github.com/azure-ad-b2c/rest-api)|A sample .Net core web API, demonstrates the use of [Restful technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/restful-technical-profile) in user journey's orchestration step and as a [validation technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/validation-technical-profile).|NA|
|[sign-up or sign-in policy with a deep link to sign-up page](policies/sign-up-deep-link)|Adds a direct link to the sign-up page. A relying party application can include a query string parameter that takes the user directly to the sign-up page.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=sign-up-deep-link)|
|[Allow sign up from specific email domains](policies/sign-up-domain-whitelist)|This policy demonstrates how to validate the email address domain name against a list of allowed domains.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=sign-up-domain-whitelist)|

## Terms of Use/Consent
|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Provide consent UI to API scopes](policies/service-consent)|For scenarios where you provide a plug and play service to other partners. When the user chooses to use your service through a partner application, the user must login with their account with your service, and consent to various scopes which allow your service to share information with the partner application.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=service-consent)|
|[Sign Up and Sign In with dynamic 'Terms of Use' prompt](policies/sign-in-sign-up-versioned-tou)|Demonstrates how to incorporate a TOU or T&Cs into your user journey with the ability for users to be prompted to re-consent when the TOU/T&Cs change.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=sign-in-sign-up-versioned-tou)|
|[Azure AD B2C Invitation](policies/invite)|This sample console app demonstrates how to send a sign-up email invitation. After you sent the invitation, the user clicks on the **Confirm account** link, which opens the sign-up page (without the need to validate the email again). Use this approach when you need to create the users account beforehand, while allowing the user to choose the password on initial sign in. This approach is better than creating an account via Graph API and sending the password to the user via some communication means.|NA|

## Passwordless
|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Password-less sign-in with email verification](policies/passwordless-email)|Password-less authentication is a type of authentication where user doesn't need to sign-in with their password. This is commonly used in B2C scenarios where users use your application infrequently and tend to forget their password. This sample policy demonstrates how to allow user to sign-in, simply by providing and verifying the sign-in email address using OTP code (one time password).|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=passwordless-email)|
|[Login with Phone Number](policies/signup-signin-with-phone-number)|An example set of policies for password-less login via Phone Number (SMS or Phone Call).|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=signup-signin-with-phone-number)|


## Multi factor
|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Custom email verification - DisplayControls](policies/custom-email-verifcation-displaycontrol) |Allows you to send your own custom email verification email during sign-up or password reset user journey's. The is a working example of the sample reference on the Microsoft B2C documentation site - [Custom email verification in Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-email)|NA|
|[Custom SMS provider - DisplayControls](policies/custom-sms-displaycontrol)| Integrate a custom SMS provider in Azure Active Directory B2C (Azure AD B2C) to customized SMS' to users that perform multi factor authentication to your application. By using DisplayControls (currently in preview) and a third-party SMS provider, you can use your own contextualised SMS message, custom Phone Number, as well as support localization and custom one-time password (OTP) settings.|NA|
|[Email Verification at Sign In](policies/signin-email-verification)|For scenarios where you would like users to validate their email via OTP on every sign in.|
|[Sign-in with FIDO](policies/fido2)|Demonstrates how to sign-in with a FIDO authenticator (as a first factor authentication). This policy use the WebAuthn standard to register new credential and sign-in with FIDO credential.|NA|
|[Integrate Twilio Verify API for PSD2 SCA](policies/twilio-mfa-psd2) |The following sample guides you through integrating Azure AD B2C authentication with Twilio Verify API to enable your organization to meet PSD2 SCA requirements.|NA|
|[Edit MFA phone number](policies/edit-mfa-phone-number) | Demonstrates how to allow user to provide and validate a new MFA phone number. After the user changes their MFA phone number, on the next login, the user needs to provide the new phone number instead of the old one.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=edit-mfa-phone-number)|
|[TOTP multi-factor authentication](policies/custom-mfa-totp) |Custom MFA solution, based on TOTP code. Allowing users to sign-in with Microsoft or Google authenticator apps.|NA|
|[Sign In With Authenticator](policies/sign-in-with-authenticator) |This is a sample to show how you can create a B2C Custom Policy to signin with Authenticator Apps to B2C. It is related to the custom-mfa-totp sample, which shows how to use the Authenticator app as MFA.|NA|
|[Authy App multi-factor authentication](policies/custom-mfa-authy-app) |Custom MFA solution, based on Authy App (push notification). Allowing users to sign-in with Twilio Auth App (authenticator apps).|NA|
|[MFA with either Phone (Call/SMS) or Email verification](policies/mfa-email-or-phone)| Allow the user to do MFA by either Phone (Call/SMS) or Email verification, with the ability to change this preference via Profile Edit.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=mfa-email-or-phone)|
|[Add & Select 2 MFA phone numbers at SignIn/Signup](policies/mfa-add-secondarymfa)| Demonstrates how to store two phone numbers in a secure manner in B2C and choose between any two at signIn. The flow prompts the user to store a secondary phone if only one phone number is one file. Once the two numbers are stored as part of SignUp or SignIn the user is given a choice to select between the two phones for their MFA on subsequent signIns.|NEED SAMPLE REWORK|
|[MFA after timeout or IP change](policies/mfa-absolute-timeout-and-ip-change-trigger)| A policy which forces the user to do MFA on 3 conditions: The user has newly signed up, the user has not done MFA in the last X seconds, the user is logging in from a different IP than they last logged in from.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=mfa-absolute-timeout-and-ip-change-trigger)|
|[Unknown Devices MFA - device fingerprinting](policies/mfa-unknown-devices)|Demonstrates how to detect unknown devices which might be required to prompt MFA as illustrated in this particular sample or send email to the user signing in from unknown device.|NEED SAMPLE REWORK|


## Account linking
|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Account linkage](policies/account-linkage-unified)|(new version, one policy for both link and unlink) - With Azure AD B2C an account can have multiple identities, local (username and password) or social/enterprise identity (such as Facebook or AAD). This Azure AD B2C sample demonstrates how to link and unlink existing Azure AD B2C account to a social identity. Unified policy for link and unlink.|NA|
|[Account linkage](policies/account-linkage)|(a policy for link and another policy for unlink.) - With Azure AD B2C an account can have multiple identities, local (username and password) or social/enterprise identity (such as Facebook or AAD). This Azure AD B2C sample demonstrates how to link and unlink existing Azure AD B2C account to a social identity.|NA|
|[Link a local account to federated account](policies/link-local-account-with-federated-account)|Demonstrates how to link a user who logged in via a federated provider to a pre-created AAD B2C Local Account.|NA|
|[Sign-up with social and local account](policies/sign-up-with-social-and-local-account)|Demonstrate how to create a policy that allows a user to sign-up with a social account linked to local account|NA|

## Identity providers
|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Sign in with Apple as a Custom OpenID Connect identity provider](policies/sign-in-with-apple)|Demonstrates how to gather the correct configuration information to setup Sign in with Apple as an OpenID Connect identity provider.|NA|
|[Sign in with Kakao](policies/sign-in-with-kakao)|This sample shows how to setup Kakao as an identity provider in Azure AD B2C. Kakao is a South Korean Internet company that provides a diverse set of services.|NA|
|[Sign in with REST API identity provider](policies/rest-api-idp)|Demonstrates how allow users to sign-in with credentials stored in a legacy identity provider using REST API services.|NA|
|[Sign in through Azure AD as the identity provider, and include original Idp token](policies/B2C-Token-Includes-AzureAD-BearerToken)|Demonstrates how to sign in through a federated identity provider, Azure AD, and include the original identity provider token (Azure AD Bearer Token) as part of the B2C issued token.|NA|
|[Custom claims provider](policies/custom-claims-provider)|A custom OpenId connect claims provider that federates with Azure AD B2C over OIDC protocol.|NA|
|[Obtain the Microsoft Graph access token for an Azure AD Federated logon](policies/B2C-Token-Includes-AzureAD-BearerToken)|For scenarios where we would like to obtain the Microsoft Graph API token for a Azure AD federated logon in the context of the logged in user. For example this could be used to read the users Exchange Online mailbox within an Azure AD B2C application.|NA|
|[AAD Authentication with REST](policies/AAD-SignIn-with-REST)|Pass through authentication to Azure AD (no user created in B2C), then calls a REST API to obtain more claims.|NA|

## User interface
|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Render dynamic dropdown box](policies/selectemail)|For scenarios where you would like to fetch information during the runtime of the authentication flow, and display this data as a dropdown box dynamically for the user to make a selection. In this example, a users identifier is sent to an API, which returns a set of emails for them to select. The selected email is returned in the token.|NA|

## Data residency
|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Remote profile](policies/remote-profile)|Demonstrates how to store and read user profiles from a remote database.|NA|
|[Remote profile geo-based](policies/remote-profile-geo)|Demonstrates storing user profile either in B2C directory or in different Azure Table Storages based in user geography setting.|NA|
|[Encrypted profile](policies/encrypted-profile)|Demonstrates how to store and read user profiles from Azure AD B2C using encrypted data.|NA|


## User migration
|Sample name   |Description   |Quick deploy|
|---|---|---|
|[Seamless account migration](/../../../user-migration/tree/master/seamless-account-migration) | Where accounts have been pre-migrated into Azure AD B2C and you want to update the password on the account on initial sign in. Azure AD B2C calls a REST API to validate the credentials for accounts marked as requiring migration (via attribute) against a legacy identity provider, returns a successful response to Azure AD B2C, and Azure AD B2C writes the password to the account in the directory.|NA|
|[Seamless account migration from AWS](policies/signin-migration)|This is an end-to-end sample for migrating the users from AWS Cognito to Azure AD B2C.|NA|
| [Just in time migration v1](/../../../user-migration/tree/master/jit-migration-v1) | In this sample Azure AD B2C calls a REST API that validates the credential, and migrate the account with a Graph API call.|NA|
|[Just in time migration v2](/../../../user-migration/tree/master/jit-migration-v2) | In this sample Azure AD B2C calls a REST API to validate the credentials, return the user profile to B2C from an Azure Table, and B2C creates the account in the directory.|
|[B2C to B2C Migration](policies/B2C2B2CMigration) | Migrate users from one B2C instance to another using just in time migration.|NA|

## User info endpoint
|Sample name   |Description   |Quick deploy|
|---|---|---|
|[UserInfo Endpoint](policies/user-info-endpoint) | The UserInfo endpoint is part of the OpenID Connect standard (OIDC) specification and is designed to return claims about the authenticated user. The UserInfo endpoint is defined in the relying party policy using the EndPoint element.|[Go](https://b2ciefsetupapp.azurewebsites.net/Home/Experimental?sampleFolderName=user-info-endpoint)|

## Web Test

- [SignIn Web test](policies/signin-webtest) - This sample web test shows how to run tests and monitor results of B2C sign in's, using Azure Application Insights.

## CI/CD

- [Azure Devops](policies/devops-pipeline) - An example AzureDevOps pipeline that uploads policies regardless of naming convention.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).
