# Migrating CIAM solution from AWS Cognito to B2C - Website for testing Auth

This is a tiny website written in node.js and you need to have node and npm installed. Instructions for installing can be found [here](https://nodejs.org/en/download/).

The node.js app is based on the examples in this [github repo](https://github.com/lelylan/simple-oauth2) and it was selected as it is not using any Microsft libraries and therefor serves as an example that it is possible to migrate to B2C if the webapp uses open standards for its authentication.

## Builing and starting the website
To get the website up and running, you need to do the following
1. [Install node and npm](https://nodejs.org/en/download/)
2. Open a command prompt in the ***website*** folder and run ``npm install`` to install the website dependancies
3. Edit file [start-website-aws.cmd](start-website-aws.cmd) to update 
* CLIENT_ID = from the AWS Console under ***General settings*** > ***App clients*** (not the *Ropc app)
* CLIENT_SECRET = from the AWS Console under ***General settings*** > ***App clients*** (not the *Ropc app)
* AUTH_DOMAIN = from the AWS Console under ***App integration*** > ***Domain name*** (copy everything from https:// to .com)
* SCOPES = <your-prefix>AppScopes/demo.read under ***App integration*** > ***App client settings*** (copy everything from https:// to .com)
4. Start the website by the command ``start-website-aws.cmd`` 

## How to use the website for AWS Cognito
Open your browser and navigate to [http://localhost:3000/](http://localhost:3000/) and click ***Login with AWS Cognito***. You will be redirected to AWS Cognito's login page and the first time you signin with an imported user you will be required to change the password. If you successful authenticate, you will be presented with a page with links to view your tokens with jwt.ms. Remember that you have a session going in your browser and subsequent login attempts will bypass the AWS Cognito login page and give you the token directly

---
## Next step is to migrate users
To migrate users, you should continue [here](../b2c/README.md)

---
## Migrating website to use Azure AD B2C
When you have migrated the users, it is time to modify the webapp to use B2C as its Identity Provider.

### Register an Azure Application and test the migrated users
In Azure portal and in the B2C tenant.

1. Goto ***App Registrations*** blade and select ***+ New registration***
2. Give the app a name (B2C migration testapp) and specify ``http:localhost:3000/callback`` for the ***Redirect URI***, Register
3. Edit file [start-website-b2c.cmd](start-website-b2c.cmd) to update
* B2C_TENANT=yourtenant (without.onmicrosoft.com)
* CLIENT_ID=Application (client) ID from the portal
* CLIENT_SECRET=create a key in ***Certificates & secrets*** in the portal. (Remember to escape characters as needed) 
4. Start the website by the command ``start-website-b2c.cmd`` 

You can script the App Registration if you like by running the following commands. 

```Powershell
.\connect-azureadb2c.ps1 -t yourtenant.onmicrosoft.com # interactive login using your Azure AD credentials

.\new-azureadb2c-appreg.ps1 -n "B2C-migration-website-name" -w $False -r @("http://localhost:3000/callback")

Getting Tenant info...
yourtenant.onmicrosoft.com
Creating App...
ObjectID:       ab...cd
ClientID:       ef...gh
Secret:         de...8RCw=
Adding RequiredResourceAccess...
Creating ServicePrincipal...
```

You need to Grant permission for this app i portal.azure.com under ***API Permissions***

Notice that we don't change to a Microsoft authentication library and that we are still using simple-oauth which knows nothing about Azure AD B2C.

Open your browser and navigate to [http://localhost:3000/](http://localhost:3000/) and click ***Login with Azure AD B2C***. You will be redirected to B2C's Custom Policy signin page and the first time you signin, the policy will check with AWS Cognito, via your Azure Function, and it AWS says thumbs up on the authentication, the Custom Policy will complete the migration via setting the password in the B2C tenant. If the ``phoneNumberVerified`` attribute is ``True``, the phone number will be updated as the verified MFA number. You can see that MFA has been migrated by viewing the "Phone" attribute under ***Authentication methods***

You can see the migration in action if you open the Logs / Console window on the Azure Function.