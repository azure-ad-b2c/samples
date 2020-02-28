# Sign In and Sign Up with Username or Email
This sample combines the UX of both the Email and Username based journeys.

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](../../../SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections.

## How it works
### Sign Up
During sign up, the user is presented with a page to enter their Username. Upon submitting this field, the `regexAnalysisUsername` validation technical profile will call the claims transformation `isEmail`.

`isEmail` claims transformation uses a regex to return a boolean `isEmailBoolean` if it detects an email format.

`isEmailBoolean` then is used to determine if the user will go through the Username based sign up or Email sign up.
During Email based sign up, the user only needs to verify the email and provide any further details.
During Username based sign up, the user will be created with the username as the identifier, and the verified email stored in the `strongAuthenticationEmail` field.

### Sign In
During sign in, the user is presented with a page to enter their Username and Password as normal. B2C will lookup the account with either and authenticate the user as normal.

### Password Reset
During password reset the user is presented with a page to enter their Username. Upon submitting this field, the `regexAnalysisUsername` validation technical profile will call the claims transformation `isEmail`.

`isEmail` claims transformation uses a regex to return a boolean `isEmailBoolean` if it detects an email format.

`isEmailBoolean` then is used to determine if the user will go through the Username based password reset or Email password reset.
During Email based password reset, the user only verifies the email and after can change the password.
During Username based password reset, the user will verify the email, and if the email matches that which was stored in `strongAuthenticationEmail` during sign up, then the user can reset the password.

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].

If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).

To provide product feedback, visit the [Azure Active Directory B2C Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).