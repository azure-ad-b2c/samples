# Azure AD B2C Community samples guidance

1. **Sample folder** - Under the policies folder, create a sub folder for your sample (all in lower cases)

1. The sample folder may contain following sub folders:
    - **source-code**, for the source code 
    - **policy**, for the policy files.
    - **media**, if your readme file contains screenshots 


3. Under the sample folder add a `readme.md` file with following sections:
    - Title and description
    - **Adding this functionality to your policy**, describe the necessary steps how to add the functionality to an existing policy
    - **Solution artifacts** if your solution has dependencies, such as a Visual Studio solution, describe the components and how to configure and run the VS solution
    - **Tests** how to test the policy 
    - **The policy** at the end of the readme file, add a note pointing to the starter pack you use in your sample. For example: 
    
    > Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections.
    
    - **Disclaimer** The sample policy is developed and managed by the open-source community in GitHub. The policy is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The app is provided AS IS without warranty of any kind. 
    

4. Go to the root folder and update the readme.md file with the new sample you created. 

## Policy
- Avoid making changes to the base policy 
- If you didn't change the base policy, don't upload the base policy. Instead, add reference to the starter pack, as mentioned above.
- Remove you tenant name (use `yourtenant` instead), and application IDs
- Add comments to describe the logic behind the XML elements. The comments should start with **Sample:**
- Add action required comments where a developer needs to take an action, starting with **Sample action required:**

