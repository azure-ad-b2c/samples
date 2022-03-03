# Sign-in and Sign-up policy with direct link to sign-up

This sample adds a direct link to the sign-up page. A relying party application can include a query string parameter `option=signup` that takes the user directly  to the sign-up page

## Live demo

To checkout the user experience of this policy, follow these steps:

1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_SUSI_Username/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&option=signup) the *B2C_1A_Demo_SignUpSignin_DeepLink* policy, which takes you directly to the sign-up page.

## Scenario

To add the direct link to the sign-up page:

1. At the begging of the sign-up and sign-in user journey, add an orchestration step that reads the `option` claim resolver. The `Get-sign-up-claim-resolver` technical profile also checks the value of the option claim, and set the value of the Boolean `signUpFlow` claim.
    
    ```xml
    <OrchestrationStep Order="1" Type="ClaimsExchange">
      <ClaimsExchanges>
        <ClaimsExchange Id="Get-sign-up-claim-resolver" TechnicalProfileReferenceId="Get-sign-up-claim-resolver" />
      </ClaimsExchanges>
    </OrchestrationStep>
    ```

1. Then add a new orchestration step that invokes the `LocalAccountSignUpWithLogonEmail` sign-up technical profile. This steps runs only if the `signUpFlow` is `True`.

    ```xml
    <OrchestrationStep Order="2" Type="ClaimsExchange">
      <Preconditions>
        <Precondition Type="ClaimEquals" ExecuteActionsIf="true">
          <Value>signUpFlow</Value>
          <Value>False</Value>
          <Action>SkipThisOrchestrationStep</Action>
        </Precondition>
      </Preconditions>
      <ClaimsExchanges>
        <ClaimsExchange Id="SignUpExchange" TechnicalProfileReferenceId="LocalAccountSignUpWithLogonEmail" />
      </ClaimsExchanges>
    </OrchestrationStep>
    ```

1. Add a precondition to the the `CombinedSignInAndSignUp` orchestration step. Make is run only if the  `signUpFlow` is `False`.

    ```xml
    <OrchestrationStep Order="3" Type="CombinedSignInAndSignUp" ContentDefinitionReferenceId="api.signuporsignin">
      <Preconditions>
        <Precondition Type="ClaimEquals" ExecuteActionsIf="true">
          <Value>signUpFlow</Value>
          <Value>True</Value>
          <Action>SkipThisOrchestrationStep</Action>
        </Precondition>
      </Preconditions>
      <ClaimsProviderSelections>
        <ClaimsProviderSelection TargetClaimsExchangeId="FacebookExchange" />
        <ClaimsProviderSelection ValidationClaimsExchangeId="LocalAccountSigninEmailExchange" />
      </ClaimsProviderSelections>
      <ClaimsExchanges>
        <ClaimsExchange Id="LocalAccountSigninEmailExchange" TechnicalProfileReferenceId="SelfAsserted-LocalAccountSignin-Email" />
      </ClaimsExchanges>
    </OrchestrationStep>
    ```

## Set the authorization request

To direct the user to the sign-up page, in the authorization request add the `option=signup` query string parameter. 

The following example takes the user to the landing page (sign-up or sign-in):

```http
https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_SignUp_SignIn_with_link_to_sign_up&client_id=63ba0d17-c4ba-47fd-89e9-31b3c2734339&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token
```

If you add the `option=signup` parameter, the user is taken directly to the sign-up page.

```http
https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_SignUp_SignIn_with_link_to_sign_up&client_id=63ba0d17-c4ba-47fd-89e9-31b3c2734339&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&option=signup`
```

## Notes

This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
