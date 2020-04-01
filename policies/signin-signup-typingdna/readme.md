# Integrating Azure AD B2C with TypingDNA

## Introduction

This scenario demonstrates how to integrate TypingDNA as a PSD2 SCA compliant authentication factor. Find more about TypingDNA [here](https://www.typingdna.com/).

Azure AD B2C utilizes TypingDNA's technologies to capture the users typing characteristics and have them recorded and analysed for familiarity on each authentication. This can add a layer of protection pertaining to the risky-ness of an authentication. The risk level can be evaluated and Azure AD B2C can invoke other mechanisms to provide further confidence the user is who they claim to be. This can be by invoking Azure MFA, forcing email verification, or any other custom logic for your scenario.

> Note:  This sample policy is based on [SocialAndLocalAccountsWithMfa starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa).


## How it works

- Azure AD B2C pages use TypingDNA's javascript library to record the users typing pattern. In this example the username and password are recorded at sign up for the initial enrolment, and then on every sign in for verification.

- When the user submits the page, the TypingDNA library will compute the typing characteristic of the user, and inserts this into a hidden text field which Azure AD B2C has rendered. This field is hidden with CSS.

- The sample contains HTML files with the JavaScript and CSS modifications, and is referenced by the `api.selfasserted.tdnasignin` and `api.selfasserted.tdnasignup` content definitions. Follow [this](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-ui-customization#hosting-the-page-content) to host your HTML files.

- Azure AD B2C now has the typing pattern within the claimbag when the user submits their credentials. It must call an API (yours) to pass this data to the TypingDNA REST API endpoint. This API is included in the sample (TypingDNA-API-Interface). 
At sign up, the [Check User](https://api.typingdna.com/index.html#api-API_Services-GetUser) endpoint is called to confirm the user does not exist. Then the [Save Pattern](https://api.typingdna.com/index.html#api-API_Services-saveUserPattern) endpoint is called to save the users first typing pattern.

> Note: All calls to the TypingDNA REST API endpoint send a UserId. This must be a hashed value. Azure AD B2C uses the `HashObjectIdWithEmail` claims transformation to hash the email with a random salt and secret.

The REST API calls are modelled with validationTechnicalProfiles within `LocalAccountSignUpWithLogonEmail-TDNA`:
```xml
<ValidationTechnicalProfiles>
    <ValidationTechnicalProfile ReferenceId="AAD-UserWriteUsingLogonEmail-TDNA" />
    <ValidationTechnicalProfile ReferenceId="REST-TDNA-CheckUser" ContinueOnError="true"/>
    <ValidationTechnicalProfile ReferenceId="REST-TDNA-SaveUser"/>
</ValidationTechnicalProfiles>
```

- At subsequent Sign In's, the users typing pattern is computed in the same manner as at Sign Up using the custom HTML. Once the typing profile is within the Azure AD B2C claimbag, Azure AD B2C will call your API to call TypingDNA's REST API endpoint. The [Check User](https://api.typingdna.com/index.html#api-API_Services-GetUser) endpoint is called to confirm the user exists. Then the [Verify Pattern](https://api.typingdna.com/index.html#api-API_Services-verifyTypingPattern) endpoint is called to return the `net_score`. This is an indication of how close the typing pattern was to the original at Sign Up.

This is modelled with validationTechnicalProfiles within `SelfAsserted-LocalAccountSignin-Email-TDNA`:
```xml
<ValidationTechnicalProfiles>
    <ValidationTechnicalProfile ReferenceId="login-NonInteractive"/>
    <ValidationTechnicalProfile ReferenceId="REST-TDNA-CheckUser" ContinueOnError="false"/>
    <ValidationTechnicalProfile ReferenceId="REST-TDNA-VerifyUser"/>
    <ValidationTechnicalProfile ReferenceId="REST-TDNA-SaveUser">
        <Preconditions>
            <Precondition Type="ClaimEquals" ExecuteActionsIf="true">
            <Value>saveTypingPattern</Value>
            <Value>False</Value>
            <Action>SkipThisValidationTechnicalProfile</Action>
            </Precondition>
        </Preconditions>
    </ValidationTechnicalProfile>
</ValidationTechnicalProfiles>
```

- If the user obtains a typing pattern that has a high `net_score`, you can save this using the TypingDNA [Save Typing Pattern](https://api.typingdna.com/index.html#api-API_Services-saveUserPattern) endpoint. 

Your API must return a claim  `saveTypingPattern` if you would like the TypingDNA Save Typing Pattern endpoint to be called by Azure AD B2C (via your API).

- The example in the repo contains an API (TypingDNA-API-Interface) which is configured with the following properties.
1. Training mode - If the user has less than 2 patterns saved, always prompt for MFA.
1. If the user has 2-5 patterns saved, and the `net_score` is lower than 50, prompt for MFA.
1. If the user has 5+ patterns saved, and the `net_score` is lower than 65, prompt for MFA.

These thresholds should be adjusted on your use case.

- After your API has evaluated the `net_score`, it should return a boolean claim to B2C - `promptMFA`.

- The `promptMFA` claim is used with in a precondition to conditionally execute Azure MFA.
```xml
<OrchestrationStep Order="3" Type="ClaimsExchange">
    <Preconditions>
        <Precondition Type="ClaimsExist" ExecuteActionsIf="true">
            <Value>isActiveMFASession</Value>
            <Action>SkipThisOrchestrationStep</Action>
        </Precondition>
        <Precondition Type="ClaimEquals" ExecuteActionsIf="true">
            <Value>promptMFA</Value>
            <Value>False</Value>
            <Action>SkipThisOrchestrationStep</Action>
        </Precondition>
    </Preconditions>
    <ClaimsExchanges>
        <ClaimsExchange Id="PhoneFactor-Verify" TechnicalProfileReferenceId="PhoneFactor-InputOrVerify" />
    </ClaimsExchanges>
</OrchestrationStep>
```

## Quick setup instructions
- Setup the Azure AD B2C starter pack as described [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-get-started?tabs=applications)
- Sign up for TypingDNA [here](https://www.typingdna.com/)
- Host the TypingDNA-API-Interface at your hosting provider of choice
- Replace all instances of `apiKey` and `apiSecret` in TypingDNA-API-Interface solution with the credentials from your TypingDNA dashboard
- Host the HTML files at your provider of choice following the CORS requirements [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-ui-customization#3-configure-cors)
- Replace the LoadURI elements for the `api.selfasserted.tdnasignup` and `api.selfasserted.tdnasignin` content definitions in the TrustFrameworkExtensions file to the URI of your hosted HTML files respectively.
- Create a B2C Policy key under Identity Experience Framework in the Azure AD Blade at the [Azure Portal](https://portal.azure.com). Use the `Generate` option and name this key `tdnaHashedId`.
- Replace the TenantId's in the policy files 
- Replace the ServiceURLs in all TypingDNA REST API Technical profiles (REST-TDNA-VerifyUser, REST-TDNA-SaveUser, REST-TDNA-CheckUser) with the endpoint for your TypingDNA-API-Interface API.
- Upload policy files to your tenant

## Live version
- MFA has been disabled in this test version, but you can see the result on whether MFA would have been prompted by the claim `promptMFA` after authentication.

- Sign up [here](https://b2cprod.b2clogin.com/b2cprod.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_SU_TDNA&client_id=51d907f8-db14-4460-a1fd-27eaeb2a74da&nonce=defaultNonce&redirect_uri=https://jwt.ms/&scope=openid&response_type=id_token&prompt=login) and Sign in [here](https://b2cprod.b2clogin.com/b2cprod.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_SI_TDNA&client_id=51d907f8-db14-4460-a1fd-27eaeb2a74da&nonce=defaultNonce&redirect_uri=https://jwt.ms/&scope=openid&response_type=id_token&prompt=login)

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].

If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).

To provide product feedback, visit the [Azure Active Directory B2C Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).