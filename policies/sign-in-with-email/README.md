# Sign-in with email
This sample demonstrates how to sign-in to a web application by sending a sign-in link. The web application sends an email to the end user with a link to sign-in policy. The link to the sign-in policy contains the email address, which is encapsulated inside a JWT token (id_token_hint). When a user clicks on that link, Azure AD B2C validates the JWT token signature, reads the information from the token, extracts the email address and issues an access token back to the application.

## Sending Application Data
The key of sending data to Azure AD B2C custom policy is to package the data into a JWT token as claims (id_token_hint). In this case, we send the user's email address to Azure B2C. Sending JWT token requires to host the necessary metadata endpoints required to use the "id_token_hint" parameter in Azure AD B2C.

ID tokens are JSON Web Tokens (JWTs) and, in this application, are signed using RSA certificates. This application hosts an Open ID Connect metatdata endpoint and JSON Web Keys (JWKs) endpoint which are used by Azure AD B2C to validate the signature of the ID token.

The web app has following endpoints:
* **/.well-known/openid-configuration**, set this URL in the **IdTokenHint_ExtractClaims** technical profile
* **/.well-known/keys**

## Disclaimer
The sample app is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. 
The app is provided AS IS without warranty of any kind.

### Creating a signing certificate
The sample application uses a self-signed certificate to sign the ID tokens. You can generate a valid self-signed certificate for this purpose and get the thumbprint using PowerShell *(note: Run as Administrator)*:
```Powershell
$cert = New-SelfSignedCertificate -Type Custom -Subject "CN=MySelfSignedCertificate" -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3") -KeyUsage DigitalSignature -KeyAlgorithm RSA -KeyLength 2048 -NotAfter (Get-Date).AddYears(2) -CertStoreLocation "Cert:\CurrentUser\My"
$cert.Thumbprint
```

### Configuring the application
Update the *appSettings* values in **appsettings.json** with the information for your Azure AD B2C tenant and the signing certificate you just created.
* **B2CTenant**: Your Azure AD B2C tenant name (without *.onmicrosoft.com*)
* **B2CPolicy**: The policy which you'd like to send the id_token_hint
* **B2CClientId**: The application ID for the Azure AD B2C app you'd like to redirect to
* **B2CRedirectUri**: The target redirect URI for your application
* **B2CSignUpUrl** the link to B2C format
* **SigningCertThumbprint**: The thumbprint for the signing certificate you just created
* **SigningCertAlgorithm**: The certificate algorithm (must be an RSA algorithm)
* **LinkExpiresAfterMinutes**: Link expiration (in minutes) 
* **SMTPServer**: Your SMTP server
* **SMTPPort**: Your SMTP server port number
* **SMTPUsername**: SMTP user name, if necessary
* **SMTPPassword**: SMTP password, if necessary
* **SMTPUseSSL**: SMTP use SSL, true of false
* **SMTPFromAddress**: Send from email address
* **SMTPSubject**: The invitation email's subject


### Running the application
When you run the application, you'll be able to enter the email of a user. When you click on **Send sign-in email**, the app sends a sign-in email to the account you specified.

To inspect the generated token, copy and paste it into a tool like [JWT.ms](htttps://jwt.ms).

### Hosting the application in Azure App Service
If you publish the application to Azure App Service, you'll need to configure a valid certificate with a private key in Azure App Service.
1. First, export your certificate as a PFX file using the User Certificates management tool (or create a new one)
2. Upload your certificate in the **Private Certificates** tab of the **SSL Settings** blade of your Azure App Service
3. Follow [these instructions](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-ssl-cert-load#load-your-certificates) to ensure App Service loads the certificate when the app runs

## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the nessacery changes in the **Sample action required** sections. 
