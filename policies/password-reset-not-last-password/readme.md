# A B2C IEF Custom Policy - Password reset without being able to use the last password

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
For scenarios where you need to implement a password reset/change flow where the user cannot use their currently set password.

This sample presents the user with a screen to enter their current password, new password and re-enter password field.
The policy uses the `LocalAccountWritePasswordUsingObjectId` Technical Profile to call a set of Validation Technical Profiles to ensure the current password is correct, and that the new password is not the same as the current password.
First it calls `login-NonInteractive-PasswordChange` to validate the current password is correct.
Then it calls `ComparePasswords` which executes two Claims Transformations.
1. `CheckPasswordEquivalence` - This claims transformation does a string comparison operation against the new password and the current password. If they match, it outputs `True`, otherwise `False`.
2. `AssertSamePasswordIsFalse` - This claims transformation asserts that the boolean from step 1 came back as `False`, indicating the password was different.

Finally the password is written using `AAD-UserWritePasswordUsingObjectId` as normal.

## Unit Tests
1. Enter valid `current password`, and the same password for `new password` and `re-enter password`.
2. Enter invalid `current password`.
3. Enter valid `current password`, and a different password for `new password` and `re-enter password`.
4. Repeat after successful password change to endure new password was written successfully.