# A B2C IEF Custom Policy - A Sign In policy with Home Realm Discovery

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
For scenarios where you need to implement a sign in journey, where the user is automatically directed to their federated identity provider based off of their email domain entered at the B2C sign in page. And for users who enter an unknown domain, they are redirected to the default local account sign in page.

In this example, users who enter an email with the suffix `jsuri.com`, will be redirected directly to the Azure AD identity provider configured.

Users who enter an email with the suffix `outlook.com`, they will be redirected directly to the Microsoft Account (LiveId) login page.

Where a user enters an unknown email suffix, they will be redirected directly to the default local account sign in page.

## How it works
The first step of the user journey presents a page to collect the users email address. The page uses input validation such that a valid email is provided.

The next step of the journey runs multiple output claims transforms to check if the domain is known.
First an output claims transformation called `ParseDomain` extracts the email domain name for the provided email address and copies it into the `domainParameter` claim.
Using the `DomainLookup` output claims transform, the `domainParameter` is looked up against a list of known domain names. If the domain name matches, `DomainLookup` will output a claim `knownDomain` = `True`, or otherwise it will be `null`.

To return a final `True` or `False` value for whether the domain was known, the `CheckDomainParameterValue` output claims transform is called, which compares the `knownDomain` with `dummyTrue` (which holds a True value inside it). Finally we obtain a claim `isKnownCustomer` which is either `True` or `False`. This prevents having a null value in the case of `knownDomain`.

The final output claims transform `CreateidentityProvidersCollection`, will add the value of `domainParameter` into a string collection for use later - it will enable a specific identity provider only for the step where multiple known identity providers are configured to give home realm discovery (direct redirection).

Steps 3 and 4 handle the scenario where the domain was unknown, and mimic the steps to sign in via Local Account.

Steps 5 and 6 will initialize all the known identity providers (for known domains). However, the Azure AD and Microsoft Account IdP (known domain IdP's) have the following metadata items:
```xml
    <Item Key="ClaimTypeOnWhichToEnable">identityProviders</Item>
    <Item Key="ClaimValueOnWhichToEnable">outlook.com</Item>
```
This means, after the `ParseDomainHint` technical profile has run, we will fall into these cases:
1. `isKnownCustomer = True`, then this step executes
2. Based off of the `identityProviders` claim a single IdP will be enabled
3. User is redirected to the single available IdP
4. `isKnownCustomer = False`, then the entire step is skipped

## Unit Tests
1. Setup at least 1 external identity provider, and initialize it in step 5 and 6.
2. Test an email with a known domain. The user should be directed to the respective IdP.
3. Test an email with a unknown domain. The user should be directed to the Local account logon page.

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 