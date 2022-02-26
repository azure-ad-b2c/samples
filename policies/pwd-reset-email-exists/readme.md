# Password Reset verification code only sent if email is registered

Demonstrate how to use a [display control](https://docs.microsoft.com/azure/active-directory-b2c/display-controls) to send verification code to users only if the email is registered against a user in the directory.

## Live demo

To test the policy, follow these steps:

1. If you don't have an account, [create a local account](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_signup_signin/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https://jwt.ms&scope=openid&response_type=id_token&prompt=login) with your email address.
1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_PasswordReset_AccountExists/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_PasswordReset_AccountExists* policy to reset the password.
1. Perform the following test:
    1. In the *Email Address* provide an email address that is not registered in that directory. For example, `emily@contoso.com`, or `bob@fabrikam.com`, and select *Send verification code*. You should get an error message that the email address is not registered in the system.
    1. [Run](https://b2clivedemo.b2clogin.com/b2clivedemo.onmicrosoft.com/B2C_1A_Demo_PasswordReset_AccountExists/oauth2/v2.0/authorize?client_id=cfaf887b-a9db-4b44-ac47-5efff4e2902c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login) the *B2C_1A_Demo_PasswordReset_AccountExists* policy again. This time type the email address your register in the first step. Complete the password reset process. 

## Prerequisites

- You can automate the pre requisites by visiting the [setup tool](https://aka.ms/iefsetup) if you already have an Azure AD B2C tenant. Some policies can be deployed directly through this app via the **Experimental** menu.

- You will require to [create an Azure AD B2C directory](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant).

- To use the sample policies in this repo, follow the instructions here to [setup your AAD B2C environment for Custom Policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started-custom).

## How it works

Before generating and sending a verification code, we first take the users email and lookup the directory for a user. If a user is returned we will have the objectId claim in the claims bag. 

Using a precondition, on the basis of the objectId existing in the claims bag, we will send out the verification code. The XML snippet below demonstrates this.

```xml
<Action Id="SendCode">
<ValidationClaimsExchange>
    <ValidationClaimsExchangeTechnicalProfile TechnicalProfileReferenceId="AAD-UserReadUsingEmailAddress-emailAddress" />
    <ValidationClaimsExchangeTechnicalProfile TechnicalProfileReferenceId="AadSspr-SendCode">
    <Preconditions>
        <Precondition Type="ClaimsExist" ExecuteActionsIf="false">
        <Value>objectId</Value>
        <Action>SkipThisValidationTechnicalProfile</Action>
        </Precondition>
    </Preconditions>
    </ValidationClaimsExchangeTechnicalProfile>
</ValidationClaimsExchange>
</Action>
```


## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).
