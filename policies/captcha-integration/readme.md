# A B2C IEF Custom Policy which integrates with Google Captcha

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

### Scenario
This set of policies demonstrates how to integrate Google Captcha into the Sign In page.

* JavaScript is used to embed the Captcha control.
The Captcha response is inserted into a hidden field which the `SelfAsserted-LocalAccountSignin-Email` technical profile exposes to the sign in page.
* The `SelfAsserted-LocalAccountSignin-Email` technical profile retrieves the Captcha response from the hidden field and validates the blob against the Google servers using the `login-Recaptcha` validation technical profile. This technical profile is a REST API call to the Captcha API.
* If the response from the Google server is successful, B2C continues to validate the credentials against the directory using `login-noninteractive`. 
* Otherwise, the API responds back to AAD B2C indicating the Captcha was invalid and must try again. The error can be changed in the API source.
* Each time the user submits the page, the Captcha is reset using JavaScript.

Test a live version of this policy [here](https://b2cprod.b2clogin.com/b2cprod.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_Captcha_signin&client_id=51d907f8-db14-4460-a1fd-27eaeb2a74da&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login).

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 