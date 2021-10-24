# Custom email verification with Sendinblue

Custom email verification requires the use of a third-party email provider like [SendGrid](https://sendgrid.com), [Mailjet](https://Mailjet.com),  [SparkPost](https://sparkpost.com) or [Sendinblue](https://www.sendinblue.com/), a custom REST API, or any HTTP-based email provider (including your own). This article describes setting up a solution that uses Sendinblue.

## Create a Sendinblue account

If you don't already have one, start by setting up a Sendinblue account [here](https://app.sendinblue.com/account/register).

Be sure to complete the section in which you [get sendinblue API key](https://developers.sendinblue.com/docs#quick-start). Record the API key for use in a later step.

## Create Azure AD B2C policy key

Next, store the Sendinblue API key in an Azure AD B2C policy key for your policies to reference.

1. Sign in to the [Azure portal](https://portal.azure.com/).
1. Make sure you're using the directory that contains your Azure AD B2C tenant. Select the **Directory + subscription** filter in the top menu and choose your Azure AD B2C directory.
1. Choose **All services** in the top-left corner of the Azure portal, and then search for and select **Azure AD B2C**.
1. On the Overview page, select **Identity Experience Framework**.
1. Select **Policy Keys** and then select **Add**.
1. For **Options**, choose **Manual**.
1. Enter a **Name** for the policy key. For example, `sendinblueAPIKey`. The prefix `B2C_1A_` is added automatically to the name of your key.
1. In **Secret**, enter your API key that you previously recorded.
1. For **Key usage**, select **Signature**.
1. Select **Create**.

## Create Sendinblue template

With a Sendinblue account created and Sendinblue API key stored in an Azure AD B2C policy key, create a Sendinblue [transactional email template](https://help.sendinblue.com/hc/en-us/articles/360000946299-Create-customize-transactional-email-templates).

1. On the Sendinblue site, open the [templates](https://my.sendinblue.com/camp/lists/template) page and select **New Template**.
1. Enter a unique template name like `Verification email` 
1. Enter `Verify your e-mail address` for the subject line (you can customize that later)
1. select **Next Step**.
1. To begin editing your new template, select **Paste your code**.
1. In the HTML editor, paste following HTML template or use your own. The `{{params.OTP}}` and `{{params.email}}`  parameters will be replaced dynamically with their respective values.

    ```HTML
    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

    <html xmlns="http://www.w3.org/1999/xhtml" dir="ltr" lang="en"><head id="Head1">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"><title>Contoso demo account email verification code</title><meta name="ROBOTS" content="NOINDEX, NOFOLLOW">
       <style>
           table td {border-collapse:collapse;margin:0;padding:0;}
       </style>
    </head>
    <body dir="ltr" lang="en">
       <table width="100%" cellpadding="0" cellspacing="0" border="0" dir="ltr" lang="en">
            <tr>
               <td valign="top" width="50%"></td>
               <td valign="top">
                  <!-- Email Header -->
                  <table width="640" cellpadding="0" cellspacing="0" border="0" dir="ltr" lang="en" style="border-left:1px solid #e3e3e3;border-right: 1px solid #e3e3e3;">
                   <tr style="background-color: #0072C6;">
                       <td width="1" style="background:#0072C6; border-top:1px solid #e3e3e3;"></td>
                       <td width="24" style="border-top:1px solid #e3e3e3;border-bottom:1px solid #e3e3e3;">&nbsp;</td>
                       <td width="310" valign="middle" style="border-top:1px solid #e3e3e3; border-bottom:1px solid #e3e3e3;padding:12px 0;">
                           <h1 style="line-height:20pt;font-family:Segoe UI Light; font-size:18pt; color:#ffffff; font-weight:normal;">
                            <span id="HeaderPlaceholder_UserVerificationEmailHeader"><font color="#FFFFFF">Verify your email address</font></span>
                           </h1>
                       </td>
                       <td width="24" style="border-top: 1px solid #e3e3e3;border-bottom: 1px solid #e3e3e3;">&nbsp;</td>
                   </tr>
                  </table>
                  <!-- Email Content -->
                  <table width="640" cellpadding="0" cellspacing="0" border="0" dir="ltr" lang="en">
                   <tr>
                       <td width="1" style="background:#e3e3e3;"></td>
                       <td width="24">&nbsp;</td>
                       <td id="PageBody" width="640" valign="top" colspan="2" style="border-bottom:1px solid #e3e3e3;padding:10px 0 20px;border-bottom-style:hidden;">
                           <table cellpadding="0" cellspacing="0" border="0">
                               <tr>
                                   <td width="630" style="font-size:10pt; line-height:13pt; color:#000;">
                                       <table cellpadding="0" cellspacing="0" border="0" width="100%" style="" dir="ltr" lang="en">
                                           <tr>
                                               <td>

       <div style="font-family:'Segoe UI', Tahoma, sans-serif; font-size:14px; color:#333;">
           <span id="BodyPlaceholder_UserVerificationEmailBodySentence1">Thanks for verifying your {{params.email}} account!</span>
       </div>
       <br>
       <div style="font-family:'Segoe UI', Tahoma, sans-serif; font-size:14px; color:#333; font-weight: bold">
           <span id="BodyPlaceholder_UserVerificationEmailBodySentence2">Your code is: {{params.OTP}}</span>
       </div>
       <br>
       <br>

                                                   <div style="font-family:'Segoe UI', Tahoma, sans-serif; font-size:14px; color:#333;">
                                                   Sincerely,
                                                   </div>
                                                   <div style="font-family:'Segoe UI', Tahoma, sans-serif; font-size:14px; font-style:italic; color:#333;">
                                                       Contoso
                                                   </div>
                                               </td>
                                           </tr>
                                       </table>
                                   </td>
                               </tr>
                           </table>

                       </td>

                       <td width="1">&nbsp;</td>
                       <td width="1"></td>
                       <td width="1">&nbsp;</td>
                       <td width="1" valign="top"></td>
                       <td width="29">&nbsp;</td>
                       <td width="1" style="background:#e3e3e3;"></td>
                   </tr>
                   <tr>
                       <td width="1" style="background:#e3e3e3; border-bottom:1px solid #e3e3e3;"></td>
                       <td width="24" style="border-bottom:1px solid #e3e3e3;">&nbsp;</td>
                       <td id="PageFooterContainer" width="585" valign="top" colspan="6" style="border-bottom:1px solid #e3e3e3;padding:0px;">


                       </td>

                       <td width="29" style="border-bottom:1px solid #e3e3e3;">&nbsp;</td>
                       <td width="1" style="background:#e3e3e3; border-bottom:1px solid #e3e3e3;"></td>
                   </tr>
                  </table>

               </td>
               <td valign="top" width="50%"></td>
           </tr>
       </table>
    </body>
    </html>
    ```

1. Select **Save and activate**.
1. Return to the **Templates** page by selecting the back arrow.
1. Record the **ID** of template you created for use in a later step. For example, `1`. You specify this ID when you [add the claims transformation](#add-the-claims-transformation).

## Add Azure AD B2C claim types

In your policy, add the following claim types to the `<ClaimsSchema>` element within `<BuildingBlocks>`.

These claims types are necessary to generate and verify the email address using a one-time password (OTP) code.

```xml
<ClaimType Id="Otp">
  <DisplayName>Secondary One-time password</DisplayName>
  <DataType>string</DataType>
</ClaimType>
<ClaimType Id="emailRequestBody">
  <DisplayName>Sendiblue request body</DisplayName>
  <DataType>string</DataType>
</ClaimType>
<ClaimType Id="VerificationCode">
  <DisplayName>Secondary Verification Code</DisplayName>
  <DataType>string</DataType>
  <UserHelpText>Enter your email verification code</UserHelpText>
  <UserInputType>TextBox</UserInputType>
</ClaimType>
```

## Add the claims transformation

Next, you need a claims transformation to output a JSON string claim that will be the body of the request sent to Sendinblue.

The JSON object's structure is defined by the IDs in dot notation of the InputParameters and the TransformationClaimTypes of the InputClaims. Numbers in the dot notation imply arrays. The values come from the InputClaims' values and the InputParameters' "Value" properties. 

Add the following claims transformation to the `<ClaimsTransformations>` element within `<BuildingBlocks>`. Make the following updates to the claims transformation XML:

* Update the `template_id` InputParameter value with the ID of the Sendinblue transactional template you created earlier in [Create Sendinblue template](#create-sendinblue-template).
* Update the value of the `params.subject` subject line input parameter with a subject line appropriate for your organization.

```xml
<ClaimsTransformation Id="GenerateEmailRequestBody" TransformationMethod="GenerateJson">
  <InputClaims>
    <InputClaim ClaimTypeReferenceId="email" TransformationClaimType="to.0.email" />
    <InputClaim ClaimTypeReferenceId="otp" TransformationClaimType="params.OTP" />
    <InputClaim ClaimTypeReferenceId="email" TransformationClaimType="params.email" />
  </InputClaims>
  <InputParameters>
    <!-- Update the template_id value with the ID of your Sendinblue template. -->
    <InputParameter Id="template_id" DataType="int" Value="1"/>
    <!-- Update with a subject line appropriate for your organization. -->
    <InputParameter Id="params.subject" DataType="string" Value="Contoso account email verification code"/>
  </InputParameters>
  <OutputClaims>
    <OutputClaim ClaimTypeReferenceId="emailRequestBody" TransformationClaimType="outputClaim"/>
  </OutputClaims>
</ClaimsTransformation>
```

## Add DataUri content definition

Below the claims transformations within `<BuildingBlocks>`, add the following `ContentDefinition` to reference the version 2.1.0 data URI:

```xml
<ContentDefinitions>
 <ContentDefinition Id="api.localaccountsignup">
    <DataUri>urn:com:microsoft:aad:b2c:elements:contract:selfasserted:2.1.0</DataUri>
  </ContentDefinition>
  <ContentDefinition Id="api.localaccountpasswordreset">
    <DataUri>urn:com:microsoft:aad:b2c:elements:contract:selfasserted:2.1.0</DataUri>
  </ContentDefinition>
</ContentDefinitions>
```

## Create a DisplayControl

A verification display control is used to verify the email address with a verification code that's sent to the user.

This example display control is configured to:

1. Collect the `email` address claim type from the user.
1. Wait for the user to provide the `verificationCode` claim type with the code sent to the user.
1. Return the `email` back to the self-asserted technical profile that has a reference to this display control.
1. Using the `SendCode` action, generate an OTP code and send an email with the OTP code to the user.

Under content definitions, still within `<BuildingBlocks>`, add the following `DisplayControl` of type `VerificationControl` to your policy.

```xml
<DisplayControls>
  <DisplayControl Id="emailVerificationControl" UserInterfaceControlType="VerificationControl">
    <DisplayClaims>
      <DisplayClaim ClaimTypeReferenceId="email" Required="true" />
      <DisplayClaim ClaimTypeReferenceId="verificationCode" ControlClaimType="VerificationCode" Required="true" />
    </DisplayClaims>
    <OutputClaims>
      <OutputClaim ClaimTypeReferenceId="email" />
    </OutputClaims>
    <Actions>
      <Action Id="SendCode">
        <ValidationClaimsExchange>
          <ValidationClaimsExchangeTechnicalProfile TechnicalProfileReferenceId="GenerateOtp" />
          <ValidationClaimsExchangeTechnicalProfile TechnicalProfileReferenceId="SendOtp" />
        </ValidationClaimsExchange>
      </Action>
      <Action Id="VerifyCode">
        <ValidationClaimsExchange>
          <ValidationClaimsExchangeTechnicalProfile TechnicalProfileReferenceId="VerifyOtp" />
        </ValidationClaimsExchange>
      </Action>
    </Actions>
  </DisplayControl>
</DisplayControls>
```

## Add OTP technical profiles

The `GenerateOtp` technical profile generates a code for the email address. The `VerifyOtp` technical profile verifies the code associated with the email address. You can change the configuration of the format and the expiration of the one-time password. 

Add the following technical profiles to the `<ClaimsProviders>` element.

```xml
<ClaimsProvider>
  <DisplayName>One time password technical profiles</DisplayName>
  <TechnicalProfiles>
    <TechnicalProfile Id="GenerateOtp">
      <DisplayName>Generate one time password</DisplayName>
      <Protocol Name="Proprietary" Handler="Web.TPEngine.Providers.OneTimePasswordProtocolProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
      <Metadata>
        <Item Key="Operation">GenerateCode</Item>
        <Item Key="CodeExpirationInSeconds">1200</Item>
        <Item Key="CodeLength">6</Item>
        <Item Key="CharacterSet">0-9</Item>
        <Item Key="ReuseSameCode">true</Item>
        <Item Key="NumRetryAttempts">5</Item>
      </Metadata>
      <InputClaims>
        <InputClaim ClaimTypeReferenceId="email" PartnerClaimType="identifier" />
      </InputClaims>
      <OutputClaims>
        <OutputClaim ClaimTypeReferenceId="otp" PartnerClaimType="otpGenerated" />
      </OutputClaims>
    </TechnicalProfile>

    <TechnicalProfile Id="VerifyOtp">
      <DisplayName>Verify one time password</DisplayName>
      <Protocol Name="Proprietary" Handler="Web.TPEngine.Providers.OneTimePasswordProtocolProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
      <Metadata>
        <Item Key="Operation">VerifyCode</Item>
      </Metadata>
      <InputClaims>
        <InputClaim ClaimTypeReferenceId="email" PartnerClaimType="identifier" />
        <InputClaim ClaimTypeReferenceId="verificationCode" PartnerClaimType="otpToVerify" />
      </InputClaims>
    </TechnicalProfile>
   </TechnicalProfiles>
</ClaimsProvider>
```

## Add a REST API technical profile

This REST API technical profile generates the email content (using the Sendinblue format).

As with the OTP technical profiles, add the following technical profiles to the `<ClaimsProviders>` element.

```xml
<ClaimsProvider>
  <DisplayName>RestfulProvider</DisplayName>
  <TechnicalProfiles>
    <TechnicalProfile Id="SendOtp">
      <DisplayName>Use Sendinblue email API to send the code the the user</DisplayName>
      <Protocol Name="Proprietary" Handler="Web.TPEngine.Providers.RestfulProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
        <Metadata>
        <Item Key="ServiceUrl">https://api.sendinblue.com/v3/smtp/email</Item>
        <Item Key="AuthenticationType">ApiKeyHeader</Item>
        <Item Key="SendClaimsIn">Body</Item>
        <Item Key="ClaimUsedForRequestPayload">emailRequestBody</Item>
        </Metadata>
        <CryptographicKeys>
            <Key Id="api-key" StorageReferenceId="B2C_1A_sendinblueAPIKey" />
        </CryptographicKeys>
      <InputClaimsTransformations>
        <InputClaimsTransformation ReferenceId="GenerateEmailRequestBody" />
      </InputClaimsTransformations>
      <InputClaims>
        <InputClaim ClaimTypeReferenceId="emailRequestBody" />
      </InputClaims>
    </TechnicalProfile>
  </TechnicalProfiles>
</ClaimsProvider>
```

## Make a reference to the DisplayControl

In the final step, add a reference to the DisplayControl you created. Replace your existing `LocalAccountSignUpWithLogonEmail` and `LocalAccountDiscoveryUsingEmailAddress` self-asserted technical profiles with the following. If you used an earlier version of Azure AD B2C policy. These technical profiles use `DisplayClaims` with a reference to the DisplayControl.

```xml
<ClaimsProvider>
  <DisplayName>Local Account</DisplayName>
  <TechnicalProfiles>
    <TechnicalProfile Id="LocalAccountSignUpWithLogonEmail">
      <Metadata>
        <!--OTP validation error messages-->
        <Item Key="UserMessageIfSessionDoesNotExist">You have exceeded the maximum time allowed.</Item>
        <Item Key="UserMessageIfMaxRetryAttempted">You have exceeded the number of retries allowed.</Item>
        <Item Key="UserMessageIfInvalidCode">You have entered the wrong code.</Item>
        <Item Key="UserMessageIfSessionConflict">Cannot verify the code, please try again later.</Item>
      </Metadata>
      <DisplayClaims>
        <DisplayClaim DisplayControlReferenceId="emailVerificationControl" />
        <DisplayClaim ClaimTypeReferenceId="displayName" Required="true" />
        <DisplayClaim ClaimTypeReferenceId="givenName" Required="true" />
        <DisplayClaim ClaimTypeReferenceId="surName" Required="true" />
        <DisplayClaim ClaimTypeReferenceId="newPassword" Required="true" />
        <DisplayClaim ClaimTypeReferenceId="reenterPassword" Required="true" />
      </DisplayClaims>
    </TechnicalProfile>
    <TechnicalProfile Id="LocalAccountDiscoveryUsingEmailAddress">
      <Metadata>
        <!--OTP validation error messages-->
        <Item Key="UserMessageIfSessionDoesNotExist">You have exceeded the maximum time allowed.</Item>
        <Item Key="UserMessageIfMaxRetryAttempted">You have exceeded the number of retries allowed.</Item>
        <Item Key="UserMessageIfInvalidCode">You have entered the wrong code.</Item>
        <Item Key="UserMessageIfSessionConflict">Cannot verify the code, please try again later.</Item>
      </Metadata>
      <DisplayClaims>
        <DisplayClaim DisplayControlReferenceId="emailVerificationControl" />
      </DisplayClaims>
    </TechnicalProfile>
  </TechnicalProfiles>
</ClaimsProvider>
```

## [Optional] Localize your email

To localize the email, you must send localized strings to Sendinblue, or your email provider. For example, you can localize the email subject, body, your code message, or signature of the email. To do so, you can use the `GetLocalizedStringsTransformation` claims transformation to copy localized strings into claim types. The `GenerateEmailRequestBody` claims transformation, which generates the JSON payload, uses input claims that contain the localized strings.

1. In your policy, define the following string claims: subject, message, codeIntro, and signature.
1. Define a `GetLocalizedStringsTransformation` claims transformation to substitute localized string values into the claims from step 1.
1. Change the `GenerateEmailRequestBody` claims transformation to use input claims with the following XML snippet.
1. Update your Sendinblue template to use dynamic parameters in place of all the strings that will be localized by Azure AD B2C.

    ```xml
    <ClaimsTransformation Id="GetLocalizedStringsForEmail" TransformationMethod="GetLocalizedStringsTransformation">
      <OutputClaims>
        <OutputClaim ClaimTypeReferenceId="subject" TransformationClaimType="email_subject" />
        <OutputClaim ClaimTypeReferenceId="message" TransformationClaimType="email_message" />
        <OutputClaim ClaimTypeReferenceId="codeIntro" TransformationClaimType="email_code" />
        <OutputClaim ClaimTypeReferenceId="signature" TransformationClaimType="email_signature" />
      </OutputClaims>
    </ClaimsTransformation>
    <ClaimsTransformation Id="GenerateEmailRequestBody" TransformationMethod="GenerateJson">
      <InputClaims>
        <InputClaim ClaimTypeReferenceId="email" TransformationClaimType="to.0.email" />
        <InputClaim ClaimTypeReferenceId="subject" TransformationClaimType="params.subject" />
        <InputClaim ClaimTypeReferenceId="otp" TransformationClaimType="params.OTP" />
        <InputClaim ClaimTypeReferenceId="email" TransformationClaimType="params.email" />
        <InputClaim ClaimTypeReferenceId="message" TransformationClaimType="params.message" />
        <InputClaim ClaimTypeReferenceId="codeIntro" TransformationClaimType="params.codeIntro" />
        <InputClaim ClaimTypeReferenceId="signature" TransformationClaimType="params.signature" />
      </InputClaims>
      <InputParameters>
        <InputParameter Id="template_id" DataType="int" Value="1" />
      </InputParameters>
      <OutputClaims>
        <OutputClaim ClaimTypeReferenceId="emailRequestBody" TransformationClaimType="outputClaim" />
      </OutputClaims>
    </ClaimsTransformation>
    ```

1. Add the following `Localization` element.

    ```xml
    <Localization Enabled="true">
      <SupportedLanguages DefaultLanguage="en" MergeBehavior="Append">
        <SupportedLanguage>en</SupportedLanguage>
        <SupportedLanguage>es</SupportedLanguage>
      </SupportedLanguages>
      <LocalizedResources Id="api.custom-email.en">
        <LocalizedStrings>
          <!--Email template parameters-->
          <LocalizedString ElementType="GetLocalizedStringsTransformationClaimType" StringId="email_subject">Contoso account email verification code</LocalizedString>
          <LocalizedString ElementType="GetLocalizedStringsTransformationClaimType" StringId="email_message">Thanks for validating the account</LocalizedString>
          <LocalizedString ElementType="GetLocalizedStringsTransformationClaimType" StringId="email_code">Your code is</LocalizedString>
          <LocalizedString ElementType="GetLocalizedStringsTransformationClaimType" StringId="email_signature">Sincerely</LocalizedString>
        </LocalizedStrings>
      </LocalizedResources>
      <LocalizedResources Id="api.custom-email.es">
        <LocalizedStrings>
          <!--Email template parameters-->
          <LocalizedString ElementType="GetLocalizedStringsTransformationClaimType" StringId="email_subject">Código de verificación del correo electrónico de la cuenta de Contoso</LocalizedString>
          <LocalizedString ElementType="GetLocalizedStringsTransformationClaimType" StringId="email_message">Gracias por comprobar la cuenta de </LocalizedString>
          <LocalizedString ElementType="GetLocalizedStringsTransformationClaimType" StringId="email_code">Su código es</LocalizedString>
          <LocalizedString ElementType="GetLocalizedStringsTransformationClaimType" StringId="email_signature">Sinceramente</LocalizedString>
        </LocalizedStrings>
      </LocalizedResources>
    </Localization>
    ```

1. Add references to the LocalizedResources elements by updating the `ContentDefinitions` element.

    ```XML
    <ContentDefinitions>
      <ContentDefinition Id="api.localaccountsignup">
        <DataUri>urn:com:microsoft:aad:b2c:elements:contract:selfasserted:2.1.0</DataUri>
        <LocalizedResourcesReferences MergeBehavior="Prepend">
          <LocalizedResourcesReference Language="en" LocalizedResourcesReferenceId="api.custom-email.en" />
          <LocalizedResourcesReference Language="es" LocalizedResourcesReferenceId="api.custom-email.es" />
        </LocalizedResourcesReferences>
      </ContentDefinition>
      <ContentDefinition Id="api.localaccountpasswordreset">
        <DataUri>urn:com:microsoft:aad:b2c:elements:contract:selfasserted:2.1.0</DataUri>
        <LocalizedResourcesReferences MergeBehavior="Prepend">
          <LocalizedResourcesReference Language="en" LocalizedResourcesReferenceId="api.custom-email.en" />
          <LocalizedResourcesReference Language="es" LocalizedResourcesReferenceId="api.custom-email.es" />
        </LocalizedResourcesReferences>
      </ContentDefinition>
    </ContentDefinitions>
    ```

1. Finally, add following input claims transformation to the `LocalAccountSignUpWithLogonEmail` and `LocalAccountDiscoveryUsingEmailAddress` technical profiles.

    ```XML
    <InputClaimsTransformations>
      <InputClaimsTransformation ReferenceId="GetLocalizedStringsForEmail" />
    </InputClaimsTransformations>
    ```
    
## [Optional] Localize the UI

The Localization element allows you to support multiple locales or languages in the policy for the user journeys. The localization support in policies allows you to provide language-specific strings for both `Verification display control user interface elements`, and `One time password error messages`. Add the following LocalizedString to your LocalizedResources. 

```XML
<LocalizedResources Id="api.custom-email.en">
  <LocalizedStrings>
    ...
    <!-- Display control UI elements-->
    <LocalizedString ElementType="DisplayControl" ElementId="emailVerificationControl" StringId="intro_msg">Verification is necessary. Please click Send button.</LocalizedString>
    <LocalizedString ElementType="DisplayControl" ElementId="emailVerificationControl" StringId="success_send_code_msg">Verification code has been sent to your inbox. Please copy it to the input box below.</LocalizedString>
    <LocalizedString ElementType="DisplayControl" ElementId="emailVerificationControl" StringId="failure_send_code_msg">We are having trouble verifying your email address. Please enter a valid email address and try again.</LocalizedString>
    <LocalizedString ElementType="DisplayControl" ElementId="emailVerificationControl" StringId="success_verify_code_msg">E-mail address verified. You can now continue.</LocalizedString>
    <LocalizedString ElementType="DisplayControl" ElementId="emailVerificationControl" StringId="failure_verify_code_msg">We are having trouble verifying your email address. Please try again.</LocalizedString>
    <LocalizedString ElementType="DisplayControl" ElementId="emailVerificationControl" StringId="but_send_code">Send verification code</LocalizedString>
    <LocalizedString ElementType="DisplayControl" ElementId="emailVerificationControl" StringId="but_verify_code">Verify code</LocalizedString>
    <LocalizedString ElementType="DisplayControl" ElementId="emailVerificationControl" StringId="but_send_new_code">Send new code</LocalizedString>
    <LocalizedString ElementType="DisplayControl" ElementId="emailVerificationControl" StringId="but_change_claims">Change e-mail</LocalizedString>
    <!-- Claims-->
    <LocalizedString ElementType="ClaimType" ElementId="emailVerificationCode" StringId="DisplayName">Verification Code</LocalizedString>
    <LocalizedString ElementType="ClaimType" ElementId="emailVerificationCode" StringId="UserHelpText">Verification code received in the email.</LocalizedString>
    <LocalizedString ElementType="ClaimType" ElementId="emailVerificationCode" StringId="AdminHelpText">Verification code received in the email.</LocalizedString>
    <LocalizedString ElementType="ClaimType" ElementId="email" StringId="DisplayName">Eamil</LocalizedString>
    <!-- Email validation error messages-->
    <LocalizedString ElementType="ErrorMessage" StringId="UserMessageIfSessionDoesNotExist">You have exceeded the maximum time allowed.</LocalizedString>
    <LocalizedString ElementType="ErrorMessage" StringId="UserMessageIfMaxRetryAttempted">You have exceeded the number of retries allowed.</LocalizedString>
    <LocalizedString ElementType="ErrorMessage" StringId="UserMessageIfInvalidCode">You have entered the wrong code.</LocalizedString>
    <LocalizedString ElementType="ErrorMessage" StringId="UserMessageIfSessionConflict">Cannot verify the code, please try again later.</LocalizedString>
    <LocalizedString ElementType="ErrorMessage" StringId="UserMessageIfVerificationFailedRetryAllowed">The verification has failed, please try again.</LocalizedString>
  </LocalizedStrings>
</LocalizedResources>
```

After you add the localized strings, remove the OTP validation error messages metadata from the LocalAccountSignUpWithLogonEmail and LocalAccountDiscoveryUsingEmailAddress technical profiles.

## Next steps

You can find an example of a custom email verification policy on under [Sendinblue folder](policy/Sendinblue).