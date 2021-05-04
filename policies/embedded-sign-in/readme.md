# Sign-up and Sign-in with embedded sign-in

By default when you create a sign-up or sign-in policy, it cannot be embedded in an iframe within your application. To allow your Azure AD B2C user interface to be embedded in an iframe, a content security policy Content-Security-Policy and frame options X-Frame-Options must be included in the Azure AD B2C HTTP response headers. These headers allow the Azure AD B2C user interface to run under your application domain name.

This policy adds a JourneyFraming element inside the RelyingParty element. The UserJourneyBehaviors element must follow the DefaultUserJourney. Your UserJourneyBehaviors element should look like this example:

```xml
<!--
<RelyingParty>
  <DefaultUserJourney ReferenceId="SignUpOrSignIn" /> -->
  <UserJourneyBehaviors> 
    <JourneyFraming Enabled="true" Sources="https://somesite.com https://anothersite.com" /> 
  </UserJourneyBehaviors>
<!--
</RelyingParty> -->
```

## Steps to Use the Policy

>Note: Because Azure AD B2C session cookies within an iframe are considered third-party cookies, certain browsers (for example Safari or Chrome in incognito mode) either block or clear these cookies, resulting in an undesirable user experience. To prevent this issue, make sure your application domain name and your Azure AD B2C domain have the same origin. To use the same origin, [enable custom domains](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-domain) for Azure AD B2C tenant, then configure your web app with the same origin. For example, an application hosted on 'https://app.contoso.com' has the same origin as Azure AD B2C running on 'https://login.contoso.com'.

### Customizations in TrustFrameworkBaseEmbedded.xml

- Modify the `TenantId="yourtenant.onmicrosoft.com"` element in the `<TrustFrameworkPolicy>` tag.
- Modify the `PublicPolicyUri="http://yourtenant.onmicrosoft.com/B2C_1A_TrustFrameworkBase_Embedded"` element in the `<TrustFrameworkPolicy>` tag. If you're using a custom domain, this should be similar to 'http://login.yourcustomdomain.com/policyname' rather than 'http://yourtenant.onmicrosoft.com/policyname'.

### Customizations in TrustFrameworkExtensionsEmbedded.xml

- Modify the `TenantId="yourtenant.onmicrosoft.com"` element in the `<TrustFrameworkPolicy>` tag.
- Modify the `PublicPolicyUri="http://yourtenant.onmicrosoft.com/B2C_1A_TrustFrameworkExtensions_Embedded"` element in the `<TrustFrameworkPolicy>` tag. If you're using a custom domain, this should be similar to 'http://login.yourcustomdomain.com/policyname' rather than 'http://yourtenant.onmicrosoft.com/policyname'.
- Modify the `<TenantId>yourtenant.onmicrosoft.com</TenantId>` tag in the `<BasePolicy>`.
- Changes all text instances of the `ProxyIdentityExperienceFrameworkAppId` and `IdentityExperienceFrameworkAppId` with the Application Client Ids of the **ProxyIdentityExperienceFramework** and the **IdentityExperienceFramework** respectively.

### Customizations in SignUpOrSignInEmbedded.xml

- Modify the `TenantId="yourtenant.onmicrosoft.com"` element in the `<TrustFrameworkPolicy>` tag.
- Modify the `PublicPolicyUri="http://yourtenant.onmicrosoft.com/B2C_1A_TrustFrameworkExtensions_Embedded"` element in the `<TrustFrameworkPolicy>` tag. If you're using a custom domain, this should be similar to 'http://login.yourcustomdomain.com/policyname' rather than 'http://yourtenant.onmicrosoft.com/policyname'.
- Modify the `<TenantId>yourtenant.onmicrosoft.com</TenantId>` tag in the `<BasePolicy>`.
- Modify the `Sources="https://somesite.com https://anothersite.com"` element in the `<JourneyFraming>` tag. This should contain a list of origins which are allowed to load this policy in an iframe.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

> Note:  This sample policy is based on [LocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/LocalAccounts).
