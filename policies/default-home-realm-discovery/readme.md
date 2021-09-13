# A B2C IEF Custom Policy - A Sign In policy with Home Realm Discovery and a Default Identity Provider

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
For scenarios where you need to implement a sign in journey, where the user is automatically directed to their federated identity provider based off of their email domain. And for users who arrive with an unknown domain, they are redirected to a default identity provider.

In this example, users who enter an email with the suffix `contoso.com`, they will be redirected directly to their federated identity provider to sign in. In this case that is Azure AD (SAML2).

Users who enter an email with the suffix `facebook.com`, they will be redirected directly to their federated identity provider to sign in. In this case that is Facebook (OAuth).

Where a user comes from an unknown email suffix,  they will be redirected directly to a default identity provider, in this case that is Azure AD (OpenId).

## Prerequisites
- You can automate the pre requisites by visiting this [site](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to create an Azure AD B2C directory, see the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to setup your AAD B2C environment for Custom Policies [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

## How it works
The web application must take the users email address and extract the domain name. It **must** make the authentication request to Azure AD B2C with the `domain_hint` parameter containing the email domain of the user.

The first step of the journey, `ParseDomainHint`, captures the `domain_hint` value and copies it into the `domainParameter` claim.
Using the `DomainLookup` output claims transform, the `domainParameter` is looked up against a list of known domain names. If the domain name matches, `DomainLookup` will output a claim `knownDomain` = `True`, or otherwise `null`.

To return a final `True` or `False` value, `CheckDomainParameterValue` output claims transform is called, which compares the `knownDomain` with `dummyTrue` (which holds a True value inside it). Finally we obtain a claim `isKnownCustomer` which is either `True` or `False`. This prevents having a null value in thr case of `knownDomain`.

The final output claims transform `CreateidentityProvidersCollection`, will add the value of `domainParameter` into a string collection for use later - it will enable a specific identity provider only for the step where multipel known identity providers are configured to give home realm discovery (direct redirection).

Step 2/3 will initiliase all the known identity providers (for known domains). However, the SAML IdP (known domain IdP) has the following metadata items:
```xml
    <Item Key="ClaimTypeOnWhichToEnable">identityProviders</Item>
    <Item Key="ClaimValueOnWhichToEnable">contoso.com</Item>
```
This means, after the `ParseDomainHint` technical profile has run, we will fall into these cases:
1. `isKnownCustomer = True`, then this step executes
2. Based off of the `identityProviders` claim a single IdP will be enabled
3. User is redirected to the single available IdP
4. `isKnownCustomer = False`, then the entire step is skipped

If a `domain_hint` was used that was not recognised, and `isKnownCustomer = False`, then step 4/5 will execute, which has a single Identity Provider configured against it. The user is redirected directly to that identity provider.

## Unit Tests
1. Setup 3 identity providers, initialise two in Step 2/3 and initilise one in step 4/5
2. Test a `domain_hint` that does not match any of the known IdPs. The user should be directed to the default IdP.
3. Test a `domain_hint` that does match a known IdP, the user should be directed to the specific IdP without interaction.

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 