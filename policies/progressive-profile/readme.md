# A B2C IEF Custom Policy - Progressive profile update

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
User will not be prompted for 'Loyalty number' during registration. But will be progressively prompted for it during sign-in.

## How it works

When the user signs in if 'extension_LoyaltyNumber' doesn't exist and if its not a 'newUser' they will be directed to 'SelfAsserted-ProfileUpdate' to update the Loyalty number.

## Unit Tests
1. During registration - Loyalty number will not be prompted.
2. During sign-in - you will be forced to enter a loyalty number. 

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections.
