# A B2C IEF Custom Policy which allows Password Reset via Phone Number (OTP) after entering your Username

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
For scenarios where the login Id is a username and then password reset requires a SMS Verification code.

## Screen Shots
**Enter Username**

![Enter Username](media/Screen1.png)

**Verify SMS - SendCode**

![Verify SMS - SendCode](media/Screen2_a.png)

**Verify SMS - Enter Code**

![Verify SMS - Enter Code](media/Screen2_b.png)

**New Password**

![New Password](media/Screen3.png)

## Notes
> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.