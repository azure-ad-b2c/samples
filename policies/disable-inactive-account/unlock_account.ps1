$tenant  = "something.onmicrosoft.com"

$oauth = $null
$ClientID      = ""                # Should be a ~35 character string insert your info here
$ClientSecret  = ""         
$loginURL      = "https://login.microsoftonline.com"
$resource = "https://graph.windows.net"

# Get an Oauth 2 access token based on client id, secret and tenant domain
$body       = @{grant_type="client_credentials";client_id=$ClientID;client_secret=$ClientSecret;resource=$resource}
$oauth      = Invoke-RestMethod -Method Post -Uri $loginURL/$tenant/oauth2/token?api-version=1.0 -Body $body

#find user
Connect-AzureAD
get-azureaduser -all $true | Where-Object {$_.signinnames.value -eq "somebody@microsoft.com"} | fl accountenabled, objectid
get-azureaduser -all $true | Where-Object {$_.signinnames.value -eq "somebody@microsoft.com"} | Select-Object -ExpandProperty extensionproperty

#update user
$objectId = "insert-objectid"
$url = "https://graph.windows.net/$tenant/users/$objectId`?api-version=1.6"

$currentdate = (Get-Date).ToUniversalTime()
$body = @"
{
    "extension_yourAppIdWithoutDashes_lastLogonTime" : "$currentdate"
}
"@
$authHeader = @{"Authorization"= $oauth.access_token;"Content-Type"="application/json";"ContentLength"=$body.length }
Invoke-WebRequest -Headers $authHeader -Uri $url -Method Patch -Body $body