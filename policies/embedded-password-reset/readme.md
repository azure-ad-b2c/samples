## Deprecated
This sample is now deprecated, you can find the up to date version [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/add-password-reset-policy?pivots=b2c-custom-policy#self-service-password-reset-recommended).

# Sign-up and Sign-in with embedded password reset

By default when you create a sign-up or sign-in policy (with local accounts), you see a Forgot password? link on the first page of the experience. Clicking this link doesn't automatically trigger a password reset policy. Instead, the error code AADB2C90118 is returned to your app. Your app needs to handle this error code by invoking a specific password reset policy.

This policy demonstrates how to embed the password reset flow a part of the sign-up or sign-in policy. So, Azure AD B2C will not return the AADB2C90118 error message. For more information, see [Set up a password reset flow in Azure Active Directory B2C](https://docs.microsoft.com/azure/active-directory-b2c/add-password-reset-policy).


## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.
