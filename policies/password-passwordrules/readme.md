# Integrate `passwordrules` for Autofill Password Fields

iOS devices support an attribute on password fields called `passwordrules`, which define specific, client-side password rules, such as min/max length, special characters, and restricted characters. You can learn more about `passwordrules` from the [iOS documentation](https://developer.apple.com/documentation/security/password_autofill/customizing_password_autofill_rules), along with a `passwordrules` [generator](https://developer.apple.com/password-rules/) .

## What's Required

This sample uses the following components in order to insert a `passwordrules` attribute into a password field:

- Custom claim (`pwdrules` in this sample)
- ClaimsTransformation Technical Profile to set the rules
- Specifying the `pwdrules` as a query string parameter for loading the custom Sign-Up template
- A custom Sign-Up template provided by an Azure App Service (or any other way to host dynamic content)


### Custom Claim

The `pwdrules` custom claim is defined as such:

```xml
<ClaimsSchema>
    <ClaimType Id="pwdrules">
        <DisplayName>password rules</DisplayName>
        <DataType>string</DataType>
        <UserInputType>Paragraph</UserInputType>
    </ClaimType>
</ClaimsSchema>
```

### ClaimsTransformation Technical Profile

The `pwdrules` claim is populated using a ClaimsTransformation Technical Profile:

```xml
<TechnicalProfile Id="Get-Parameters">
    <DisplayName>Profile to populate calims with parameters</DisplayName>
    <Protocol Name="Proprietary" Handler="Web.TPEngine.Providers.ClaimsTransformationProtocolProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
    <Metadata>
        <Item Key="IncludeClaimResolvingInClaimsHandling">true</Item>
    </Metadata>
    <OutputClaims>
        <OutputClaim ClaimTypeReferenceId="pwdrules" DefaultValue="minlength: 20; maxlength: 100; required: lower; required: upper; required: digit; required: [-];" AlwaysUseDefaultValue="true"/>
    </OutputClaims>
</TechnicalProfile>
```

The `OutputClaim` for `pwdrules` is obtained using the `passwordrules` [generator/validation tool](https://developer.apple.com/password-rules/) from Apple.


### Specifying `pwdrules` as a Query String Parameter

In the RelyingParty, the `pwdrules` claim is specified as a query string parameter, and will be passed to the location of the custom Sign-Up page so that the rules can be injected into the template. The `pwdrules` claim is specified as a Query String parameter using the `ContentDefinitionParameters` section of the Relying Party.

```xml
<RelyingParty>
    <DefaultUserJourney ReferenceId="PasswordRulesSignUpOrSignIn" />
    <UserJourneyBehaviors>
        <JourneyInsights TelemetryEngine="ApplicationInsights" InstrumentationKey="{Settings:AppInsightsKey}" DeveloperMode="true" ClientEnabled="false" ServerEnabled="true" TelemetryVersion="1.0.0" />
        <ContentDefinitionParameters>
            <Parameter Name="pwdrules">{Claim:pwdrules}</Parameter>
        </ContentDefinitionParameters>
        <ScriptExecution>Allow</ScriptExecution>
    </UserJourneyBehaviors>
    <TechnicalProfile Id="PolicyProfile">
        <DisplayName>PolicyProfile</DisplayName>
        <Protocol Name="OpenIdConnect" />
        <OutputClaims>
            <OutputClaim ClaimTypeReferenceId="displayName" />
            <OutputClaim ClaimTypeReferenceId="givenName" />
            <OutputClaim ClaimTypeReferenceId="surname" />
            <OutputClaim ClaimTypeReferenceId="email" DefaultValue="none" />
            <OutputClaim ClaimTypeReferenceId="objectId" PartnerClaimType="sub"/>
            <OutputClaim ClaimTypeReferenceId="identityProvider" DefaultValue="local" />
        </OutputClaims>
        <SubjectNamingInfo ClaimType="sub" />
    </TechnicalProfile>
</RelyingParty>
```

### Custom Sign-Up Template

The sign-up template is obtained from an MVC site as the `passwordrules` attribute needs to be dynamically injected (and so a static storage location, like Azure Storage, isn't available). In this example, the MVC site exposes a controller action for the rules sign-up template, taking the `pwdrules` query string and adding it to the `ViewData` dictionary. The _Layout.cshtml file acquires that value from `ViewData` and then applies it to any `input` controls of type `password` in the template.

This example is very simplistic, and your scenario may be more complicated and could require additional controller actions, additional view layouts, etc.

The MVC site is then referenced by the `ContentDefinition` for sign-up as such:

```xml
<ContentDefinitions>
    <ContentDefinition Id="api.localaccountsignup.rules">
        <LoadUri>https://<your-app-service-name>.azurewebsites.net/rules</LoadUri>
        <RecoveryUri>~/common/default_page_error.html</RecoveryUri>
        <DataUri>urn:com:microsoft:aad:b2c:elements:contract:selfasserted:2.1.10</DataUri>
        <Metadata>
            <Item Key="DisplayName">Local account sign up page</Item>
        </Metadata>
    </ContentDefinition>
</ContentDefinitions>
```

## Deploying the MVC App Considerations

### CORS
One thing to keep in mind is that when deploying the MVC app that provides the Sign-Up, you need to enable CORS for the B2C tenant (https://<your-tenant>.b2clogin.com). If you do not enable CORS, then your template will not load and an error will be thrown when accessing the sign-up page.

## Configure Your Policy

This sample uses the SocialAndLocalAccounts starter pack as a base.


## Discussion

There is a healthy discussion about the `passwordrules` attribute [here](https://github.com/whatwg/html/issues/3518).