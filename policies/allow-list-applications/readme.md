# A B2C IEF Custom Policy which allows specific apps to call the b2c policy (allow list of accepted clientIDs)
## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
This policy checks to see if the client id in the OIDC request is on an "allowed list" of applications ID's.  If yes, the flow allows users to attemp sign-in or sign-up, else, a block page will be returned with a customizable error message. 

Key components of this B2C custom policy:
1. User journey steps 1 and 2 checks if the calling application client id is allowed, and will block sign-in sign-up if not allowed.
2. A block page that simply shows the "you cannot access this application" message to the user - the message can be customized.
3. Use of a technical profile "checkIfAppIsAllowed" that collects the incoming client Id (using a claims resolver {OIDC:ClientId}), and calls a claims transformation type LookUpValue, and returns true or false if the client Id is on the allow list.


## Implementation
To implement this use case follow the following steps;
1. Ensure you have followed the ["Get Started with custom policies"](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/custom-policy-get-started) steps within the Microsoft documentation site. 
1. Change the references in the [Policy](policy/B2C_1A_SignUpOrSignin_checkAppId.xml) from "yourtenant.onmicrosoft.com" to the name of your B2C Tenant.
1. Update the ClaimsTransformation with Id="isAppAllowed" to reflect your list of allowed client id's. For more information about B2C claims transformations [Microsoft Azure AD B2C Claims Transformation documentation](https://docs.microsoft.com/en-us/azure/active-directory-b2c/claimstransformations).
1. [Upload](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/custom-policy-get-started#upload-the-policies) and run your policy.


## Notes
This sample policy is based on [SocialAndLocalAccountsWithMFA starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa) However any of the starter pack policies should work for this. All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
