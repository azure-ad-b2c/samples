$tenant  = "YourTenant.onmicrosoft.com"

$oauth = $null
$ClientID      = ""                # Should be a ~35 character string insert your info here
$ClientSecret  = ""         
$loginURL      = "https://login.microsoftonline.com"
$tenantdomain  = $tenant        # For example, contoso.onmicrosoft.com
$resource = "https://graph.windows.net"

# Get an Oauth 2 access token based on client id, secret and tenant domain
$body       = @{grant_type="client_credentials";client_id=$ClientID;client_secret=$ClientSecret;resource=$resource}
$oauth      = Invoke-RestMethod -Method Post -Uri $loginURL/$tenantdomain/oauth2/token?api-version=1.0 -Body $body

#Print the token to the screen
$oauth 

#Configure the extension property
#Application objectID of the application being used to auth wit (objectID of Graph Service App)
$AppObjectID = ""
$url = "https://graph.windows.net/YourTenant.onmicrosoft.com/applications/$AppObjectID/extensionProperties?api-version=1.6"
$body = @"
{ 
 “name”: "isMigrated", 
 “dataType”: “String”, 
 “targetObjects”: [“User”]
}
"@
$authHeader = @{"Authorization"= $oauth.access_token;"Content-Type"="application/json";"ContentLength"=$body.length }
$result = Invoke-WebRequest -Headers $authHeader -Uri $url -Method Post -Body $body
#Attribute Name
($result.Content | Convertfrom-Json).name

#NEED COMPANY ADMIN ROLE ASSIGNED onwards
Add-MsolRoleMember -RoleObjectId 62e90394-69f5-4237-9190-012177145e10 -RoleMemberObjectId a335c382-97a3-44ca-ac4c-c110c32224ab -RoleMemberType ServicePrincipal
#RoleObjectID - static for company admin
#RoleMemberObjectID -> ObjectID for the servicePrincipal of the AAD App used to authenticate.

#Create the User
$body = @"
{
  "accountEnabled": true,
  "creationType": "LocalAccount",
  "displayName": "Alex Wu",
  "passwordProfile": {
    "password": "Test1234RANDOM",
    "forceChangePasswordNextLogin": false
  },
  "extension_74467a80b26148fcbaa42f7b7f335d4c_isMigrated": "true",
  "signInNames": [
    {
      "type": "emailAddress",
      "value": "test@onmicrosoft.com"
    }
  ]
}
"@

$authHeader = @{"Authorization"= $oauth.access_token;"Content-Type"="application/json";"ContentLength"=$body.length }
$url = "https://graph.windows.net/YourTenant.onmicrosoft.com/users?api-version=1.6"
Invoke-WebRequest -Headers $authHeader -Uri $url -Method Post -Body $body

#update user
$objectId = ""
$url = "https://graph.windows.net/jasb2c.onmicrosoft.com/users/$objectId`?api-version=1.6"

$body = @"
{
    "extension_4b05c02c61284f83a3f5e291714d1857_birthday" : "hello"
}
"@
$authHeader = @{"Authorization"= $oauth.access_token;"Content-Type"="application/json";"ContentLength"=$body.length }
Invoke-WebRequest -Headers $authHeader -Uri $url -Method Patch -Body $body

