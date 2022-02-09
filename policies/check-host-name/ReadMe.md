## Allow/Deny access to Azure AD B2C Custom Policy based on the Hostname


### Overview

This sample provides an example of how to block access to particular B2C policy based on the [Hostname] of the request, e.g. allow requests made to the policy using ```login.contoso.com``` but block ```foo.b2clogin.com```. This is particularly useful when using custom domain(s) with Azure AD B2C tenant and you like to block policy access via default hostname ```*.b2login.com```.


<img width="1485" alt="A diagram visually representing blocking access to default hostname." src="https://user-images.githubusercontent.com/13594864/124837408-c035dd80-df52-11eb-87ce-7cf958ae696d.png">


### Prerequisites
- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance on [storing the extension properties](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [adding the application objectID](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

## How it works 

The technical profile ```CheckIfHostNameIsAllowed``` is invoked as the first step in the user journey and if value of either ```blockAccess_b2clogin``` or ```blockAccess_microsoftonline``` is ```True``` then ```ShowBlockPage``` technical profile is invoked which shows a friendly message to the user.


```
 <OrchestrationSteps>
        <!-- Check to see if the host name is allowed -->
        <OrchestrationStep Order="1" Type="ClaimsExchange">
          <ClaimsExchanges>
            <ClaimsExchange Id="IsAccessAllowed" TechnicalProfileReferenceId="CheckIfHostNameIsAllowed" />
          </ClaimsExchanges>
        </OrchestrationStep>
        <!-- The step 1 will check to see if the host name is b2clogin.com, if yes, then we show a "you are blocked" error page -->
        <OrchestrationStep Order="2" Type="ClaimsExchange">
          <Preconditions>
            <Precondition Type="ClaimEquals" ExecuteActionsIf="true">
              <Value>blockAccess_b2clogin</Value>
              <Value>False</Value>
              <Action>SkipThisOrchestrationStep</Action>
            </Precondition>
          </Preconditions>
          <ClaimsExchanges>
            <ClaimsExchange Id="BlockAccess_b2clogin" TechnicalProfileReferenceId="ShowBlockPage" />
          </ClaimsExchanges>
        </OrchestrationStep>
        <!-- The step 1 will check to see if the host name is microsoftonline.com, if yes, then we show a "you are blocked" error page -->
        <OrchestrationStep Order="3" Type="ClaimsExchange">
          <Preconditions>
            <Precondition Type="ClaimEquals" ExecuteActionsIf="true">
              <Value>blockAccess_microsoftonline</Value>
              <Value>False</Value>
              <Action>SkipThisOrchestrationStep</Action>
            </Precondition>
          </Preconditions>
          <ClaimsExchanges>
            <ClaimsExchange Id="BlockAccess_microsoftonline" TechnicalProfileReferenceId="ShowBlockPage" />
          </ClaimsExchanges>
        </OrchestrationStep>

```


The technical profile ```CheckIfHostNameIsAllowed``` uses [Context:HostName](https://docs.microsoft.com/en-us/azure/active-directory-b2c/claim-resolver-overview#context) claim resolver to capture the hostname of the current request. Two claim transformation rules ```isAccessAllowed_b2clogin``` and ```isAccessAllowed_microsoftonline```, are invoked which sets ```blockAccess_b2clogin``` and  ```blockAccess_microsoftonline``` claims respectively with boolean value of ```True``` or ```False``` which is used later in user journey.

```
<TechnicalProfile Id="CheckIfHostNameIsAllowed">
          <DisplayName>Check if the host (URL) is allowed</DisplayName>
          <Protocol Name="Proprietary" Handler="Web.TPEngine.Providers.ClaimsTransformationProtocolProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
          <Metadata>
            <Item Key="IncludeClaimResolvingInClaimsHandling">true</Item>
          </Metadata>
          <InputClaims>
            <InputClaim ClaimTypeReferenceId="hostName" DefaultValue="{Context:HostName}" AlwaysUseDefaultValue="true" />
            <!-- <InputClaim ClaimTypeReferenceId="allowedHostName" DefaultValue="https://login.consumerbiz.net" AlwaysUseDefaultValue="true" /> -->
          </InputClaims>
          <OutputClaims>
            <OutputClaim ClaimTypeReferenceId="blockAccess_microsoftonline" />
            <OutputClaim ClaimTypeReferenceId="blockAccess_b2clogin" />
          </OutputClaims>
          <OutputClaimsTransformations>
            <OutputClaimsTransformation ReferenceId="isAccessAllowed_microsoftonline" />
            <OutputClaimsTransformation ReferenceId="isAccessAllowed_b2clogin" />
          </OutputClaimsTransformations>
          <UseTechnicalProfileForSessionManagement ReferenceId="SM-Noop" />
        </TechnicalProfile>
```


```
 <ClaimsTransformation Id="isAccessAllowed_b2clogin" TransformationMethod="StringContains">
        <InputClaims>
          <InputClaim ClaimTypeReferenceId="hostName" TransformationClaimType="inputClaim" />
        </InputClaims>
        <InputParameters>
          <InputParameter Id="contains" DataType="string" Value="foo.b2clogin.com" />
          <InputParameter Id="ignoreCase" DataType="string" Value="true" />
        </InputParameters>
        <OutputClaims>
          <OutputClaim ClaimTypeReferenceId="blockAccess_b2clogin" TransformationClaimType="outputClaim" />
        </OutputClaims>
  </ClaimsTransformation>
```

```
<ClaimsTransformation Id="isAccessAllowed_microsoftonline" TransformationMethod="StringContains">
        <InputClaims>
          <InputClaim ClaimTypeReferenceId="hostName" TransformationClaimType="inputClaim" />
        </InputClaims>
        <InputParameters>
          <InputParameter Id="contains" DataType="string" Value="login.microsoftonline.com" />
          <InputParameter Id="ignoreCase" DataType="string" Value="true" />
        </InputParameters>
        <OutputClaims>
          <OutputClaim ClaimTypeReferenceId="blockAccess_microsoftonline" TransformationClaimType="outputClaim" />
        </OutputClaims>
      </ClaimsTransformation>
```

### FAQ

* Can you configure allow/block logic across all polices in the tenant?
  
  _This is a policy level configuration so has to be implemented at the policy level_

* Does Azure AD B2C userflow supports allow/block request based on the hostname?

  _Currently you can only implement this functionality within Azure AD B2C custom policy (IEF Framework)_
