# A B2C IEF Custom Policy which forces TOTP validation on Sign In

## Disclaimer
The sample policy is developed and managed by the open-source community in GitHub. This policy is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The policy is provided AS IS without warranty of any kind.

## Scenario
For scenarios where you would like users to validate their email via TOTP on all sign ins.

At Sign In, the email address authenticated with is copied into a read only attribute `readOnlyEmail` via an `outputClaimTransformation` in the `SelfAsserted-LocalAccountSignin-Email` technical profile.

The `readOnlyEmail` claim is passed as an input claim to the `EmailVerifyOnSignIn` self asserted technical profile to validate the email address via TOTP. This is made possible by using `PartnerClaimType="Verified.Email"` in the output claims section.

The user journey only calls the `EmailVerifyOnSignIn` self asserted technical profile if the user is not a new user. This bypassess this particular step if the user is signing up.
