# Setting up **Sign in with Apple** as a Custom OpenID Connect identity provider

> Disclaimer: **Sign in with Apple** is currently in beta and, as such, may be subject to change. This sample is provided AS IS - a best effort will be made to update this sample as the service evolves.

This sample shows how to enable **Sign in with Apple** as an identity provider in Azure AD B2C. **Sign in with Apple** uses an authentication protocol with is very close to OpenID Connect - close enough that we can use these features in Azure AD B2C to build the integration.

These instructions will guide you to:
- Create the application you will need in the Apple Developer portal
- Create the JWT client secret value which is required to initiate an OpenID Connect flow with Apple, including:
  - Generating and downloading the private key
  - Understand how to structure the client secret JWT
  - Generate and cryptographically sign the client secret JWT
- Host an OIDC metadata endpoint for the **Sign in with Apple** service
- Use **Sign in with Apple** in a user flow (built-in policy)

## Creating an application in the Apple Developer portal
To setup **Sign in with Apple**, you will need to create an App ID and a service ID in the Apple Developer portal.

1. If you do not already have an Apple Developer subscription, you will need to register and pay for one at https://developer.apple.com.
2. On the Apple Developer portal, go to **Certificates, Identifiers, & Profiles**.
3. On the **Identifers** tab, click the **(+)** button.
![Creating a new app identifier in the Apple Developer Portal](media/apple_newapp.jpg)
4. On the **Register a New Identifer** page, choose **App IDs** and click **Continue**. (App IDs include one or more Service IDs.)
![Registering a new app identifier in the Apple Developer Portal](media/apple_registerapp.jpg)
5. On the **Register an App ID** page, provide a description and a bundle ID, and select **Sign in with Apple** from the capabilities list. Then click **Continue** and . Take note of your **App ID Prefix (Team ID)** from this step, you will need it later.
![Configuring a new app identifier in the Apple Developer Portal](media/apple_configureapp1.jpg)
![Configuring a new app identifier in the Apple Developer Portal](media/apple_configureapp2.jpg)
6. Review the app registration information and click **Register**.
7. Again, on the **Identifers** tab, click the **(+)** button.
![Creating a new service identifier in the Apple Developer Portal](media/apple_newapp.jpg) 
8. On the **Register a New Identifer** page, choose **Services IDs** and click **Continue**.
![Registering a new service identifier in the Apple Developer Portal](media/apple_registerservice.jpg)
9. On the **Register a Services ID** page, provide a description and an identifier. The description is what will be show to the user on the consent screen, and the identifier will be your client ID for the OpenID Connect flow. Then click on **Configure**.
![Registering a new service identifier in the Apple Developer Portal](media/apple_configureservice1.jpg)
10. On the pop-up window, specify the App ID you just created as the Primary App ID. Specify your application's domain in the domain section (you do not need to verify it unless you intend to use the email relay service). For the return URL, use your B2C tenant's reply URL such as `https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/oauth2/authresp`. Then click **Add** and **Save**.
![Registering a new service identifier in the Apple Developer Portal](media/apple_configureservice2.jpg)
11. Review the service registration information and click **Save**.

## Creating the client secret

Apple has taken the interesting and unusual approach of requiring app developers to create and sign a JWT token which is used as the client secret value in OpenID Connect flows. This requires you to generate and download an elliptic curve private key from the Apple Developer portal and use it to sign a JWT with a specific payload.

### Creating and downloading the private key
1. On the **Keys** tab in the Apple Developer portal, choose **Create a key** or click the **(+)** button.
![Creating a new key in the Apple Developer Portal](media/apple_newkey.jpg)
2. On the **Register a New Key** page give the key a name, check the box next to **Sign in with Apple** and click **Configure**.
![Registering a key in the Apple Developer Portal](media/apple_registerkey.jpg)
3. On the **Configure Key** page, link the key to the primary app ID you created previously and click **Save**.
![Configuring a key in the Apple Developer Portal](media/apple_configurekey.jpg)
4. Finish creating the key by confirming the information and clicking **Continue** and then reviewing the information and clicking **Register**.
5. On the **Download Your Key** page, download the key. It will download as a `.p8` (PKCS#8) file - you'll use this to sign your client secret JWT. After finishing the process, you can come back later to revoke the key, in case you ever need to.
![Downloading a key in the Apple Developer Portal](media/apple_downloadkey.jpg)

### Structuring the client secret JWT
Apple requires the JWT token which will be used as your client secret to have a payload structured like this example:
```json
{
  "alg": "ES256",
  "typ": "JWT"
}.{
  "sub": "com.yourcompany.app1",
  "nbf": 1560203207,
  "exp": 1560289607,
  "iss": "ABC123DEFG",
  "aud": "https://appleid.apple.com"
}.[Signature]
```
- **sub**: The Apple Service ID (also the client ID)
- **iss**: Your Apple Developer Team ID
- **aud**: Apple is receiving the token, so they are the audience
- **exp**: No more than six months after **nbf**

_Note: Apple does not accept client secret JWTs with an expiration date more than six months after the creation (or not-before) date. That means you'll need to rotate your client secret, at minimum, every six months._

### Signing the client secret JWT
You'll use the `.p8` file you downloaded previously to sign the client secret JWT. This file is a [PCKS#8 file](https://en.wikipedia.org/wiki/PKCS_8) which contains the private signing key in PEM format. There are many libraries which can create and sign the JWT for you. One way of doing this is by running a small amount of code in an Azure Function.

The [`SigninWithApple_ClientSecret` Azure Function](source-code/B2CSignInWithApple/SigninWithApple_ClientSecret/run.csx) in the source code of this sample accepts a POST request with a JSON body like this example:
```json
{
    "appleTeamId": "ABC123DEFG",
    "appleServiceId": "com.yourcompany.app1",
    "p8key": "MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQg+s07NiAcuGEu8rxsJBG7ttupF6FRe3bXdHxEipuyK82gCgYIKoZIzj0DAQehRANCAAQnR1W/KbbaihTQayXH3tuAXA8Aei7u7Ij5OdRy6clOgBeRBPy1miObKYVx3ki1msjjG2uGqRbrc1LvjLHINWRD"
}
```
- **appleTeamId**: Your Apple Developer Team ID
- **appleServiceId**: The Apple Service ID (also the client ID)
- **p8key**: The PEM format key - you can obtain this by opening the `.p8` file in a text editor, and copying everything between `-----BEGIN PRIVATE KEY-----` and `-----END PRIVATE KEY-----` without line breaks

The function will respond with a properly formatted and signed client secret JWT in a response like this example:
```json
{
    "token": "eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJjb20ueW91cmNvbXBhbnkuYXBwMSIsIm5iZiI6MTU2MDI2OTY3NSwiZXhwIjoxNTYwMzU2MDc1LCJpc3MiOiJBQkMxMjNERUZHIiwiYXVkIjoiaHR0cHM6Ly9hcHBsZWlkLmFwcGxlLmNvbSJ9.Dt9qA9NmJ_mk6tOqbsuTmfBrQLFqc9BnSVKR6A-bf9TcTft2XmhWaVODr7Q9w1PP3QOYShFXAnNql5OdNebB4g"
}
```

This token is the client secret value you will use to configure the OpenID Connect federation.

You can create this Azure Function by publishing the `B2CSignInWithApple` Visual Studio project from the `source-code` folder of this project.

_Note: The sample Azure Function generates a JWT which is valid for 180 days. You may want to change this in the code depending on your desired secret rotation frequency._

_Note: Ensure the `WEBSITE_LOAD_USER_PROFILE` configuration setting is set to `1` in your Azure Functions app environment - this provides access to the cryptographic context which is required to load the private key._

## Creating the OIDC metadata endpoint

Apple does not expose an OpenID Connect metadata endpoint. In order to use it as an identity provider in Azure AD B2C, you must host a metadata document which B2C can access over the internet. You could host this file in Azure Blob Storage (with public access enabled), on your website, in an [Azure Function](source-code/B2CSignInWithApple/SigninWithApple_OpenidConfiguration/run.csx), or in some other location. In the source code for this sample, there is a function app which exposes this metadata.

The metadata document is a JSON file with the following content:
```json
{
    "issuer": "https://appleid.apple.com",
    "authorization_endpoint": "https://appleid.apple.com/auth/authorize",
    "token_endpoint": "https://appleid.apple.com/auth/token",
    "jwks_uri": "https://appleid.apple.com/auth/keys"
}
```

_Note: Ensure the URL to the metadata ends in exactly: `/.well-known/openid-configuration`_

## Using **Sign in with Apple** in a user flow (built-in policy)

Now that you have created and collected the necessary configuration values which are necessary to configure **Sign in with Apple** as an identity provider, you can do so in the Azure AD B2C blade in the Azure Portal.

Choose *OpenID Connect* as the identity provider type and use these configuration values:
- **Metadata url:** URL to the metadata endpoint you created
- **Client id:** The Apple Service ID (e.g. com.mycompany.app1)
- **Client secret:** The signed JWT you created
- **Scope:** `name` and/or `email` may be specified, however, please see the note below
- **Response type:** code
- **Response mode:** form_post

Apple only provides a single useful claim in their ID tokens, the `sub` claim. You should map this to both the User ID and Display Name claims.
![Claims mapping in Azure AD B2C](media/b2c_claimsmapping.jpg)

_Note: even if you include the `name` or `email` scopes, nether will not be included as claims in the token which Azure AD B2C receives from Apple. Apple provides these values in a non-standard way as part of the authorization response payload. Azure AD B2C will not be able to read these values and you should collect them from the user separately if needed._

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].

If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).

To provide product feedback, visit the [Azure Active Directory B2C Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).