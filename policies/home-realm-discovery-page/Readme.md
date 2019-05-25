# Azure AD B2C: Home Realm Discovery page
This sample custom policy demonstrates how to create home realm discovery page. On the sign-in page user provides the sign-in email address and clicks continue. B2C checks the domain portion of the sign-in email address. If the domain name is `contoso.com` the user is redirected to Contoso.com Azure AD to complete the sign-in. Otherwise the user continues the sign-in with user name and password. In both cases (AAD B2C local account and AAD account), the user dons't need to retype the user name. 

 User flow:
1. On the sign-in page user provides the sign-in email address and clicks continue. 
1. B2C extracts the domain portion of the sign-in email address (also change to lower case). 
1. Based on the domain name, user continues with:
    1. Redirect to an enterprise Azure AD tenant to complete the sign-in.
    1. Sign-in as a local account (user name and password)

## Disclaimer
The sample is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The sample (Azure AD B2C policy and any companion code) is provided AS IS without warranty of any kind.

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.
