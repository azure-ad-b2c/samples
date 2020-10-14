# Azure Active Directory B2C: Custom CIAM User Journeys

In this repo, you will find samples for several enhanced Azure AD B2C Custom CIAM User Journeys.

## Getting started

- See our [Azure AD B2C Wiki articles](https://github.com/azure-ad-b2c/ief-wiki/wiki) here to help walkthrough the custom policy components.

- See our Custom Policy Documentation [here](https://aka.ms/ief).

- See our Custom Policy Schema reference [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-reference-trustframeworks-defined-ief-custom).

## Prerequisites

- You will require to create an Azure AD B2C directory, see the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to setup your AAD B2C environment for Custom Policies [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

## Local account policy enhancements
- [Password reset via Email or Phone verification](policies/pwd-reset-via-email-or-phone) - This demonstrates how to verify a user via Email or SMS on a single screen.

- [Sign In and Sign Up with Username or Email](policies/username-or-email) - This sample combines the UX of both the Email and Username based journeys.

- [Split Signup into separate steps for email verification and account creation](policies/split-email-verification-and-signup) - When you dont want to use the default Signup page which shows both email verification and user registration controls on the same page at once. This sample splits the default signup behaviour into two separate steps. First step performs Email Verification only, avoiding all other default fields related to users registration. Second step (if email verification was successful) takes the users to a new screen where they can actually create their accounts. This uses Azure AD to send out emails, no separate email provider integrations needed.

- [Provide consent UI to API scopes](policies/service-consent) - For scenarios where you provide a plug and play service to other partners. When the user chooses to use your service through a partner application, the user must login with their account with your service, and consent to various scopes which allow your service to share information with the partner application.

- [Sign Up and Sign In with dynamic 'Terms of Use' prompt](policies/sign-in-sign-up-versioned-tou) - Demonstrates how to incorporate a TOU or T&Cs into your user journey with the ability for users to be prompted to re-consent when the TOU/T&Cs change.


- [Local account change sign-in name email address](policies/change-sign-in-name) - During sign-in with a local account, a user may want to change the sign-in name (email address). This sample policy demonstrates how to allow a user to provide and validate a new email address, and store the new email address to the Azure Active Directory user account. After the user changes their email address, subsequent logins require the use of the new email address.

- [Password-less sign-in with email verification](policies/passwordless-email) - Passwordless authentication is a type of authentication where user doesn't need to sign-in with their password. This is commonly used in B2C scenarios where users use your application infrequently and tend to forget their password. This sample policy demonstrates how to allow user to sign-in, simply by providing and verifying the sign-in email address using OTP code (one time password).

- [Custom email verification - DisplayControls](policies/custom-email-verifcation-displaycontrol) - Allows you to send your own custom email verification email during sign-up or password reset user journey's. The is a working example of the sample refernced on the Microsoft B2C documentation site - [Custom email verification in Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-email)

- [Custom SMS provider - DisplayControls](policies/custom-sms-displaycontrol) Integrate a custom SMS provider in Azure Active Directory B2C (Azure AD B2C) to customized SMS' to users that perform multi factor authentication to your application. By using DisplayControls (currently in preview) and a third-party SMS provider, you can use your own contextualised SMS message, custom Phone Number, as well as support localization and custom one-time password (OTP) settings.

- [Force password reset first logon](policies/force-password-reset-first-logon) - Demonstrates how to force a user to reset their password on the first logon.

- [Sign-up and sign-in with embedded password reset](policies/embedded-password-reset) - This policy demonstrates how to embed the password reset flow a part of the sign-up or sign-in policy without the AADB2C90118 error message.

- [Force password after 90 days](policies/force-password-reset-after-90-days) - Demonstrates how to force a user to reset their password after 90 days from the last time user set their password.

- [Password reset only](policies/password-reset-only) - This example policy prevents issuing an access token to the user after resetting their password.

- [Username discovery](policies/username-discovery) - This example shows how to discover a username by email address. It's useful when a user forgot their username and remembers only their email address.

- [Azure AD B2C Invitation](policies/invite) This sample console app demonstrates how to send a sign-up email invitation. After you sent the invitation, the user clicks on the **Confirm account** link, which opens the sign-up page (without the need to validate the email again). Use this approach when you need to create the users account beforehand, while allowing the user to choose the password on initial sign in. This approach is better than creating an account via Graph API and sending the password to the user via some communication means.

- [Email Verification at Sign In](policies/signin-email-verification) - For scenarios where you would like users to validate their email via TOTP on every sign in.

- [Google Captcha on Sign In](policies/captcha-integration) - An example set of policies which integrate Google Captcha into the sign in journey.

- [Login with Phone Number](policies/signup-signin-with-phone-number) - An example set of policies for passwordless login via Phone Number (SMS or Phone Call).

- [Password Reset with Phone Number](policies/password-reset-with-phone-number) - An example policy to reset a users password using Phone Number (SMS or Phone Call).

- [Password reset without the ability to use the last password](policies/password-reset-not-last-password) - For scenarios where you need to implement a password reset/change flow where the user cannot use their currently set password.

- [Disable and lockout an account after a period of inactivity](policies/disable-inactive-account) - For scenarios where you need to prevent users logging into the application after a set number of days. The account will also be disabled at the time of the users login attempt in the case the user logs in after the time period.

- [Email delivered account redemption link](policies/sign-in-with-email) - This sample demonstrates how to allow the user to sign up to a web application by providing their email which sends the user a magic link to complete their account creation to their email.

- [Sign-in with a magic link](policies/sign-in-with-magic-link) - This sample demonstrates how a user can sign in to your web application by sending them a sign-in link. A magic link can be used to pre-populate user information, or accelerate the user through the user journey.

- [Banned password list](policies/banned-password-list-no-API) - For scenarios where you need to implement a sign up and password reset/change flow where the user cannot use a new password that is part of a banned password list. This sample does not use an API.

- [Impersonation Flow](policies/impersonation) - For scenarios where you require one user to impersonate another user. This is common for support desk or delegated administration of a user in an application or service. It is recommended to always issue the token of the original authenticated user and append additional information about the targeted impersonated user as part of the auth flow

- [Sign-in with FIDO](policies/fido2) - Demonstrates how to sign-in with a FIDO authenticator (as a first factor authentication). This policy use the WebAuthn standard to register new credential and sign-in with FIDO credential.

- [Sign-in with Home Realm Discovery and Default IdP](policies/default-home-realm-discovery) - Demonstrates how to implement a sign in journey, where the user is automatically directed to their federated identity provider based off of their email domain. And for users who arrive with an unknown domain, they are redirected to a default identity provider.

- [Terms of Service with Sign-in or Sign-up](policies/terms-of-service) - Demonstrates how to implement Terms of Service within a SUSI experience. This policy writes a configurable policy version onto an attribute stored in the directory. If you update the version within the policy, it will prompt the user during the next login to force the user to accept the new terms of service agreement.

## Social account policy enhancements

- [Social identity provider force email verification](policies/social-idp-force-email) - When a user signs in with a social account, in some scenarios, the identity provider doesn't share the email address. This sample demonstrates how to force the user to provide and validate an email address.

- [Dynamic identity provider selection](policies/idps-filter) - Demonstrates how to dynamically filter the list of social identity providers rendered to the user based on the requests application ID. In the following screenshot user can select from the list of identity providers, such as Facebook, Google+ and Amazon. With Azure AD B2C custom policies, you can configure the technical profiles to be displayed based a claim's value. The claim value contains the list of identity providers to be rendered.

- [Home Realm Discovery page](policies/home-realm-discovery-modern) - Demonstrates how to create a home realm discovery page. On the sign-in page, the user provides their sign-in email address and clicks continue. B2C checks the domain portion of the sign-in email address. If the domain name is `contoso.com` the user is redirected to Contoso.com Azure AD to complete the sign-in. Otherwise the user continues the sign-in with username and password. In both cases (AAD B2C local account and AAD account), the user does not need to retype the user name.

- [Sign-in with social identity provider and force email uniqueness](policies/force-unique-email-across-social-identities) - Demonstrates how to force a social account user to provide and validate their email address, and also checks that there is no other account with the same email address.

- [Account linkage](policies/account-linkage-unified) (new version, one policy for both link and unlink) - With Azure AD B2C an account can have multiple identities, local (username and password) or social/enterprise identity (such as Facebook or AAD). This Azure AD B2C sample demonstrates how to link and unlink existing Azure AD B2C account to a social identity. Unified policy for link and unlink.

- [Account linkage](policies/account-linkage) (a policy for link and another policy for unlink.) - With Azure AD B2C an account can have multiple identities, local (username and password) or social/enterprise identity (such as Facebook or AAD). This Azure AD B2C sample demonstrates how to link and unlink existing Azure AD B2C account to a social identity.

- [Link a local account to federated account](policies/link-local-account-with-federated-account) - Demonstrates how to link a user who logged in via a federated provider to a pre-created AAD B2C Local Account.

- [Preventing logon for Social or External IdP Accounts when Disabled in AAD B2C](policies/disable-social-account-from-logon) - For scenarios where you would like to prevent logons via Social or External IdPs when the account has been disabled in Azure AD B2C.

- [Sign in with Apple as a Custom OpenID Connect identity provider](policies/sign-in-with-apple) - Demonstrates how to gather the correct configuration information to setup Sign in with Apple as an OpenID Connect identity provider.

- [Sign in with REST API identity provider](policies/rest-api-idp) - Demonstrates how allow users to sign-in with credentials stored in a legacy identity provider using REST API services.

- [Sign in through Azure AD as the identity provider, and include original Idp token](policies/B2C-Token-Includes-AzureAD-BearerToken) - Demonstrates how to sign in through a federated identity provider, Azure AD, and include the original identity provider token (Azure AD Bearer Token) as part of the B2C issued token.

## Multi factor authentication enhancements

- [Integrate Twilio Verify API for PSD2 SCA](policies/twilio-mfa-psd2) - The following sample guides you through integrating Azure AD B2C authentication with Twilio Verify API to enable your organization to meet PSD2 SCA requirements.

- [Edit MFA phone number](policies/edit-mfa-phone-number) - Demonstrates how to allow user to provide and validate a new MFA phone number. After the user changes their MFA phone number, on the next login, the user needs to provide the new phone number instead of the old one.

- [TOTP multi-factor authentication](policies/custom-mfa-totp) - Custom MFA solution, based on TOTP code. Allowing users to sign-in with Microsoft or Google authenticator apps.

- [Sign In With Authenticator](policies/sign-in-with-authenticator) - This is a sample to show how you can create a B2C Custom Policy to signin with Authenticator Apps to B2C. It is related to the custom-mfa-totp sample, which shows how to use the Authenticator app as MFA.

- [Authy App multi-factor authentication](policies/custom-mfa-authy-app) - Custom MFA solution, based on Authy App (push notification). Allowing users to sign-in with Twilio Auth App (authenticator apps).

- [MFA with either Phone (Call/SMS) or Email verification](policies/mfa-email-or-phone) - Allow the user to do MFA by either Phone (Call/SMS) or Email verification, with the ability to change this preference via Profile Edit.

- [Add & Select 2 MFA phone numbers at SignIn/Signup](policies/mfa-add-secondarymfa) - Demonstrates how to store two phone numbers in a secure manner in B2C and choose between any two at signIn. The flow prompts the user to store a secondary phone if only one phone number is one file. Once the two numbers are stored as part of SignUp or SignIn the user is given a choice to select between the two phones for their MFA on subsequent signIns.

- [MFA after timeout or IP change](policies/mfa-absolute-timeout-and-ip-change-trigger) - A policy which forces the user to do MFA on 3 conditions:

  1. The user has newly signed up.
  2. The user has not done MFA in the last X seconds.
  3. The user is logging in from a different IP than they last logged in from.

- [Unknown Devices MFA](policies/mfa-unknown-devices) - Demonstrates how to detect unknown devices which might be required to prompt MFA as illustrated in this particular sample or send email to the user signing in from unknown device.

## User interface enhancements

- [Render dynamic dropdown box](policies/selectemail) - For scenarios where you would like to fetch information during the runtime of the authentication flow, and display this data as a dropdown box dynamically for the user to make a selection. In this example, a users identifier is sent to an API, which returns a set of emails for them to select. The selected email is returned in the token.

## Generic enhancements

- [Delete my account](policies/delete-my-account) - Demonstrates how to delete a local or social account from the directory

- [Integrating Azure AD B2C with TypingDNA](policies/signin-signup-typingdna) - This sample demonstrates how to integrate TypingDNA as a PSD2 SCA compliant authentication factor. Find more about TypingDNA [here](https://www.typingdna.com/).

- [Password Reset OTP only sent if Email is registered](policies/pwd-reset-email-exists) - Demonstrate how to use a displayControl to send One-Time-Passcodes to users only if the email is registered against a user in the directory.

- [Relying party app Role-Based Access Control (RBAC)](policies/relying-party-rbac) - Enables fine-grained access management for your relying party applications. Using RBAC, you can grant only the amount of access that users need to perform their jobs in your application. This sample policy (along with the REST API service) demonstrates how to read user's group membership, add the groups to JWT token and also prevent users from sign-in if they aren't members of one of predefined security groups.

- [Sign-up with social and local account](policies/sign-up-with-social-and-local-account) - Demonstrate how to create a policy that allows a user to sign-up with a social account linked to local account

- [Integrate REST API claims exchanges and input validation](https://github.com/azure-ad-b2c/rest-api) - A sample .Net core web API, demonstrates the use of [Restful technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/restful-technical-profile) in user journey's orchestration step and as a [validation technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/validation-technical-profile).

- [Remote profile](policies/remote-profile) - Demonstrates how to store and read user profiles from a remote database.

- [Username based journey](policies/username-signup-or-signin) - For scenarios where you would like users to sign up and sign in with Usernames rather than Emails.

- [Custom claims provider](policies/custom-claims-provider) - A custom OpenId connect claims provider that federates with Azure AD B2C over OIDC protocol.

- [Obtain the Microsoft Graph access token for an Azure AD Federated logon](policies/B2C-Token-Includes-AzureAD-BearerToken) - For scenarios where we would like to obtain the Microsoft Graph API token for a Azure AD federated logon in the context of the logged in user. For example this could be used to read the users Exchange Online mailbox within an Azure AD B2C application.

 - [AAD Authentication with REST](policies/AAD-SignIn-with-REST) - Pass through authentication to Azure AD (no user created in B2C), then calls a REST API to obtain more claims. 

## App migration

- [Angular5](policies/app-migration-angular5) This guide shows how to migrate an exiting Angular SPA application to be protected with Azure AD B2C authentication.

## Conditional Access

- [sign-in with Conditional access](policies/conditional-access)

## Web Test

- [SignIn Web test](policies/signin-webtest) This sample web test shows how to run tests and monitor results of B2C sign in's, using Azure Application Insights.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).
