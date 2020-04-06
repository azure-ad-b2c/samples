# Integrate Twilio Verify API for PSD2 SCA

The following sample guides you through integrating Azure AD B2C authentication with Twilio Verify API to enable your organization to meet PSD2 SCA requirements.

This sample uses the Twilio [Verify API](https://www.twilio.com/verify) to achieve this.

## Live Sample
You can see a live version of PSD2 SCA in action by following below:

1. Visit this [link](https://psd2demo.azurewebsites.net/)
2. Click **Sign up / Sign in**
3. Complete your sign up
4. Click **Send Money**
5. Enter a Payee Name, Account Number, and Amount, click **Continue**
6. You are sent to the B2C logon page where your authentication is *stepped up* with **Twilio Verify API**
7. Once completed, and arrived back to the application, click **Confirm** to complete the transaction.

## Pre requisites
1. Complete the [Azure AD B2C Get Started with Custom Policies](https://aka.ms/ief)
1. Be familiar with the `id_token_hint` usage, which is demonstrated in this [sample](https://github.com/azure-ad-b2c/samples/tree/master/policies/invite)
1. Be familiar with the `display controls` usage, which is demonstrated in this [sample](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-email) and documented [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/display-controls)
1. Obtain a [trial](https://www.twilio.com/try-twilio) account at Twilio
1. Purchase a Phone number at Twilio using this [article](https://support.twilio.com/hc/en-us/articles/223135247-How-to-Search-for-and-Buy-a-Twilio-Phone-Number-from-Console)
1. Navigate to [Verify API](https://www.twilio.com/console/verify/services) at the Twilio console, create a service and enable the PSD2 option

For the step up authentication policy to work, the Azure AD B2C user must already exist in the directory with a Phone Number registered. For the purposes of this demo, this can be achieved by signing up a user through a [User Flow](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-user-flows) with [MFA enabled](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-multi-factor-authentication).

You can pre-enrol users into the directory with **known verified** Phone Numbers by using the [Microsoft Graph API](https://docs.microsoft.com/en-us/azure/active-directory-b2c/manage-user-accounts-graph-api).

## How it works (Step Up Policy)
1. You only require the Step Up policy file to initialize a Step Up authentication
1. The id token hint must be sent with the authentication request containing the `amount` and `payee`. This is parsed in the `IdTokenHint_ExtractClaims` technical profile
1. The policy relies on a existing logon session to access the current authenticated users phone number. This claim name must be persistent across the policy files, and the session management technical profiles.
1. The Single Sign On state will skip all step until **Orchestration Step 5**, where the Twilio Verify API is invoked
1. At this step, the `Custom-SMS-Verify` self asserted technical profile is executed
1. The `inputClaimsTransformation` takes the Phone Number from the session claim and converts it to a read only state
1. The Display Control `phoneVerificationControl-ReadOnly` calls the Twilio Verify API to initiate and verify the OTP requests
1. The REST API technical profile `TwilioRestAPI-Verify-Step1` models the Twilio Verify API REST API request to generate and send a PSD2 compliant SMS
1. The REST API technical profile `TwilioRestAPI-Verify-Step2` models the Twilio Verify API REST API request to verify the entered OTP
1. Since the Twilio API returns a HTTP 200 with a status of the OTP verification in the JSON Body, Azure AD B2C needs to check the JSON payload for the `valid` OTP status. To do this, the REST API Technical Profile invokes an `outputClaimsTransform` to assert that the value of `status` is "approved"
```xml
<!-- Capture the "status" value from the JSON response. Set a comparison 
claim with a static value (dummyStatus) for the known good value-->
<OutputClaims>
    <OutputClaim ClaimTypeReferenceId="dummyStatus" DefaultValue="approved" />
    <OutputClaim ClaimTypeReferenceId="status"/>
</OutputClaims>

<!-- Initiate a claims transform to compare Status with DummyStatus-->
<OutputClaimsTransformations>
    <OutputClaimsTransformation ReferenceId="AssertOTPisApproved"/>
</OutputClaimsTransformations>

<!-- Compare Status with DummyStatus-->
<ClaimsTransformation Id="AssertOTPisApproved" TransformationMethod="AssertStringClaimsAreEqual">
    <InputClaims>
        <InputClaim ClaimTypeReferenceId="status" TransformationClaimType="inputClaim1" />
        <InputClaim ClaimTypeReferenceId="dummyStatus" TransformationClaimType="inputClaim2" />
    </InputClaims>
    <InputParameters>
        <InputParameter Id="stringComparison" DataType="string" Value="ordinalIgnoreCase" />
    </InputParameters>
</ClaimsTransformation>
```

In the `Custom-SMS-Verify` technical profile add a the metadata item to provide an error when the Claims Transform comparison does not return True.
```xml
<Item Key="UserMessageIfClaimsTransformationStringsAreNotEqual">Incorrect verification code. Try again.</Item>
```

For the Phone Number to travel in the session across policies, configure the `SM-AAD` in all policies to maintain the phone number claim:
```xml
<TechnicalProfile Id="SM-AAD">
    <DisplayName>Session Mananagement Provider</DisplayName>
    <Protocol Name="Proprietary" Handler="Web.TPEngine.SSO.DefaultSSOSessionProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
    <PersistedClaims>
        <PersistedClaim ClaimTypeReferenceId="phoneNumberString" />
    </PersistedClaims>
</TechnicalProfile>
```

The claim name (`phoneNumberString`) needs to be consistent across all policies.

## Quick setup (End to end demo)
1. Open the `B2C-WebAPI-DotNet` solution and replace the following values with your own tenant specific values in the web.config:
```xml
    <add key="ida:Tenant" value="yourtenant.onmicrosoft.com" />
    <add key="ida:TenantId" value="d6f33888-0000-4c1f-9b50-1590f171fc70" />
    <add key="ida:ClientId" value="6bd98cc8-0000-446a-a05e-b5716ef2651b" />
    <add key="ida:ClientSecret" value="secret" />
    <add key="ida:AadInstance" value="https://yourtenant.b2clogin.com/tfp/{0}/{1}" />
    <add key="ida:RedirectUri" value="https://your hosted psd2 demo app url/" />
```

1. The Web App also hosts the id token hint generator and metadata endpoint
- Create your signing certificate as shown [here](https://github.com/azure-ad-b2c/samples/tree/master/policies/invite#creating-a-signing-certificate)
- Update the following lines with respect to your certificate in the web.config:
```xml
    <add key="ida:SigningCertThumbprint" value="4F39D6014818082CBB763E5BA5F230E545212E89" />
    <add key="ida:SigningCertAlgorithm" value="RS256" /> <!-- leave as-is-->
```
2. Upload the demo application to your hosting provider of choice - Guidance for Azure App Services is [here](https://github.com/azure-ad-b2c/samples/tree/master/policies/invite#hosting-the-application-in-azure-app-service) to also upload your certificate
1. Update your Azure AD B2C application registration by adding a Reply URL equivalent to the URL at which the application is hosted at
1. Open the policy files and replace all instances of `yourtenant` with your tenant name. For a tenant such as "contoso.onmicrosoft.com", replace all instances of `yourtenant` with `contoso`
1. Find the Twilio REST API technical profile `Custom-SMS-Enrol` and update the `ServiceURL` with your **Twilio AccountSID** and the `From` number to your purchased phone number
1. Find the Twilio REST API technical profiles, `TwilioRestAPI-Verify-Step1` and `TwilioRestAPI-Verify-Step2`, and update the `ServiceURL` with your **Twilio AccountSID**
1. Go to the Azure AD B2C Blade at the [Azure Portal](https://portal.azure.com) and click **Identity Experience Framework**. Click **Policy Keys**
1. Add a new Key, name it `B2cRestTwilioClientId`. Select `manual`, and provide the value of the **Twilio Account SID**
1. Add a new Key, name it `B2cRestTwilioClientSecret`. Select `manual`, and provide the value of the **Twilio AUTH TOKEN**
1. Upload all the policy files to your tenant
1. Customize the string in the `GenerateOTPMessageEnrol` claims transform for your Sign Up SMS text.
1. Browse to your application and test the **Sign In/Up** and **Send Money** action.

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 