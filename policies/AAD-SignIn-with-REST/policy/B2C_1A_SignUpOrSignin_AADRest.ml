<?xml version="1.0" encoding="utf-8" ?>
<TrustFrameworkPolicy xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
  xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
  xmlns="http://schemas.microsoft.com/online/cpim/schemas/2013/06" PolicySchemaVersion="0.3.0.0" TenantId="yourtenant.onmicrosoft.com" PolicyId="B2C_1A_SignUpOrSignin_AADRest" PublicPolicyUri="http://yourtenant.onmicrosoft.com/B2C_1A_SignUpOrSignin_AADRest" DeploymentMode="Development">

  <BasePolicy>
    <TenantId>yourtenant.onmicrosoft.com</TenantId>
    <PolicyId>B2C_1A_TrustFrameworkExtensions</PolicyId>
  </BasePolicy>

  <BuildingBlocks>
    <ClaimsSchema>
      <ClaimType Id="clientNumber">
        <DisplayName>Client CRM Number</DisplayName>
        <DataType>string</DataType>
        <UserHelpText/>
      </ClaimType>
      <ClaimType Id="clientOrg">
        <DisplayName>Clients CRM Organisational Unit</DisplayName>
        <DataType>string</DataType>
        <UserHelpText/>
      </ClaimType>        
    </ClaimsSchema>
  </BuildingBlocks>


  <ClaimsProviders>
    <ClaimsProvider>
      <DisplayName>Get CRM Data</DisplayName>
      <TechnicalProfiles>
        <!-- Example REST Call to CRM -->
        <TechnicalProfile Id="REST-GetCRMData">
          <DisplayName></DisplayName>
          <Protocol Name="Proprietary" Handler="Web.TPEngine.Providers.RestfulProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
          <Metadata>
            <Item Key="ServiceUrl">https://yourfunction.azurewebsites.net/api/Echo</Item>
            <Item Key="AuthenticationType">None</Item>
            <Item Key="SendClaimsIn">QueryString</Item>
          </Metadata>
          <InputClaims>
            <InputClaim ClaimTypeReferenceId="clientNumber" DefaultValue="123456789" />
            <InputClaim ClaimTypeReferenceId="clientOrg" DefaultValue="Accounting" />
          </InputClaims>
          <OutputClaims>
            <OutputClaim ClaimTypeReferenceId="clientNumber" />
            <OutputClaim ClaimTypeReferenceId="clientOrg" />
          </OutputClaims>
          <UseTechnicalProfileForSessionManagement ReferenceId="SM-Noop" />
        </TechnicalProfile>
      </TechnicalProfiles>
    </ClaimsProvider>

    <ClaimsProvider>
      <Domain>Contoso</Domain>
      <DisplayName>Login using Contoso</DisplayName>
      <TechnicalProfiles>
        <TechnicalProfile Id="OIDC-Contoso">
          <DisplayName>Contoso Employee</DisplayName>
          <Description>Login with your Contoso account</Description>
          <Protocol Name="OpenIdConnect"/>
          <Metadata>
            <Item Key="METADATA">https://login.microsoftonline.com/tenant-name.onmicrosoft.com/v2.0/.well-known/openid-configuration</Item>
            <Item Key="client_id">00000000-0000-0000-0000-000000000000</Item>
            <Item Key="response_types">code</Item>
            <Item Key="scope">openid profile</Item>
            <Item Key="response_mode">form_post</Item>
            <Item Key="HttpBinding">POST</Item>
            <Item Key="UsePolicyInRedirectUri">false</Item>
          </Metadata>
          <CryptographicKeys>
            <Key Id="client_secret" StorageReferenceId="B2C_1A_ContosoAppSecret"/>
          </CryptographicKeys>
          <OutputClaims>
            <OutputClaim ClaimTypeReferenceId="issuerUserId" PartnerClaimType="oid"/>
            <OutputClaim ClaimTypeReferenceId="tenantId" PartnerClaimType="tid"/>
            <OutputClaim ClaimTypeReferenceId="givenName" PartnerClaimType="given_name" />
            <OutputClaim ClaimTypeReferenceId="surName" PartnerClaimType="family_name" />
            <OutputClaim ClaimTypeReferenceId="email" PartnerClaimType="email" />
            <OutputClaim ClaimTypeReferenceId="displayName" PartnerClaimType="name" />
            <OutputClaim ClaimTypeReferenceId="authenticationSource" DefaultValue="socialIdpAuthentication" AlwaysUseDefaultValue="true" />
            <OutputClaim ClaimTypeReferenceId="identityProvider" PartnerClaimType="iss" />
          </OutputClaims>
          <OutputClaimsTransformations>
            <OutputClaimsTransformation ReferenceId="CreateRandomUPNUserName"/>
            <OutputClaimsTransformation ReferenceId="CreateUserPrincipalName"/>
            <OutputClaimsTransformation ReferenceId="CreateAlternativeSecurityId"/>
            <OutputClaimsTransformation ReferenceId="CreateSubjectClaimFromAlternativeSecurityId"/>
          </OutputClaimsTransformations>
          <UseTechnicalProfileForSessionManagement ReferenceId="SM-SocialLogin"/>
        </TechnicalProfile>
      </TechnicalProfiles>
    </ClaimsProvider>

  </ClaimsProviders>

  <UserJourneys>
    <UserJourney Id="SignUpOrSignIn_AAD_REST">
      <OrchestrationSteps>
        <OrchestrationStep Order="1" Type="ClaimsExchange">
          <ClaimsExchanges>
            <ClaimsExchange Id="GetAccessToken" TechnicalProfileReferenceId="OIDC-Contoso"/>
          </ClaimsExchanges>
        </OrchestrationStep>
        <OrchestrationStep Order="2" Type="ClaimsExchange">
          <ClaimsExchanges>
            <ClaimsExchange Id="GetSecureData" TechnicalProfileReferenceId="REST-GetCRMData"/>
          </ClaimsExchanges>
        </OrchestrationStep>
        <OrchestrationStep Order="3" Type="SendClaims" CpimIssuerTechnicalProfileReferenceId="JwtIssuer" />
      </OrchestrationSteps>
    </UserJourney>
  </UserJourneys>

  <RelyingParty>
    <DefaultUserJourney ReferenceId="SignUpOrSignIn_AAD_REST" />
    <TechnicalProfile Id="PolicyProfile">
      <DisplayName>PolicyProfile</DisplayName>
      <Protocol Name="OpenIdConnect" />
      <OutputClaims>
      <OutputClaim ClaimTypeReferenceId="issuerUserId" PartnerClaimType="sub"/>
        <OutputClaim ClaimTypeReferenceId="givenname"/>
        <OutputClaim ClaimTypeReferenceId="surName"/>
        <OutputClaim ClaimTypeReferenceId="email"/>
        <OutputClaim ClaimTypeReferenceId="clientNumber" />
        <OutputClaim ClaimTypeReferenceId="clientOrg" />
        <OutputClaim ClaimTypeReferenceId="tenantId" AlwaysUseDefaultValue="true" DefaultValue="{Policy:TenantObjectId}" />
      </OutputClaims>
      <SubjectNamingInfo ClaimType="sub" />
    </TechnicalProfile>
  </RelyingParty>
</TrustFrameworkPolicy>
