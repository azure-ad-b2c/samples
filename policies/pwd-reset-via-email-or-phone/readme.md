# Password Reset - Via either Email or Phone verification
Demonstrate how to use a displayControl to conditionally process on the users decision to verify their account via Email OTP or SMS.

## How it works
1. Read the users profile once they provide their email address. This will provide their phone number used to Sign Up.
2. Use a displayControl to display the user a radio box selection on whether to verify their account via Email or Phone.
3. The displayControl uses preconditions on the `SendCode` and `VerifyCode` actions to control the `ValidationClaimsExchangeTechnicalProfile` based on the users selection on whether to use phone or email to verify their account. That decision is held in the claim `mfaType`, which acts as the radio box.


## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Notes
This sample policy is based on [SocialAndLocalAccountsWithMFA starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
