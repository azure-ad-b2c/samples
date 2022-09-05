# A/B testing

Demonstrates how to create an A/B testing policy to check a new user experience. For example, if you introduce new features into the user journey, such as the ability to sign-in with Google or MFA, you can test the policy for 50% of your users.  

The first step of the _SignUpOrSignIn_WithAbTesting_ invokes _Get-RandomNumber_ technical profile that returns _randomNumber_ claim with 0 or 1 values. Based on the value returned the user journey calls the corresponding sub user journey.

## Live demo

To test the policy, run the [B2C_1A_signup_signin_WithAbTesting](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_signup_signin_WithAbTesting/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login). Then, refresh the page several times. You should see that the sign-in with Google button appears and disappears (based on the random value). 

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Notes

This sample policy is based on [SocialAndLocalAccountsWithMFA starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
