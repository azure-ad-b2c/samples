# Azure AD B2C: Password-less sign-in with phone number 
This sample policy demonstrates how to allow user to sing-up or sign-in without providing an email address. Account verification is based on user phone number. 

## Solution flow
This solution inlcues following scenarios:
1. Sign-up providing the user name (email is not required), user profile, and phone number enrollment. 
1. Sing-in with username and password.  
1. Sing-in with username and phone verification.
1. Sign-in with social account. 
1. Password reset, user provides a username and verifies the account with the phone registered for the account.

## Notes
The solution is not completed. TBD: 
1. Defer the user creation until user verifies the phone number
1. Move the changes to the extension policy
1. Adding support for profile editing policy
1. Adding support to sing-in with phone number only, without providing the username. Note: the phone number is also stored in the signinNames collection. So, it's possible to look up the account by the phone number and also the phone number must be uniuq across the tenant.

## Disclaimer
The sample is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. The sample (Azure AD B2C policy and any companion code) is provided AS IS without warranty of any kind.


