# Custom SMS verification in Azure Active Directory B2C

Integrate a custom SMS provider in Azure Active Directory B2C (Azure AD B2C) to customized SMS' to users that perform multi factor authentication to your application. By using DisplayControls (currently in preview) and a third-party SMS provider, you can use your own contextualised SMS message, custom Phone Number, as well as support localization and custom one-time password (OTP) settings.

Custom SMS integration requires the use of a third-party SMS provider such as Twilio or your own custom SMS provider. This article describes setting up a solution that uses Twilio.

## Create a Twilio account
When you're ready to get a Twilio account, sign up at [Try Twilio](https://www.twilio.com/docs/usage/tutorials/how-to-use-your-free-trial-account#sign-up-for-your-free-twilio-trial). You can start with a free account, and upgrade your account later.

When you sign up for a Twilio account, you'll receive an account ID and an authentication token. Both will be needed to make Twilio API calls. To prevent unauthorized access to your account, keep your authentication token secure. Your account ID and authentication token are viewable at the [Twilio account page](https://www.twilio.com/console), in the fields labeled **ACCOUNT SID** and **AUTH TOKEN**, respectively. These will be referenced later in this article.

## Create Azure AD B2C policy key

Next, store the Twilio AUTH TOKEN in an Azure AD B2C policy key for your policies to reference when communicating with the Twilio API.

1. Sign in to the [Azure portal](https://portal.azure.com/).
1. Make sure you're using the directory that contains your Azure AD B2C tenant. Select the **Directory + subscription** filter in the top menu and choose your Azure AD B2C directory.
1. Choose **All services** in the top-left corner of the Azure portal, and then search for and select **Azure AD B2C**.
1. On the Overview page, select **Identity Experience Framework**.

### Create the Twilio Username
1. Select **Policy Keys** and then select **Add**.
1. For **Options**, choose `Manual`.
1. Enter a **Name** for the policy key. For example, `TwilioAccountSid`. The prefix `B2C_1A_` is added automatically to the name of your key.
1. In **Secret**, enter your ACCOUNT SID that you previously recorded.
1. For **Key usage**, select `Signature`.
1. Select **Create**.

### Create the Twilio Secret
1. Select **Policy Keys** and then select **Add**.
1. For **Options**, choose `Manual`.
1. Enter a **Name** for the policy key. For example, `TwilioSecret`. The prefix `B2C_1A_` is added automatically to the name of your key.
1. In **Secret**, enter your AUTH TOKEN that you previously recorded.
1. For **Key usage**, select `Signature`.
1. Select **Create**.

## Register a Phone Number in Twilio
Follow the steps at Twilio [to obtain a phone number from which SMS](https://www.twilio.com/docs/usage/tutorials/how-to-use-your-free-trial-account#get-your-first-twilio-phone-number) will be sent from.

## Add Azure AD B2C claim types

In your policy, add the following claim types to the `<ClaimsSchema>` element within `<BuildingBlocks>`.

These claims types are necessary to generate and verify one-time password (OTP) codes sent via your third party MFA provider.
They will also handle both the initial enrolment and subsequent verification scenarios.

## Add Claims transformations

It is required to add the following claims transformations to to generate the OTP message to send to your third party SMS provider and handle displaying the phone number as read only when the user logs in on subsequent attempts.

Change the string in `GenerateOTPMessage` for your personal SMS message. The `{0}` is required for Azure AD B2C to insert the OTP into the string.

## Add DataUri content definition

Below the claims transformations within `<BuildingBlocks>`, add the following [ContentDefinition](contentdefinitions.md) to reference the version 2.0.0 data URI:

```xml
 <ContentDefinition Id="api.localaccountsignup">
    <DataUri>urn:com:microsoft:aad:b2c:elements:contract:selfasserted:2.0.0</DataUri>
  </ContentDefinition>
```

## Add Display Controls
Within the `<BuildingBlocks>` element, add the following `DisplayControls` of type `VerificationControl` to your policy.

The Display Controls are configured to provide a SMS entry for enrolment, and an SMS entry for verification on subsequent logins where the phone number is displayed as read only.

## Add Claims Providers

Add the Claims Providers within the `ClaimsProviders` element to handle enrolment and verification via SMS.
And to read and write the phone number during sign up/in.
## Add OTP technical profiles

The `GenerateOtp` technical profile generates a code for the third party SMS provider. The `VerifyOtp` technical profile verifies the code associated with the phone number. You can change the configuration of the format and the expiration of the one-time password. For more information about OTP technical profiles, see [Define a one-time password technical profile](one-time-password-technical-profile.md).

Add the following technical profiles to the `<ClaimsProviders>` element.

## Add the Twilio REST API Claims Providers

The `DisplayControl` is configured to send the generated OTP to a REST API. In this case, that is Twilio's API endpoint. The following Claims Provider will make a request to the Twilio API with the configured SMS message from the specific caller Id. 

Replace the Account SID in the `ServiceURL` metadata item with your Twilio **ACCOUNT SID**.
Replace the `DefaultValue` for the `From` Input Claim with the Phone Number registered at Twilio to send SMS'.

The `CryptographicKeys` element references the 'ACCOUNT SID' and 'AUTH TOKEN' keys setup prior in the Azure AD B2C Policy Keys menu. This is used to build the authorization header to authenticate to your Twilio instance using HTTP Basic authentication.

### Manage 2FA Session 
When the user has performed MFA via a policy, subsequent policies may skip MFA. For example, calling Profile Edit after Sign In.

## Add Orchestration steps
From the TrustframeworkBase file, copy the entire `SignUpOrSignIn` UserJourney element. Insert this into the TrustframeworkExtension file within the UserJourneys element. Uncomment the Userjourneys element if it is commented out.
Rename the `SignUpOrSignIn` UserJourney to `SignUpOrSignInCustomSMS`.
After Step 3 of the `SignUpOrSignIn` journey add the following Orchestration steps:

## Update relying party

In the SignUpOrSign.xml file, change the `DefaultUserJourney` value to `SignUpOrSignInCustomSMS`.

```xml
  <RelyingParty>
    <DefaultUserJourney ReferenceId="SignUpOrSignInCustomSMS" />
```
