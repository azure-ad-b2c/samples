# Password Reset OTP only sent if Email is registered

Demonstrate how to use a displayControl to send One-Time-Passcodes to users only if the email is registered against a user in the directory.

## How it works
Before generating and sending an OTP, we first take the users email and lookup the directory for a user. If a user is returned we will have the objectId claim in the claimbag. 

Using a precondition, on the basis of the objectId existing in the claimbag, we will send out the OTP. The XML snippet below demonstrates this.

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

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
