# Azure AD B2C Community samples guidance

1. **Sample folder** - Under the policies root folder, create a sub folder for your sample (in lower case)

1. The sample folder may contain the following sub folders:
    - **source-code**, for the any supporting source code, including APIs, scripts 
    - **policy**, for the Azure AD B2C policy files
    - **media**, if your readme file contains screenshots 


3. Under the sample folder add a `readme.md` file with following sections:
    - **Title**
    - **Description**, describes the user flow and how it's implemented  in the policy
    - **A diagram** with the user flow
    - **Solution artifacts** if your solution has dependencies, such as a Visual Studio solution, describe the components and how to configure and run the VS solution
    - **Tests** how to unit test the policy 
    - **The policy** at the end of the readme file, add a note pointing to the starter pack you used in your sample. For example: 
    
    > Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections.
    
    - **Disclaimer** The sample policy is developed and managed by the open-source community in GitHub. The policy is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The app is provided AS IS without warranty of any kind. 
    

4. In the policy root folder, update the [readme.md](readme.md) file with the new sample you created. 

## Policy
- Make all the changes in the **relying party policy**. Avoid making changes to the base and extension policy.
- If you have multiple relying party policies, create your own extension policy that inherits from the original extension file, for example:
    - B2C_1A_TrustFrameworkBase
    - B2C_1A_TrustFrameworkExtensions
    - B2C_1A_FIDO_Extension
        - B2C_1A_FIDO_sign_in
        - B2C_1A_FIDO_enroll 
- The relying party name should start with a prefix, such as **FIDO**SignUpOrSignIn
- If you don't change the base or extension policy, don't include them. Instead, add reference to the starter pack, as mentioned above.
- Add notes that describe the purpose of the component, starts with `<!--Sample: -->`
- Add action required comments where a developer needs to take an action, starting with `<!--Sample action required: -->`
- Remove you tenant name (use `yourtenant` instead), and application IDs


