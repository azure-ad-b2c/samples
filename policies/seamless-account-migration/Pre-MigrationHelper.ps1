#Part 1 - Obtain an Access Token to Azure AD Graph API
#AAD B2C tenant
$tenant = "contoso.onmicrosoft.com"
#B2CUserMigration Application Registration Application Id
$ClientID = ""
#B2CUserMigration Application Registration generated key (client secret)
$ClientSecret = ""         
$loginURL = "https://login.microsoftonline.com"
$resource = "https://graph.windows.net"

# Get an OAuth 2 access token based on client id, secret and tenant
$body = @{grant_type="client_credentials";client_id=$ClientID;client_secret=$ClientSecret;resource=$resource}
$oauth = Invoke-RestMethod -Method Post -Uri $loginURL/$tenant/oauth2/token?api-version=1.0 -Body $body

#Part 2 - Register the extension attribute named "requiresMigration" into Azure AD B2C
#ObjectID of the B2CUserMigration App Registration
$AppObjectID = ""

#Set the endpoint to register extension attributes
$url = "https://graph.windows.net/$tenant/applications/$AppObjectID/extensionProperties?api-version=1.6"

#Define the extension attribute
$body = @"
{ 
 "name": "requiresMigration", 
 "dataType": "Boolean", 
 "targetObjects": ["User"]
}
"@

#Generate the authentication header and make the request
$authHeader = @{"Authorization"= $oauth.access_token;"Content-Type"="application/json";"ContentLength"=$body.length }
$result = Invoke-WebRequest -Headers $authHeader -Uri $url -Method Post -Body $body

#Print the full attribute Name
($result.Content | Convertfrom-Json).name

#Part 3 - Create a user object in Azure AD B2C
#Populate the user properties
#Insert the Application Id of the B2CUserMigration App Registration used in Part 2 to register the extension property in the extension name without the dashes in the GUID
$body = @"
{
  "accountEnabled": true,
  "creationType": "LocalAccount",
  "displayName": "John Smith",
  "passwordProfile": {
    "password": "Test*1234RANDOM!",
    "forceChangePasswordNextLogin": false
  },
  "signInNames": [
    {
      "type": "emailAddress",
      "value": "j.smith@contoso.com"
    }
  ],
  "extension_<B2CUserMigration App Id without dashes>_requiresMigration": true
}
"@

#Build the authentication header
$authHeader = @{"Authorization"= $oauth.access_token;"Content-Type"="application/json";"ContentLength"=$body.length }

#Set the endpoint to make the POST request to
$url = "https://graph.windows.net/$tenant/users?api-version=1.6"

#Make the POST request with the body to create the user
Invoke-WebRequest -Headers $authHeader -Uri $url -Method Post -Body $body