# Azure AD B2C: Account lockout
Demonstrate how to lockout account after six unsuccessful sign-in.

## Run the solution
To run the visual studio solution, you need to deploy this web app to Azure App Services. For more information, see [Create and publish the web app](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-get-started-dotnet#create-and-publish-the-web-app). In production environment make sure to secure the communication between Azure AD B2C to your Rest API. For more information, see: [Secure your RESTful service by using client certificates](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-custom-rest-api-netfw-secure-cert) OR [Secure your RESTful services by using HTTP basic authentication](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-custom-rest-api-netfw-secure-basic)

## Solution artifacts
### Azure AD custom policy

This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). 
   * All changes are marked with **Demo:** comment inside the policy XML files.
   * Make the necessary changes in the **Action required** comments

### Visual studio solution
- **IdentityController** The custom policy calls this REST API endpoint
- **Models** folder - this folder contains the necessary object-mapping classes 
- **Models/Consts.cs** - Contains the app settings:
    - LOCKOUT_AFTER - Specifies the amount of unsuccess sign-in to lockup the account e.g 6 (times)
    - UNLOCK_AFTER - Specifies the amount of minutes to unlock the account e.g. 5 (minutes)


## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

