# SignUp with email invitation
This sample console app (.Net core) demonstrates how to send sign-up email invitation. The console application generates a link to sign-up policy with the email address to be validated. The link to the sign-up policy contains the email address, which is encapsulated inside a JWT token (client assertion). When a user clicks on that link, Azure AD B2C validates the JWT token signature, reads the information from the  token, and extracts the email address.

---
## This solution id deprecated!
---

The sample solution contains two policies you can use:
1. **B2C_1A_invite_signup**  - Sign-up invitation policy
1. **B2C_1A_invite_signin_with_mfa**  - Sign-up invitation with MFA policy. The application sends also the user phone number as input claim. User is asked to verify the phone number, without the ability to change the phone number.

The solution also contains the sign-in policy, with and without MFA. Policy names:
1. **B2C_1A_invite_signin** 
1. **B2C_1A_invite_signin_with_mfa**

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Sending Application Data

**Important**: The way Azure AD B2C accepts the client assertion probably will be changed in the future. Until this change, you can use the client assertion method safely. But you should prepare yourself to the changes.

The key of sending data to Azure AD B2C custom policy is to package the data into a JWT token as claims (client assertion). In this case, we send the user's email address to Azure B2C. Sending JWT token requires adding two query strings in the request to the policy.
1.	**client_assertion_type** The value always should be `urn:ietf:params:oauth:client-assertion-type:jwt-bearer`, which is a constant string.
2.	**client_assertion** The value is a JWT token containing input claims for the policy and signed by your application.

### Packaging Data to Send
As described earlier, the data to be sent, needs to be packaged as JWT with claims. In this example, we add the  _ValidationEmail_ claim, which represents the user email address. You can add more claims as nessuccry. The app generates a JWT representation, which can then be used as a client assertion. 

### Signing the client assertion
With client assertion, the client signs the JWT token to prove the token request comes from your web application, by using signing key. You need the signing key, later to store it B2C keys. Your policy uses that key to validate the incoming JWT token, issued by your web application. Use following PowerShell code to generate client secret.

```PowerShell
$bytes = New-Object Byte[] 32
$rand = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rand.GetBytes($bytes)
$rand.Dispose()
$newClientSecret = [System.Convert]::ToBase64String($bytes)
$newClientSecret
```

> Note: the PowerShell generates a secret string. But you can define and use any arbitrary string.


###  Add the sign-in key to Azure AD B2C
As mentioned, Azure AD B2C needs the client secret to validate the incoming JWT token. You need to store the client secret your application uses to sign in, in your Azure AD B2C tenant:  

1.  Go to your Azure AD B2C tenant, and select **B2C Settings** > **Identity Experience Framework**
2.  Select **Policy Keys** to view the keys available in your tenant.
3.  Click **+Add**.
4.  For **Options**, use **Manual**.
5.  For **Name**, use `ClientAssertionSigningKey`.  
    The prefix `B2C_1A_` might be added automatically.
6.  In the **Secret** box, enter your sign-in key you generated earlier
7.  For **Key usage**, use **Encryption**.
8.  Click **Create**
9.  Confirm that you've created the key `B2C_1A_ClientAssertionSigningKey`.

### How Azure AD B2C custom policy read the client assertion?
The Relying Party is responsible to read the input claim (client assertion). Make sure you specify the `client_secret` with the policy key you already created. In this example, we read the input claim `ValidationEmail` to both claim types: _email_ this claim use to store the email address in the user account. And the second one is _ReadOnlyEmail_ for display only

```XML
<CryptographicKeys>
    <Key Id="client_secret" StorageReferenceId="B2C_1A_ClientAssertionSigningKey" />
</CryptographicKeys>
<InputClaims>
    <InputClaim ClaimTypeReferenceId="email" PartnerClaimType="ValidationEmail" />
    <InputClaim ClaimTypeReferenceId="ReadOnlyEmail" PartnerClaimType="ValidationEmail" />
</InputClaims>
```

## Solution artifacts

This sample policy is based on [SocialAndLocalAccountsWithMfa starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa). 
   * All changes are marked with **Demo:** comment inside the policy XML files.
   * Make the necessary changes in the **Action required** comments

### Visual studio solution
* **Program.cs** The `static void Main(string[] args)` contains the code that generates the client assertion, marge the HTML template and send the email
* **appsettings.json** application settings
* **Template.html** The email template. You can customize the template.
* **Models** folder - this folder contains the necessary object-mapping classes 
 
### Application Settings
To test the sample solution, open the `AADB2C.Invite.sln` Visual Studio solution in Visual Studio. In the `AADB2C.Invite` project, open the `appsettings.json`. Replace the app settings with your own values:
* **SMTPServer**: Your SMTP server
* **SMTPPort**: Your SMTP server port number
* **SMTPUsername**: SMTP user name, if necessary
* **SMTPPassword**: SMTP password, if necessary
* **SMTPUseSSL**: SMTP use SSL, true of false
* **SMTPFromAddress**: Send from email address
* **SMTPSubject**: The invitation email's subject
* **ClientSigningKey**: The JTW signature secret you generated earlier with the PowerShell.
* **SignUpUrl**: Full url to your policy, including your tenant name, policy name such as `B2C_1A_invite_signup` or `B2C_1A_invite_signin_with_mfa`, client Id and redirect URL.


For example:

```JSON
  "AppSettings": {
    "SMTPServer": "smtp.sendgrid.net",
    "SMTPPort": 587,
    "SMTPUsername": "sendgrid-service@contoso.com",
    "SMTPPassword": "1234",
    "SMTPUseSSL": true,
    "SMTPFromAddress": "admin@contoso.com",
    "SMTPSubject": "Sign-up account email verification",
    "ClientSigningKey": "VK62QTn0m1hMcn0DQ3RPYDAr6yIiSvYgdRwjZtU5QhI=",
    "SignUpUrl": "https://contoso.b2clogin.com/contoso.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_invite_signin&client_id=0239a9cc-309c-4d41-87f1-31288feb2e82&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login"
  }
```
 
## User flow
1) Run the console app
2) Type user email address and click enter
3) [Optional] Type a phone number with country prefix, such as (+4402072343456). You can skip this step.
4) Open the user's mailbox and click on the **Confirm account** link
5) Provide your password and user profile and click continue
6) If you use MFA, verify your phone number and click continue


## Test your policy
1. Try to send an invite and make user the user is created in the directory
1. Try to run the policy without invitation, make sure B2C presents the unsolicited error message
