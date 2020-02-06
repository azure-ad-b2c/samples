# Azure AD B2C: Dynamic identity provider selection

This sample policy demonstrates how to dynamically filter the list of social identity providers render to the user based on a custom query string parameter `idps`. In the following screenshot user can select from the list of identity providers, such as Facebook, Google+ and Twitter. With Azure AD B2C custom policies, you can configure the technical profiles to be displayed based a claim's value. The  claim value contains the list of identity provider to be rendered.

![IDP Selection](/media/IDPSelection.png)

By default Azure AD B2C displays every identity provider that appears in the `ClaimsProviderSelections` element of the first orchestration step of your user journey. To filter the list of identity providers dynamically, you send a custom query string parameter `idps`, in a comma delimiter format. The following URL illustrates how to display only Facebook and Google sign-in buttons:

https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/B2C_1A_Dynamic_IDP_signup_signin/oauth2/v2.0/authorize?client_id=0239a9cc-309c-4d41-87f1-31288feb2e82&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login&**idps=google,facebook**

## Solution building blocks

1. The `IdentityProviders` **string collection** claim contains the list of identity providers to be displayed.
1. The `idps` **string** claim contains incoming query string parameter `idps`.
1. To convert the `idps` comma delimiter value to a string collection, we use the [StringSplit claims transformation](https://docs.microsoft.com/en-us/azure/active-directory-b2c/string-transformations#stringsplit).
1. The first orchestration step invokes the `Get-IdentityProvidersList`  [claims transofmation technical profile](https://docs.microsoft.com/en-us/azure/active-directory-b2c/claims-transformation-technical-profile). This technical profile reads the `idps` query string parameter, using [claims resolvers ](https://docs.microsoft.com/en-us/azure/active-directory-b2c/claim-resolver-overview), then call the `ConvertIDPsToStringCollection` claims transformation (to convert the comma delimiter string to a string collection).
1. In each technical profile:
    1. The `EnabledForUserJourneys` element set to `OnItemExistenceInStringCollectionClaim`. This element controls if the technical profile is executed in a user journey. The value of the tels B2C to execute only when an item exists in a string collection claim.
    1. You also need to add two metadata elements: `ClaimTypeOnWhichToEnable` specifies the claim's type that is to be evaluated. In this case the string collection claim `identityProviders`. `ClaimValueOnWhichToEnable` specifies the value that is to be compared. The name of the identity provider, for example **facebook**.


```XML
<ClaimsProvider>
  <DisplayName>Facebook</DisplayName>
  <TechnicalProfiles>
    <TechnicalProfile Id="Facebook-OAUTH">
      <Metadata>
        ...
        <Item Key="ClaimTypeOnWhichToEnable">identityProviders</Item>
        <Item Key="ClaimValueOnWhichToEnable">facebook</Item>
      </Metadata>
      ...
      <EnabledForUserJourneys>OnItemExistenceInStringCollectionClaim</EnabledForUserJourneys>
    </TechnicalProfile>
  </TechnicalProfiles>
</ClaimsProvider>

<ClaimsProvider>
  <DisplayName>Google</DisplayName>
  <TechnicalProfiles>
    <TechnicalProfile Id="Google-OAUTH">
      <Metadata>
        ...
        <Item Key="ClaimTypeOnWhichToEnable">identityProviders</Item>
        <Item Key="ClaimValueOnWhichToEnable">google</Item>
      </Metadata>
      ...
      <EnabledForUserJourneys>OnItemExistenceInStringCollectionClaim</EnabledForUserJourneys>
    </TechnicalProfile>
  </TechnicalProfiles>
</ClaimsProvider>
```

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.
