# A B2C IEF Custom Policy - Sign in with MFA method choice (Phone/Email)

## Disclaimer
The sample policy is developed and managed by the open-source community in GitHub. This policy is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The policy is provided AS IS without warranty of any kind.

## Scenario
For scenarios where you would like to give users the choice to use Email verification or SMS/Phone Call as the second authentication factor, and allow them to change this choice at a later point via Profile Edit.

## How it works
When the user signs up, where the user attribute for `extension_mfaByPhoneOrEmail` does not exist, the user is prompted to make a selection via a radio box. 
When a user has made a selection for the MFA method, then the `PersistMFAMethod` technical profile is executed, which writes the selection value to the `extension_mfaByPhoneOrEmail` attribute.

On Sign In, the value for `extension_mfaByPhoneOrEmail` is read by augmenting the `AAD-UserReadUsingObjectId` technical profile. Also, on Sign In, the email is captured and stored into a read only attribute for use later when the Email Verification screen is presented in the case where the MFA method for the user was set to email. This is achieved in the the `SelfAsserted-LocalAccountSignin-Email` technical profile with the Claims Transformation `CopySignInNameToReadOnly`.

If the MFA method was returned as `phone`, then the preconditions as part of the UserJourney will execute the `PhoneFactor-InputOrVerify` technical profile which will ask the user to verify their phone number via SMS or Phone call.

If the MFA method was returned as `email`, then the preconditions as part of the UserJourney will execute the `EmailVerifyOnSignIn` technical profile which will ask the user to verify their email using a One Time Pass Code.

The user can execute the Profile Edit journey to modify their MFA Method later.
When the user tries to reset their password and the MFA method for the user is set to `phone`, then they will be prompted to verify their phone number via SMS or Phone call. If the MFA method is set to `email`, they will not do any extra verification, since the first step already verifies their email address.

## Unit Tests
1. Sign Up and verify the MFA Method is selectable. 
2. Sign In and verify the expected MFA Method is prompted for.
3. Run the Profile Edit policy and change the MFA Method, repeat the Sign In for both methods to make sure the choice is respected.
4. Run the password reset journey and confirm the MFA method is respected.