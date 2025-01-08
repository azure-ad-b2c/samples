# A B2C IEF Custom Policy - Disable and lockout an account after a time period

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
For scenarios where you need to prevent users logging into the application after a set number of days. The account will also be disabled at the time of the users login attempt in the case the user logs in after the time period.

This sample presents the user with a dummy screen where a friendly error can be presented when a user tries to login after their account is deemed inactive.
Administrators would need to update the users last logon time extension attribute and set the account to enabled in Azure AD B2C before the user can login again. This is demonstrated in the unlock_account.ps1 script.

The policy gets the current time using a claims transformation (`GetSystemDateTime`) as part of the `AAD-UserReadUsingObjectId` technical profile.

In the case of a new sign up, we use the presence of the `newUser` claim to write the current time into the `extension_lastLogonTime` attribute via the `AAD-UserWriteLogonTimeUsingObjectId` technical profile.

For sign in's, the last logon time is read via the `AAD-UserReadLastLogonTime` technical profile.
As part of this step, a claims transformation is run called `CompareTimetoLastLogonTime`, which checks the difference between the last logon time and the set time period in the `CompareTimetoLastLogonTime` claims transformation. 

This time period is defined in the input parameter called `timeSpanInSeconds`, and is measured in seconds. This policy is set to 160 seconds by default. This will output `True` if the time is greater than `timeSpanInSeconds` compared to the last logon time, and `False` otherwise. The value is stored in `userLockedOutForInactivity`.

Next, the `AAD-DisableAccount` technical profile is executed based off the precondition that `userLockedOutForInactivity` returned `True`. This will mark the account as disabled. 
The user will be presented with a dummy page rendered by the `Self-Asserted-AccountDisabled` technical profile. A custom error page can be shown here.

For subsequent logons whilst the account is disabled, the user will be get treated by the native Azure AD B2C functionality for disabled accounts. This error message can also be customised.

### Note: 
This sample requires custom attributes to be enabled in the policy by providing `ApplicationObjectId` in the `AAD-Common` technical profile metadata as mentioned [here](https://learn.microsoft.com/en-us/azure/active-directory-b2c/user-flow-custom-attributes?pivots=b2c-custom-policy#modify-your-custom-policy). 

## Unit Tests
1. Sign up with an account, check the value of exentsion_lastLogonTime using the unlock-account.ps1 script.
2. Sign in after sign up to make sure the account can login successfully.
3. After a few minutes, sign in again and verify the account is locked out.
4. Sign in again and you will see the native account disabled error from Azure AD B2C
5. Use the script to unlock the account and repeat these steps.
