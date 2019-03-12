# Custom email verification
Custom email verification solution allows you to send your own custom email verification during sign-up or password reset user journey. The solution requires using Azure AD B2C custom policy and a REST API endpoint that sends and verifies the email address.

The solution includes:
1. REST API to **send** the email verification with the TOTP code
1. REST API to **verify** the email address with the TOTP code
1. HTML page (Azure AD B2C content definition) with the JavaScript code that calls the REST API endpoint to send and verify the email address.
1. Custom policy that includes a claim type that collects the TOPT code from the end user. A validation technical profile the calls the `verify` endpoint.

The key concept of custom email verification: During sign-up or password reset, a client-side JavaScirpt calls the **send** REST API endpoint with the email address. Based on a combination of the email address and a secret, the send endpoint generates a TOTP code and sends the email with the TOTP code to the end-user email address. User copies the TOTP code and types in the sign-up or password reset page. Then clicks on verify to call the `verify` endpoint (client side). After the email is verified, the user continues providing personal data, or resetting the password and clicks continue. The B2C custom policy, reads the validation code (the TOTP code) and run another call (this time from the server side) to revalidate the provided TOTP code. 

## Solution artifacts
### HTML Page
The self-asserted HTML page (source-code\AADB2C.EmailVerification\AADB2C.EmailVerification\wwwroot\selfAsserted.html) contains the JavaScript to send and verify the email address.

### REST API
The REST API has a single controller (source-code\AADB2C.EmailVerification\AADB2C.EmailVerification\Controllers\IdentityController.cs) with two endpoints: **Send** and **Verify**. Under the **Models** folder you will find the models, such as input claims, HTTP message return the endpoints, and application settings. 

### App Settings
source-code\AADB2C.EmailVerification\AADB2C.EmailVerification\appsettings.json contains the application setting, including:

* **SMTPServer**, SMTP server name, or IP address	 
* **SMTPPort**, SMTP server port number
* **SMTPUsername**, SMTP server username	 
* **SMTPPassword**, SMTP server password	 
* **SMTPUseSSL**, Indicating if SSL is required to communicate with the SMTP server	 
* **SMTPFromAddress**, The from address to appear in the verification email	 
* **SMTPSubject**, The subject to be appears in the verification email
* **TOTPSecret**, The TOTP secret. Note: the actual TOTP secret is a combination of this value and the email address provided by the end user
* **TOTPStep**, The TOTP code validity time window.

### Generate a secret
Use following PowerShell code to generate TOTP secret.

```PowerShell
$bytes = New-Object Byte[] 32
$rand = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rand.GetBytes($bytes)
$rand.Dispose()
$newClientSecret = [System.Convert]::ToBase64String($bytes)
$newClientSecret
```

> [!NOTE]
>
>The PowerShell generates a secret string. But you can define and use any arbitrary string.
>

