# Azure AD B2C: Force password after 90 days

This solution demonstrates how to force user to reset password after 90 day or so. The solution is based on an extension attribute that stores the last time user sets the password and a comparison to the current date and time, minus specified number of days. Read here how to [configure extension attributes](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom). When a user sign-up or resets the password, the policy sets the extension attributes to the current datetime. On sign-in the policy checks whether both the extension attribute is not null and whether the current date time, minus 90 day is greater than last time user sets the password or not. If greater, it means that at least 90 days passed from the time user reset the password.

## Adding this functionality to your policy
To merge the policy with yours, you need:
1.	Add the claims in the extension policy

1.	Add the claims transformations in the extension policy

1. In the **ComparePasswordResetOnWithCurrentDateTime** claims transformation, change the value of the **timeSpanInSeconds** input parameter to the number of days in seconds you want the users to reset their password. For testing only the value is set to 80 seconds. Note: the value must be negative (starts with minus)

1. In the **AAD-UserReadUsingObjectId** technical profile, add the output claim and output claims transformation.  This technical profile reads the extension_passwordResetOn attribute from the user account, checks if the claim is null and compares the value of the extension_passwordResetOn claim with current datetime. The result of this technical profile is the **skipPasswordReset** output claim (return in the last output claims transformation) that indicates whether password reset is required or not (based on date comparison and if extension_passwordResetOn is null) 

1. In **AAD-UserWriteUsingLogonEmail** and **AAD-UserWritePasswordUsingObjectId** add the input claims transformation and perssis claims. These technical profiles, set the current datetime to the extension_passwordResetOn claim and persists the data do the user account.

1.	Add the extra orchestration before the last orchestration step. This orchestration step asks the user to reset the password, saves the new password, and also sets the extension_passwordResetOn claim to current date and time. The orchestration setup will NOT run for social account and if the skipPasswordReset claim is true. 

## Tests
You should run at least following acceptance tests:
- **New account** doesn't need to reset the password (skipPasswordReset: true)
- **Existing account before 90 days** doesn't need to reset the password (skipPasswordReset: true)
- **Existing account after 90 days** need to reset the password (skipPasswordReset: false)
- **Account that extension_passwordResetOn never set before**, should reset the password (skipPasswordReset: false) = pass


## Disclaimer
The sample is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The sample (Azure AD B2C policy and any companion code) is provided AS IS without warranty of any kind.

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.
