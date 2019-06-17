# A B2C IEF Custom Policy which forces TOTP validation on Sign In

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
For scenarios where you would like users to validate their email via TOTP on all sign ins.

At Sign In, the email address authenticated with is copied into a read only attribute `readOnlyEmail` via an `outputClaimTransformation` in the `SelfAsserted-LocalAccountSignin-Email` technical profile.

The `readOnlyEmail` claim is passed as an input claim to the `EmailVerifyOnSignIn` self asserted technical profile to validate the email address via TOTP. This is made possible by using `PartnerClaimType="Verified.Email"` in the output claims section.

The user journey only calls the `EmailVerifyOnSignIn` self asserted technical profile if the user is not a new user. This bypassess this particular step if the user is signing up.

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 