# Include an Azure AD access token thourgh a B2C token, as part of a B2C Sign In
> Disclaimer: This sample is provided AS IS - a best effort will be made to update this sample as the service evolves.

This sample builds on the built-in user flows, and shows how to include an Azure AD bearer token as a claim within a B2C token issued from a custom B2C sign in policy.  It also shows how to call the Graph API of the users’ home Azure AD tenant using the issued Azure AD token.  For reference, similar capability can be achieved to receive the original identity provider’s id token, using the built-in B2C user flows
https://docs.microsoft.com/en-us/azure/active-directory-b2c/idp-pass-through-user-flow
The following diagram overviews this sample:.

![AAD Token](media/IssueAADTokenThroughB2C.jpg)

To configure the solution above, you will need to use Azure AD B2C Custom policies that use the Azure AD B2C Identity Experience Framework.  Review the getting started with custom B2C applications
https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom?tabs=applications
In the documentation, it references the B2C starter pack of advanced policies – download the starter pack, and use the policy files from the SocialAndLocalAccounts folder– which the use of both local and Azure AD accounts.   

We also recommend using Visual Studio Code and the B2C extensions for managing Custom policies, as there are many tools to simplify the configuration of the policies.

•	Download Visual Studio Code:  https://code.visualstudio.com/

•	Install Visual Studio Code Extension: Azure AD B2C  https://marketplace.visualstudio.com/items?itemName=AzureADB2CTools.aadb2c

Register an application that allows users from Azure AD tenants to sign-in to your B2C tenant

From the Azure AD B2C portal, select App registration, create a friendly name for the app, and select the “Accounts in any organization directory or any identity provider…” under the Supported Account Types.   This selection makes this a Multi-tenant application, that becomes available to any Azure AD tenant.  Details about multi-tenant Azure AD applications can be found here:  https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-convert-app-to-be-multi-tenant

The reply url (redirect URI) should be your b2c tenant instance’s reply uRL  (replace “your-B2C-tenant-name” with your tenant’s name.).  
https://your-B2C-tenant-name.b2clogin.com/your-B2C-tenant-name.onmicrosoft.com/oauth2/authresp


![RegisterYourApp](media/RegisterYourApp.jpg) 

Select Register.  After the application is successfully created, copy down the Application (client) id – which will be used in the B2C policy file (see below)

![clientID](media/ClientID.jpg)

![clientID2](media/ClientID2.jpg)



## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].

If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).

To provide product feedback, visit the [Azure Active Directory B2C Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).
