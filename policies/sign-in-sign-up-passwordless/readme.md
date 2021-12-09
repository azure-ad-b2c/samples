# A B2C IEF Custom Policy - Sign Up and Sign In passwordless flow

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
In this scenario, the user can use a passwordless sign up and sign in flow. The passwordless component is delivered via a SMS OTP on the user's phone.

Because the sign up is passwordless, the user is not required to enter and verify a password. B2C requires a password to create a user so the password is generated in the custom policy. To make it random across users, a GUID is included as part of the password. The user is not made aware of their password.

To stop users randomly typing in email addresses when signing in and then proofing up, the user can only sign in if that email address has a `strongAuthenticationPhoneNumber` attribute associated with it i.e. the user has already proofed up. 

## Prerequisites
- You can automate the prerequisites by visiting this [site](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to create an Azure AD B2C directory, see the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to setup your AAD B2C environment for Custom Policies [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

- For any custom policy sample which makes use of Extension attributes, follow the guidance [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#create-a-new-application-to-store-the-extension-properties) and [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-create-custom-attributes-profile-edit-custom#modify-your-custom-policy-to-add-the-applicationobjectid). The `AAD-Common` Technical profile will always need to be modified to use your `ApplicationId` and `ObjectId`.

## How it works

When the user signs up, the `LocalAccountSignUpWithLogonEmail-Passwordless` technical profile asks the user to enter an email address and verify it.

The user then enters the display name, given name and surname.

The `AAD-UserWriteUsingLogonEmail` validation technical profile first calls the InputClaimsTransformation `CreatePasswordGUID` that creates a random GUID and then calls the InputClaimsTransformation `CreatePassword` that adds this to a long string to create a random password. This information is then written to B2C.

The `PhoneFactor-InputOrVerify` technical profile then asks the user to proof up and validate the phone number via a SMS OTP.

The `AAD-UserWritePhoneNumberUsingObjectId` technical profile then saves the phone number.

The JWT is then sent to the user.

When the user signs in, the `SelfAsserted-LocalAccountEmailSignin` technical profile asks them to enter an email address and then the `AAD-UserReadUsingEmailAddress` technical profile is called to check that the user exists.

The account has to have a `strongAuthenticationPhoneNumber` attribute associated with it otherwise an error is displayed.

If the user exists, the `PhoneFactor-InputOrVerify` technical profile is used to verify the user by means of a SMS OTP sent to the phone number they proofed up with.

The JWT is then sent to the user.

## Notes

This sample policy is based on [SocialAndLocalAccountsWithMfa starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa).

The reference to `B2C_1A_TrustFrameworkExtensions_Samples_MFA` file in the sample is to the `TrustFrameworkExtensions` file in the sample pack.

If you want the option of a phone call as well as a SMS OTP, comment out the `setting.authenticationMode` in this sample.

## Unit Tests
1. Sign up, verify email, and verify that you are asked to proof up.
2. Sign in with an invalid email. This should cause an error.
3. Sign in with a valid email. Check that you are used to verify via a SMS OTP.
4. Create an user via the portal. Then try and sign in with that user. Because that user has not yet proofed up, an error will be thrown.

