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

A [live version of this policy](https://b2cprod.b2clogin.com/b2cprod.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_Captcha_signin&client_id=51d907f8-db14-4460-a1fd-27eaeb2a74da&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) is available to test.

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 

## Implementation notes:

In this proof-of-concept implementation, Google reCAPTCHA v2 checkbox is being used.

The links below provide more information about Google reCAPTCHA versions:
[reCAPTCHA Developer's Guide](https://developers.google.com/recaptcha/intro)
[reCAPTCHA v2](https://developers.google.com/recaptcha/docs/display)

Additionally, a “Captcha Handler API” needs to be built to help to handle the captcha integration.  A sample API solution can be found in [policies/captcha-integration/Captcha-Handler-API](https://github.com/azure-ad-b2c/samples/tree/master/policies/captcha-integration/Captcha-Handler-API)

For proof of concept, instead of building an independent API solution, an Azure Function API can be substituted as handler. Azure Function API code can be found in [policies/captcha-integration/Captcha-Handler-API-AzFunc/run.csx](https://github.com/azure-ad-b2c/samples/blob/master/policies/captcha-integration/Captcha-Handler-API-AzFunc/run.csx)

Detailed steps to create a Captcha Handler API with Azure Function App are also mentioned below.

### How does reCAPTCHA token verification work?

1, The client (application) presents a “verification request” aka reCAPTCHA token request to the Google reCAPTCHA services. This is normally presented as a challenge.

2, Google reCAPTCHA services responds by providing a captcha token.

3, The client application form submits a request along with the captcha token to the “Captcha handler API” endpoint (Eg: Azure Function API endpoint).

4, The handler API sends a request to verify reCAPTCHA token to Google reCAPTCHA services.

5, Google will provide a verification result (Captcha verified “yes” / Captcha not verified “no”) back to the handler.

6, The handler API will then provide a response back to the client application. Hence a success or failure message after verification is returned to the client.

### Steps to implement a Sample Google reCAPTCHA Solution:


#### Step 1: Generate the Captcha Keys:

1, Navigate to [reCAPTCHA Admin console](https://www.google.com/recaptcha/admin) -> login with valid google account

2, Provide “Label” (friendly name for Captcha)

3, Select reCAPTCHA type Challenge v2 | “I’m not a robot Checkbox”
 ![image](https://github.com/azure-ad-b2c/samples/assets/129459714/a0364102-9819-45ce-9081-fde2fa599d68)
 
4, Under Domains, add domain name “yourtenant.b2clogin.com” (or add custom domain name if you are using custom domains for accessing b2c)

5, Agree to the terms and conditions -> Submit. Copy the generated “Site Key” and “Secret Key” as this is needed later for captcha integration.


#### Step 2: Build Captcha Handler API endpoint with Azure Function App:

*****Create an Azure Function App:*****

1, Login to Azure Portal -> Create a resource -> Web -> choose “Function App” -> Create

2, Select the “Subscription” and “Resource Group”

3, Provide an apt name for “Function App name” (Eg: contosoapp)

4, Select “Code” for deployment -> choose .NET for runtime stack -> provide version and region details

5, Choose the Operating System (Eg: Windows) and Hosting plan (Eg: Consumption) -> Next (Storage)

6, Provide a “Storage account” -> Next (Networking) -> select “On” for “Enable public access” -> Next (Monitoring), choose default options for “Monitoring”, “Deployment” , mention “Tags” if needed -> “Review + create”

*****Configure the function app as a Captcha Handler API by defining a function:*****

1, In Azure Portal, navigate to the Azure Function App -> Overview -> Functions -> Create -> Select Template “HTTP trigger” -> Create a New Function (Eg: HttpTrigger1), choose “Anonymous” Authorization level

2, Open the function (Eg: HttpTrigger1) -> Under “Developer”, choose “Code + Test” -> choose “run.csx” from the drop down, paste the code in “run.csx” file in the sample [policies/captcha-integration/Captcha-Handler-API-AzFunc/run.csx](https://github.com/azure-ad-b2c/samples/blob/master/policies/captcha-integration/Captcha-Handler-API-AzFunc/run.csx) and save
  
3, Go to the Function (Eg: HttpTrigger1) -> Overview, ensure it is enabled. 

4, Go back to the Function App -> Settings -> Configuration -> go to “Application settings” create “New application setting” -> provide name as “B2C_Recaptcha_Secret” -> Provide the “Secret Key” value given earlier by Google reCAPTCHA Services.

5, Ensure Function App has been started (Status: Running) from the Overview tab. 

6, In this case the Captcha Handler API endpoint will be (https://function-app-name.azurewebsites.net/api/function-name) . Replace the placeholders with your function app name and function name (Eg:   https://contosoapp.azurewebsites.net/api/HttpTrigger1 ) . Keep a note of this URI as this needs to be defined later in the policy.


#### Step 3: Build the custom policy:

1, Use the B2C SocialAndLocalAccounts Starter Pack to build the policy. The Starter Pack can be downloaded from [SocialAndLocalAccounts](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/main/SocialAndLocalAccounts)
Integrate “Captcha_TrustFrameworkExtensions.xml” and Signin.xml”

2, Update the “data-sitekey” value in “customCaptcha.html” page with “Site Key” value generated earlier from reCAPTCHA Admin console. Also update “href” elements with proper b2c policy URLs as needed.

3, Ensure “customCaptcha.html” is properly hosted and defined in policy file (Eg: ensure “LoadUri” elements under “ContentDefinitions” in policy file “Captcha_TrustFrameworkExtensions.xml” are properly updated as needed)

4, Update the “ServiceUrl” metadata element for “login-Recaptcha” Technical profile with the Azure Function Captcha Handler API endpoint URI created earlier.

5, Upload the policies and test the custom policy

*******Additional References:*******

[Tutorial - Create user flows and custom policies - Azure Active Directory B2C | Microsoft Learn](https://learn.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-user-flows?pivots=b2c-custom-policy)

[Azure Active Directory B2C custom policy overview | Microsoft Learn](https://learn.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview)
