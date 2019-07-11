# A B2C IEF Custom Policy which invokes MFA based on the IP and absolute time window in which the user last did MFA

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
MFA IP Timeout - A policy which forces the user to do MFA on 3 conditions:
 1. The user has newly signed up.
 2. The user has not done MFA in the last X seconds.
 3. The user is logging in from a different IP than they last logged in from.

If the users IP address is different to the last logon IP, then the user will do MFA.

If the users IP address is the same as the last logon IP at which they did MFA, then the user will not do MFA unless the user did MFA over 100 seconds ago (default in this example).

This means the users MFA session is absolute, and if the user contiues to logon from the same IP, the user will be prompted to perform MFA after this time period expires.

The absolute time window for which the user must perform MFA after, can be adjusted in seconds by modifying the `timeSpanInSeconds` paramter in the `CompareTimetoLastMFATime` technical profile in the `TrustFrameworkBase` file. It has been set to 100 seconds in this example.

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 