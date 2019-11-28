# Include an Azure AD access token thourgh a B2C token, as part of a B2C Sign In
> Disclaimer: This sample is provided AS IS - a best effort will be made to update this sample as the service evolves.

This sample builds on the built-in user flows, and how to include an Azure AD bearer token as a claim in a B2C token issued from a custom B2C sign in policy.  It also shows how to call the Graph API of the users’ home Azure AD tenant using the issued Azure AD token.  For reference, similar capability can be achieved to receive the original identity provider’s id token, using the built-in B2C user flows
https://docs.microsoft.com/en-us/azure/active-directory-b2c/idp-pass-through-user-flow
The following diagram overviews this sample:.

![AAD Token](media/issueAADTokenThroughB2C.jpg)

These instructions will guide you to:

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].

If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).

To provide product feedback, visit the [Azure Active Directory B2C Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).
