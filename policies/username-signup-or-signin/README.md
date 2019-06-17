# A B2C IEF Custom Policy which uses Usernames as the sign in identifier

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
For scenarios where you would like users to sign up and sign in with Usernames rather than Emails.

At Sign Up, the user is asked to validate an email address. This email address will be associated to the user account by writing to a protected attribute: `strongAuthenticationEmailAddress`. This attribute can only be read or written to by the B2C policy execution.

At Sign In, the Username provided is used as a lookup against all SignInNames that are present on user objects stored in the the directory. This is achieved by sending the paramater `nca=1` when making the authentication request via the `login-NonInteractive` technical profile in `TrustFrameworkBase`. 

At Password Reset, the user will be asked to confirm the email address. It will be sent a TOTP and validated against the email address stored at sign up in the `strongAuthenticationEmailAddress` attribute. This ensures the user owns this Username.