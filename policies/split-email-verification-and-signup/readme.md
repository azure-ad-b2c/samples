# Separate the Email Verification and User Registration into 2 separate screens

The AD B2C Signup requires users to verify their email first before they can create their accounts. The default Signup page on AD B2C clubs together the email verification controls, followed by user's First Name, Last Name and other fields. This may not be always desirable since the users cannot interact with these fields without verifying their email anyway. Ideal user experience would be to split the registration process into two separate screens, where the users only see the email verification related controls first, and not the entire set of Registration controls. When they complete the email verification, they should be taken to the next screen which asks for more user attributes (firstname, lastname, dob etc.).

## Prerequisites
- You can automate the pre requisites by visiting this [site](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to create an Azure AD B2C directory, see the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to setup your AAD B2C environment for Custom Policies [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

## How it works

This policy splits the Signup process into 2 steps:
 * First step performs Email Verification only, skipping all other default fields related to users registration
 * Second step (if email verification was successful) takes the users to a new screen where they can actually create their accounts
 
![Flow](media/flow.png)

## Notes
This sample policy is based on [LocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/LocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 