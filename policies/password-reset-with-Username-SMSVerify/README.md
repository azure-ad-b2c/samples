# A B2C IEF Custom Policy which allows Password Reset via Phone Number (OTP) after entering your Username

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
For scenarios where the login Id is a username and then password reset requires a SMS Verification code.

## Screen Shots
1. **Enter Username**

![A prompt for user to input their Username only.](media/Screen1.png)

1. **Verify SMS - SendCode**

![A user is provided with two options. Send Code or Call Me.](media/Screen2_a.PNG)

1. **Verify SMS - Enter Code**

![A prompt is provided to input SMS code while providing an empty box is provided.](media/Screen2_b.PNG)

3. **New Password**

![Two fields are provided to input a New Password in twice.](media/Screen3.PNG)

## Notes
> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.
