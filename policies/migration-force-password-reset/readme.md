# Dynamically migrate users with forced SSPR

This sample allows dynamically detecting whether a user has been migrated or not. While migrated users will automatically be logged in with valid credentials, required migrated users will be forced to perform a self-service password reset (SSPR). This allows a seemless migration while securely capturing passwords into Azure AD B2C; this scenario is required because you either do not have access to the passwords or you want to force users to reset the passwords during migration. The email provided during login is carried over and prepopulated for required migrated users to provide the best migration experience.

Please note this is controlled by an attribute stored into the Azure AD B2C directory that can be written to called **extension_mustResetPassword**.

## Prerequisites



- You will require to create an Azure AD B2C directory, see the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to setup your AAD B2C environment for Custom Policies [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

- Policy is based on the Display Controls SocialAndLocalAccount starter pack [here](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/Display%20Controls%20Starterpack/SocialAndLocalAccounts)

## Seamless migration flow during Sign In

What happens during signin when the users are migrated to B2C is illustrated in the below diagram:

![A flowchart describing a user authentication flows for this specific sample. The diagram provides a walkthrough visual with squares and words for each step within the boxes. There are arrows that indicates the next step of the user journey.](media/migration-force-password-reset-flow-diagram.png "A flowchart describing a user authentication flows for this specific sample. The diagram provides a walkthrough visual with squares and words for each step within the boxes. There are arrows that indicates the next step of the user journey.")

## Walkthrough UX flow

What happens when your user attempts to login but is forced to perform a self-service password reset (SSPR):
<p align="center">
  <img src="media/migration-force-password-reset-walkthrough.png" alt="drawing" width="75%"/>
</p>

## Test Policy
1. Write value into extension attribute using MS Explorer
   - Add schema to the directory
   - Create test user by running the policy (User ObjectId can be retrieved in [Azure AD Admin Portal](https://portal.azure.com) under Users blade)
   - PATCH user object with mustRestPassword attribute using an API. You can use MS Graph Explorer to do this as well. 
  ```
  POST https://graph.microsoft.com/beta/users/<objectGUID>
    {
        "extension_<appID>_mustResetPassword": true
    }
  ```
- Example
  ```
  PATCH https://graph.microsoft.com/beta/users/a5c9a04a-97aa-41ad-9700-729a579d1e4b
    {
        "extension_32e5499906bd4352b65a5edd7ee3c3a1_mustResetPassword": true
    }
  ```
2. Verify attribute was written in directory by reading user object
  ```
  GET https://graph.microsoft.com/beta/users/{user-id}
  ```
- Example:
  ```
   GET https://graph.microsoft.com/beta/users/a5c9a04a-97aa-41ad-9700-729a579d1e4b
  ```
3. Run Policy and test flow

## Notes

This sample policy is based on [Display Controls SocialAndLocalAccount starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/Display%20Controls%20Starterpack/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections.
