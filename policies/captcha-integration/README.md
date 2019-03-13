# A B2C IEF Custom Policy which integrates with Google Captcha

## Disclaimer
The sample policy is developed and managed by the open-source community in GitHub. This policy is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The policy is provided AS IS without warranty of any kind.

### Scenario
This set of policies demonstrates how to integrate Google Captcha into the Sign In page.

* JavaScript is used to embed the Captcha control.
The Captcha response is inserted into a hidden field which the `SelfAsserted-LocalAccountSignin-Email` technical profile exposes to the sign in page.
* The `SelfAsserted-LocalAccountSignin-Email` technical profile retrieves the Captcha response from the hidden field and validates the blob against the Google servers using the `login-Recaptcha` validation technical profile. This technical profile is a REST API call to the Captcha API.
* If the response from the Google server is successful, B2C continues to validate the credentials against the directory using `login-noninteractive`. 
* Otherwise, the API responds back to AAD B2C indicating the Captcha was invalid and must try again. The error can be changed in the API source.
* Each time the user submits the page, the Captcha is reset using JavaScript.

Test a live version of this policy [here](https://b2cprod.b2clogin.com/b2cprod.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_Captcha_signin&client_id=51d907f8-db14-4460-a1fd-27eaeb2a74da&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login).