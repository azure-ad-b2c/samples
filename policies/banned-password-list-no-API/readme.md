# A B2C IEF Custom Policy - Sign up and Password reset with banned password list

## Disclaimer
The sample policy is developed and managed by the open-source community in GitHub. This policy is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The policy is provided AS IS without warranty of any kind.

## Scenario
For scenarios where you need to implement a sign up and password reset/change flow where the user cannot use a new password that is part of a banned password list.

## How it works
When the user signs up, the `LocalAccountSignUpWithLogonEmail` technical profile uses the `SignUpWithoutBannedPassword` validation technical profile to validate that the new password is not on the banned list.
The `SignUpWithoutBannedPassword` technical profile uses multiple claims transformations to determine if the new password exists on the banned password list and throws an error if it does.
1. `passwordToMatchBanList` - Uses the [LookupValue Claims transformation](https://docs.microsoft.com/en-us/azure/active-directory-b2c/string-transformations#lookupvalue) to determine if the new password matches a password on the banned list. This will return `bannedPassword=true` if the value matches. Where there is no match, a claim is not output.
2. `CheckBannedPasswordValue` - Compares `bannedPassword` with dummy claim which has been set to `true`. If  `bannedPassword` matches `dummyTrue`, then we have a banned password. The `banned` claim is output as `true`.
If they don't match, then the `banned` claim is output as `false`. This is to counter the fact that the lookup claimsTransformation does not output a claim when there isn't a match.
3. `AssertBannedPasswordFalse` - Compares the value of `banned` claim to `false`. If `banned` claim is false, then no error is thrown, the user did not use a banned password. If `banned` claim is `true`, then it will not match to `false` and therefore the user used a password from the banned password list.

The `LocalAccountSignUpWithLogonEmail` technical profile uses the metadata item `UserMessageIfClaimsTransformationBooleanValueIsNotEqual` to throw an error when the password is on the banned password list. It is referring to when the `AssertBannedPasswordFalse` is not able to assert that the value of `banned` is false, ie that the password was on the banned list.

During Password Reset, a similar flow occurs. When `LocalAccountWritePasswordUsingObjectId` technical profile is called, a validation technical profile called `PasswordReset-CheckpasswordEquivalenceAndBannedList` is invoked. This runs through the same claims transformations as described above for the sign up process.

`AAD-UserWritePasswordUsingObjectId` technical profile is only run based on the [precondition](https://docs.microsoft.com/en-us/azure/active-directory-b2c/validation-technical-profile) that the password was not on the banned password list (`banned=false`).

## Unit Tests
1. New password matches one of the passwords on the banned list (within `passwordToMatchBanList` claims transformation).
2. New password does not match one of the passwords on the banned list (within `passwordToMatchBanList` claims transformation).
