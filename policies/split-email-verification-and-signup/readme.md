# Separate the Email Verification and User Registration into 2 separate screens

The AD B2C Signup requires users to verify their email first before they can create their accounts. The default Signup page on AD B2C clubs together the email verification controls, followed by user's First Name, Last Name and other fields. This may not be always desirable since the users cannot interact with these fields without verifying their email anyway. Ideal user experience would be to split the registration process into two separate screens, where the users only see the email verification related controls first, and not the entire set of Registration controls. When they complete the email verification, they should be taken to the next screen which asks for more user attributes (firstname, lastname, dob etc.).

## How it works

This policy splits the Signup process into 2 steps:
 * First step performs Email Verification only, skipping all other default fields related to users registration
 * Second step (if email verification was successful) takes the users to a new screen where they can actually create their accounts
 
![Flow](media/flow.png)

## Notes
This sample policy is based on [LocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/LocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 