# A B2C IEF Custom Policy which uses Usernames as the sign in identifier

## Disclaimer
The sample policy is developed and managed by the open-source community in GitHub. This policy is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The policy is provided AS IS without warranty of any kind.

## Scenario
For scenarios where you would like users to sign up and sign in with Usernames rather than Emails.

At Sign Up, the user is asked to validate an email address. This email address will be associated to the user account by writing to a protected attribute: `strongAuthenticationEmailAddress`. This attribute can only be read or written to by the B2C policy execution.

At Sign In, the Username provided is used as a lookup against all SignInNames that are present on user objects stored in the the directory. This is achieved by sending the paramater `nca=1` when making the authentication request via the `login-NonInteractive` technical profile in `TrustFrameworkBase`. 

At Password Reset, the user will be asked to confirm the email address. It will be sent a TOTP and validated against the email address stored at sign up in the `strongAuthenticationEmailAddress` attribute. This ensures the user owns this Username.