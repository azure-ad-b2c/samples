# A B2C IEF Custom Policy which authenticates to AAD and calls a REST API for more claims

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
This policy utilises passthrough authentication to B2C. THe user wil *NOT* be stored within the B2C directory. Each authentication will call AAD retrived the provided claims and then call a REST API to augment additional claims to send to the target Applications.
This policy is based on the [Azure AD Single tenant implementation](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/identity-provider-azure-ad-single-tenant-custom?tabs=app-reg-ga) as well as the ["Integrate REST API claims"](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/custom-policy-rest-api-intro) documentation.

## Implementation
To implement this use case follow the following steps;
1. Ensure you have followed the ["Get Started with custom policies"](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/custom-policy-get-started) steps within the Microsoft documentation site. 
1. Change the refernces in the [Policy](policy/B2C_1A_SignUpOrSignin_AADRest.xml) from "yourtenant.onmicrosoft.com" to the name of your B2C Tenant.
1. Update the OIDC-Contoso technical profile to reflect your azureAd tenant details as per the [Microsoft dcumentation](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/identity-provider-azure-ad-single-tenant-custom?tabs=app-reg-ga#add-a-claims-provider).
1. Update the REST-GetCRMData technical profile to represent your API as per the [Microsoft docmentation](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/custom-policy-rest-api-intro).
1. [Uplaod](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/custom-policy-get-started#upload-the-policies) and run your policy.


## Notes
This sample policy is based on [SocialAndLocalAccountsWithMFA starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa) However any of the starter pack policies should work for this. All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 