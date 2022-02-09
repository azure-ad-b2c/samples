## Community Help and Support
Use Stack Overflow to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c]. If you find a bug in the sample, please raise the issue on GitHub Issues. To provide product feedback, visit the Azure Active Directory B2C Feedback page.

## How is your Azure AD B2C Sign In doing?
Monitoring availability and responsiveness of Azure AD B2C Sign-in’s using the Azure Application Web test feature of Azure Application Insights.

If you need to run continuous tests to monitor the availability and responsiveness of sign-in’s (username and password only - non-MFA) in Azure AD B2C, for a stand-alone Sign In policy,  or the Sign-in portion of a combined Sign-In-And-Sign-Up policy, then this solution may meet your needs.  This web test can also be modified for testing B2C Profile Edit, or other self-asserted (user input) based policies.   

If you need similar web tests for Sign-In’s using MFA (requiring access to phone based SMS and phone based authenticator app OTPs), or for Self-Service Password Resets or Sign-up’s (involving receipt of codes via email), then this sample will need to be modified with additional steps (to handle these secondary user inputs from Emails, Authenticator app etc).

This sample web test solution uses the Multi-step web test [capabilities of Azure Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/availability-multistep).


1. Download the signIn.webtest sample from this repository.

2. Follow the [instructions for creating a multi-step webtest](https://docs.microsoft.com/en-us/azure/azure-monitor/app/availability-multistep) using Visual Studio.  

3. After your initial webtest solution is created in Visual Studio, right click on your project, and select Add – Existing Item, and select the signIn.Webtest file that you downloaded. Open the SignIn.webtest, and edit the following sections and properties. Update configuration for your B2C Tenant testing.  
4. From within Visual Studio (VS), open the SignIn.webtest (under References in VS Solution Explorer).  

Under the Context Parameters,  enter your values:

       Test User Account
       * B2C Username and Password (credential for a B2C test user account used for the sign-in test)
       
       B2C Policy that you would like to test
       * PolicyID – this is your B2C policy that you wish to test: e.g. B2C_1_SignInSignUp
       
       B2C Application – values for the registered B2C application that represents the web test
       * Client ID – this is the B2C application ID – e.g. c823f166-8b0d-4937-9e27-be39930c0588 
       * Redirect URI – this is the B2C application Redirect URI value e.g. https://jwt.ms 
       
5. Update each steps URL endpoints' with your tenant and B2C policy names.

 a. Replace each instance of “ENTER-YOUR-TENANT-NAME” with you b2c tenant name e.g. `contoso.onmicrosoft.com`

 b. Replace each instance of “ENTER-YOUR-B2C-POLICY-NAME” with your policy e.g. `B2C_1A_SUSI`

6. Save the changes to signIn.webtest, then run the Webtest from Visual Studio and validate test results. [Additional test run details](https://docs.microsoft.com/en-us/azure/azure-monitor/app/availability-multistep) are recommended by Microsoft. 

Additional Validation rules including Response Time Goal and Maximum Request Times can be configured for the test run.

After validating that the local test runs successfully, you can upload the SignIn.webtest to your test in  Azure Application Insights, and configure the test to be run on a continuous cycle from different geographical regions.  Performance/Availability Reports will be available, along with additional configurations to setup alerts that will be triggered based upon your test thresholds for response times, success rate etc..
