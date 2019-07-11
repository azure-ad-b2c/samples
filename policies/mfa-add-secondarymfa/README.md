# AAD B2C IEF/Custom policy to allow users to store and select two phone numbers at SignIn or SignUp

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
Store two strong phone numbers in AAD B2C securely
 1. The user has forgotten or lost one of their phone numbers and still need access
 2. Select between any of the two phone numbers at the time of signIn
 
## Flow
### New User / SignUp
    1. Collect signUp attributes and first MFA phone number. 
    2. Prompt the user if they want to store an additional phone number for MFA
        2.1 If user selected "Yes" - Ask for another phone number
            2.1.1 Validate the other phone number via text or phone call
            2.1.2 Store the secodary phone number
            2.1.3 Issue token
        2.2 If user selected "No" 
            2.2.1 Issue token
### Existing user SignIn/Login

3. User enters username and password. 

    3.1 User does not have two phone numbers on file
        3.1.1 Gets prompted for first MFA and completes MFA
        3.1.2 Gets prompted to store another MFA phone number and follows step from 2. from previous 

    3.2 User has two phone numbers on file
        3.2.1 User gets prompted for MFA with an option to select between any two phone numers
        






## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
