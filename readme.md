# Azure Active Directory B2C: Custom CIAM User Journeys

In this repo, you will find samples for several enhanced Azure AD B2C Custom CIAM User Journeys.

## Prerequisites
- You will require to create an Azure AD B2C directory, see the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to setup your AAD B2C environment for Custom Policies [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.


## Local account policy enhancements
- [Sign Up and Sign In with dynamic 'Terms of Use' prompt](policies/sign-in-sign-up-versioned-tou) - Demonstrates how to incorporate a TOU or T&Cs into your user journey with the ability for users to be prompted to re-consent when the TOU/T&Cs change.

- [Delete my account](policies/delete-my-account) - Demonstrates how to delete a local or social account from the directory

- [Local account change sign-in name email address](policies/change-sign-in-name) - During sign-in with a local account, a user may want to change the sign-in name (email address). This sample policy demonstrates how to allow a user to provide and validate a new email address, and store the new email address to the Azure Active Directory user account. After the user changes their email address, subsequent logins require the use of the new email address.

- [Password-less sign-in with email verification](policies/passwordless-email) - Passwordless authentication is a type of authentication where user doesn't need to sign-in with their password. This is commonly used in B2C scenarios where users use your application infrequently and tend to forget their password. This sample policy demonstrates how to allow user to sign-in, simply by providing and verifying the sign-in email address using OTP code (one time password).  

- [Custom email verification](policies/custom-email-verifcation) - Allows you to send your own custom email verification email during sign-up or password reset user journey's. The solution requires using Azure AD B2C custom policy and a REST API endpoint that sends and verifies the TOTP. 

- [Force password reset first logon](policies/force-password-reset-first-logon) - Demonstrates how to force a user to reset their password on the first logon. 

- [Sign-up and sign-in with embedded password reset](policies/embedded-password-reset) - This policy demonstrates how to embed the password reset flow a part of the sign-up or sign-in policy without the AADB2C90118 error message.

- [Force password after 90 days](policies/force-password-reset-after-90-days) - Demonstrates how to force a user to reset their password after 90 days from the last time user set their password.  

- [Password reset only](policies/password-reset-only) - This example policy prevents issuing an access token to the user after resetting their password.

- [Username discovery](policies/username-discovery) - This example shows how to discover a username by email address. It's useful when a user forgot their username and remembers only their email address.

- [Azure AD B2C Invitation](policies/invite)  This sample console app demonstrates how to send a sign-up email invitation. After you sent the invitation, the user clicks on the **Confirm account** link, which opens the sign-up page (without the need to validate the email again). Use this approach when you need to create the users account beforehand, while allowing the user to choose the password on initial sign in. This approach is better than creating an account via Graph API and sending the password to the user via some communication means. 

- [Email Verification at Sign In](policies/signin-email-verification) - For scenarios where you would like users to validate their email via TOTP on every sign in.

- [Google Captcha on Sign In](policies/captcha-integration) - An example set of policies which integrate Google Captcha into the sign in journey.

- [Login with Phone Number](policies/signup-signin-with-phone-number) - An example set of policies to for passwordless login via Phone Number (SMS or Phone Call).

- [Password reset without the ability to use the last password](policies/password-reset-not-last-password) - For scenarios where you need to implement a password reset/change flow where the user cannot use their currently set password.

- [Disable and lockout an account after a time period](policies/disable-inactive-account) - For scenarios where you need to prevent users logging into the application after a set number of days. The account will also be disabled at the time of the users login attempt in the case the user logs in after the time period.

- [Email delivered account redemption link](policies/sign-in-with-email) - This sample demonstrates how to allow the user to automatically sign-in to a web application by redeeming a link sent via email. The web application sends an email to the end user with a link to sign-in policy. When user clicks on the link, Azure AD B2C issues an id_token, without prompting for a password.

- [Sign-in with a magic link](policies/sign-in-with-magic-link) - This sample demonstrates how to sign-in to a web application by sending a sign-in link. A magic link can be used to pre-populate user information, or accelerate the user through the user journey.

- [Banned password list](policies/banned-password-list-no-API) - For scenarios where you need to implement a sign up and password reset/change flow where the user cannot use a new password that is part of a banned password list. This sample does not use an API.

- [Impersonation Flow](policies/impersonation) - For scenarios where you require one user to impersonate another user. This is common for support desk or delegated administration of a user in an application or service. It is recommended to always issue the token of the original authenticated user and append additional information about the targeted impersonated user as part of the auth flow

- [Sign-in with FIDO](policies/fido2) - Demonstrates how to sign-in with a FIDO authenticator (as a first factor authentication). This policy use the WebAuthn standard to register new credential and sign-in with FIDO credential.


## Social account policy enhancements
- [Social identity provider force email verification](policies/social-idp-force-email) - When a user signs in with a social account, in some scenarios, the identity provider doesn't share the email address. This sample demonstrates how to force the user to provide and validate an email address.

- [Dynamic identity provider selection](policies/idps-filter) - Demonstrates how to dynamically filter the list of social identity providers rendered to the user based on the requests application ID. In the following screenshot user can select from the list of identity providers, such as Facebook, Google+ and Amazon. With Azure AD B2C custom policies, you can configure the technical profiles to be displayed based a claim's value. The claim value contains the list of identity providers to be rendered.

- [Home Realm Discovery page](policies/home-realm-discovery-page) - Demonstrates how to create a home realm discovery page. On the sign-in page, the user provides their sign-in email address and clicks continue. B2C checks the domain portion of the sign-in email address. If the domain name is `contoso.com` the user is redirected to Contoso.com Azure AD to complete the sign-in. Otherwise the user continues the sign-in with username and password. In both cases (AAD B2C local account and AAD account), the user does not need to retype the user name. 

- [Sign-in with social identity provider and force email uniqueness](policies/force-unique-email-across-social-identities) - Demonstrates how to force a social account user to provide and validate their email address, and also checks that there is no other account with the same email address.

- [Account linkage](policies/account-linkage) - With Azure AD B2C an account can have multiple identities, local (username and password) or social/enterprise identity (such as Facebook or AAD). This Azure AD B2C sample demonstrates how to link and unlink existing Azure AD B2C account to a social identity.

- [Link a local account to federated account](policies/link-local-account-with-federated-account) - Demonstrates how to link a user who logged in via a federated provider to a pre-created AAD B2C Local Account.

- [Preventing logon for Social or External IdP Accounts when Disabled in AAD B2C](policies/disable-social-account-from-logon) - For scenarios where you would like to prevent logons via Social or External IdPs when the account has been disabled in Azure AD B2C.

- [Sign in with Apple as a Custom OpenID Connect identity provider](policies/sign-in-with-apple) - Demonstrates how to gather the correct configuration information to setup Sign in with Apple as an OpenID Connect identity provider.

## Multi factor authentication enhancements

- [Edit MFA phone number](policies/edit-mfa-phone-number) - Demonstrates how to allow user to provide and validate a new MFA phone number. After the user changes their MFA phone number, on the next login, the user needs to provide the new phone number instead of the old one.

- [TOTP multi-factor authentication](policies/custom-mfa-totp) - Custom MFA solution, based on TOTP code. Allowing users to sign-in with Microsoft or Google authenticator apps.

- [Authy App multi-factor authentication](policies/custom-mfa-authy-app) - Custom MFA solution, based on Authy App (push notification). Allowing users to sign-in with Twilio Auth App (authenticator apps).

- [MFA with either Phone (Call/SMS) or Email verification](policies/mfa-email-or-phone) - Allow the user to do MFA by either Phone (Call/SMS) or Email verification, with the ability to change this preference via Profile Edit.

- [Add & Select 2 MFA phone numbers at SignIn/Signup](policies/mfa-add-secondarymfa) - Demonstrates how to store two phone numbers in a secure manner in B2C and choose between any two at signIn. The flow prompts the user to store a secondary phone if only one phone number is one file. Once the two numbers are stored as part of SignUp or SignIn the user is given a choice to select between the two phones for their MFA on subsequent signIns. 

## Generic enhancements
- [Relying party app Role-Based Access Control (RBAC)](policies/relying-party-rbac) - Enables fine-grained access management for your relying party applications. Using RBAC, you can grant only the amount of access that users need to perform their jobs in your application. This sample policy (along with the REST API service) demonstrates how to read user's group membership, add the groups to JWT token and also prevent users from sign-in if they aren't members of one of predefined security groups.

- [SAML Service Provider](https://github.com/azure-ad-b2c/saml-sp)  This document walks you through adding a SAML-based Relying party to Azure AD B2C. 

- [Sign-up with social and local account](policies/sign-up-with-social-and-local-account) - Demonstrate how to create a policy that allows a user to sign-up with a social account linked to local account

- [Integrate REST API claims exchanges and input validation](https://github.com/azure-ad-b2c/rest-api) - A sample .Net core web API, demonstrates the use of [Restful technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/restful-technical-profile) in user journey's orchestration step and as a [validation technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/validation-technical-profile).

- [Remote profile](policies/remote-profile) - Demonstrates how to store and read user profiles from a remote database. 

- [MFA after timeout or IP change](policies/mfa-absolute-timeout-and-ip-change-trigger) - A policy which forces the user to do MFA on 3 conditions:
    1. The user has newly signed up.
    2. The user has not done MFA in the last X seconds.
    3. The user is logging in from a different IP than they last logged in from.

- [Username based journey](policies/username-signup-or-signin) - For scenarios where you would like users to sign up and sign in with Usernames rather than Emails.

 - [Custom claims provider](policies/custom-claims-provider) A custom OpenId connect claims provider that federates with Azure AD B2C over OIDC protocol. 

## App migration
- [Angular5](policies/app-migration-angular5) This guide shows how to migrate an exiting Angular SPA application to be protected with Azure AD B2C authentication.

## Web Test
- [SignIn Web test](policies/signin-webtest) This sample web test shows how to run tests and monitor results of B2C sign in's, using Azure Application Insights.

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).
