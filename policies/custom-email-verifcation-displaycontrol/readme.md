# Custom email verification - DisplayControls

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

### Scenario

This set of policies demonstrates how to use a Custom email verification solution, which allows you to send your own custom email verification during sign-up or during the password reset user journeys.

This sample is detailed on the Microsoft B2C documentation site:

 - [Mailjet](policy/Mailjet) - Custom email verification with Mailjet, [document](https://docs.microsoft.com/azure/active-directory-b2c/custom-email-mailjet).
 - [SendGrid](policy/MailJet) - Custom email verification with SendGrid, [document](https://docs.microsoft.com/azure/active-directory-b2c/custom-email-sendgrid)
 - [SSPR](policy/SSPR) - Azure AD B2C email verification with SSPR technical profile, [document](https://docs.microsoft.comazure/active-directory-b2c/aad-sspr-technical-profile)


## Notes

This sample policy is based on [LocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/LocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 