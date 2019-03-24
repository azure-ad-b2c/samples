# Azure Active Directory B2C: Advanced scenarios

Samples for several Azure AD B2C advanced scenarios 

## Local account
- [Local account change sign-in name email address](policies/change-sign-in-name) - During sign-in with a local account, a user may want to change the sign-in name (email address). This sample policy demonstrates how to allow a user to provide and validate a new email address, and store the new email address to the Azure Active Directory user account. After the user changes their email address, subsequent logins require the use of the new email address.

- [Password-less sign-in with email verification](policies/passwordless-email) - Passwordless authentication is a type of authentication where user doesn't need to sign-in with their password. This is commonly used in B2C scenarios where users use your application infrequently and tend to forget their password. This sample policy demonstrates how to allow user to sign-in, simply by providing and verifying the sign-in email address using OTP code (one time password).  

- [Custom email verification](policies/custom-email-verifcation) - Allows you to send your own custom email verification email during sign-up or password reset user journey's. The solution requires using Azure AD B2C custom policy and a REST API endpoint that sends and verifies the TOTP. 

- [Force password reset first logon](policies/force-password-reset-first-logon) - Demonstrates how to force a user to reset their password on the first logon. 

- [Sign-up and sign-in with embedded password rest](policies/embedded-password-reset) - This policy demostrates how to embed the password reset flow a part of the sign-up or sign-in policy without the AADB2C90118 error message.
- 
- [Force password after 90 days](policies/force-password-reset-after-90-days) - Demonstrates how to force a user to reset their password after 90 days from the last time user set their password.  

- [Password reset only](policies/password-reset-only) - This example policy prevents issuing an access token to the user after resetting their password.

- [Username discovery](policies/username-discovery) - This example shows how to discover a username by email address. It's useful when a user forgot their username and remembers only their email address.

- [Azure AD B2C Invitation](policies/invite)  This sample console app demonstrates how to send a sign-up email invitation. After you sent the invention, the user clicks on the **Confirm account** link, which opens the sign-up page (without the need to validate the email again). Use this approach when you need to create the users account by beforehand, while letting the user to choose the password on initial sign in. This approach is better than creating an account via Graph API and sending the password to the user via some communication means. 

- [Email Verification at Sign In](policies/signin-email-verification) - For scenarios where you would like users to validate their email via TOTP on every sign in.

- [Google Captcha on Sign In](policies/captcha-integration) - An example set of policies which integrate Google Captcha into the sign in journey.

- [Login with Phone Number](policies/signup-signin-with-phone-number) - An example set of policies to for passwordless login via Phone Number (SMS or Phone Call).

- [Password reset without the ability to use the last password](policies/password-reset-not-last-password) - For scenarios where you need to implement a password reset/change flow where the user cannot use their currently set password.

- [Disable and lockout an account after a time period](policies/disable-inactive-account) - For scenarios where you need to prevent users logging into the application after a set number of days. The account will also be disabled at the time of the users login attempt in the case the user logs in after the time period.

## Social account
- [Social identity provider force email verification](policies/social-idp-force-email) - When a users signs in with a social account, in some scenarios, the identity provider doesn't share the email address. This sample demonstrates how to force the user to provide and validate an email address.

- [Dynamic identity provider selection](policies/idps-filter)  Demonstrates how to dynamically filter the list of social identity providers rendered to the user based on application ID. In the following screenshot user can select from the list of identity providers, such as Facebook, Google+ and Amazon. With Azure AD B2C custom policies, you can configure the technical profiles to be displayed based a claim's value. The claim value contains the list of identity providers to be rendered.

- [Home Realm Discovery page](policies/home-realm-discovery-page) - Demonstrates how to create a home realm discovery page. On the sign-in page, the user provides their sign-in email address and clicks continue. B2C checks the domain portion of the sign-in email address. If the domain name is `contoso.com` the user is redirected to Contoso.com Azure AD to complete the sign-in. Otherwise the user continues the sign-in with username and password. In both cases (AAD B2C local account and AAD account), the user does not need to retype the user name. 

- [Sign-in with social identity provider and force email uniqueness](policies/force-unique-email-across-social-identities) - Demonstrates how to force a social account user to provide and validate their email address, and also checks that there is no other account with the same email address.

- [Link a local account to federated account](policies/link-local-account-with-federated-account) - Demonstrates how to link a user who logged in via a federated provider to a pre-created AAD B2C Local Account.

- [Preventing logon for Social or External IdP Accounts when Disabled in AAD B2C](policies/disable-social-account-from-logon) - For scenarios where you would like to prevent logons via Social or External IdPs when the account has been disabled in Azure AD B2C.

## Multi factor authentication

- [Edit MFA phone number](policies/edit-mfa-phone-number) - Demonstrates how to allow user to provide and validate a new MFA phone number. After the user changes their MFA phone number, on the next login, the user needs to provide the new phone number instead of the old one.

- [TOTP multi-factor authentication](policies/custom-mfa-totp) - Custom MFA solution, based on TOTP code. Allowing users to sign-in with Microsoft or Google authenticator apps.

## Generic 
- [Relying party app Role-Based Access Control (RBAC)](policies/relying-party-rbac) - Enables fine-grained access management for your relying party applications. Using RBAC, you can grant only the amount of access that users need to perform their jobs in your application. This sample policy (along with the REST API service) demonstrates how to read user's group membership, add the groups to JWT token and also prevent users from sign-in if they aren't members of one of predefined security groups.

- [SAML Service Provider](https://github.com/azure-ad-b2c/saml-sp)  This document walks you through adding a SAML-based Relying party to Azure AD B2C. 

- [Sign-up with social and local account](policies/sign-up-with-social-and-local-account) - Demonstrate how to create a policy that allows a user to sign-up with a social account linked to local account

- [Integrate REST API claims exchanges and input validation](https://github.com/azure-ad-b2c/rest-api) - A sample .Net core web API, demonstrates the use of [Restful technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/restful-technical-profile) in user journey's orchestration step and as a [validation technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/validation-technical-profile).

- [Remote profile](policies/remote-profile) - Demonstrates how to store and read user profile from a remote database. 

- [MFA after timeout or IP change](policies/mfa-absolute-timeout-and-ip-change-trigger) - A policy which forces the user to do MFA on 3 conditions:
    1. The user has newly signed up.
    2. The user has not done MFA in the last X seconds.
    3. The user is logging in from a different IP than they last logged in from.

- [Username based journey](policies/username-signup-or-signin) - For scenarios where you would like users to sign up and sign in with Usernames rather than Emails.

## App migration
- [Angular5](policies/app-migration-angular5) This guide shows how to migrate an exiting Angular SPA application to be protected with Azure AD B2C authentication.