param (
    [Parameter(Mandatory=$False)][Alias('t')][string]$Tenant = "yourtenant",
    [Parameter(Mandatory=$False)][Alias('c')][string]$client_id = "89...c2",            # an App registered in the AAD tenant (not B2C blade) that has r/w directory data permission
    [Parameter(Mandatory=$True)][Alias('e')][string]$email = "",
    [Parameter(Mandatory=$True)][Alias('n')][string]$attributeName = "", 
    [Parameter(Mandatory=$False)][Alias('v')][string]$attributeValue = ""
    )

if ( !($Tenant -imatch ".onmicrosoft.com") ) {
    $Tenant = $Tenant + ".onmicrosoft.com"
}

$url = "https://graph.windows.net/$tenant/users?`$filter=signInNames/any(x:x/value eq '$email')&api-version=1.6"
$authHeader = @{"Authorization"= $env:OAUTH_access_token; }
$result = Invoke-WebRequest -Headers $authHeader -Uri $url -Method GET -ContentType "application/json"

$user=($result.Content | ConvertFrom-json).value
$userObjectId = $user.objectId

$attrName = "extension_" + $client_id.Replace("-", "") + "_" + $attributeName

$body = @"
{ "$attrName": "$attributeValue" }
"@
write-host $body
$url = "https://graph.windows.net/$tenant/users/$userObjectId/?api-version=1.6"
$authHeader = @{"Authorization"= $env:OAUTH_access_token; }
$result = Invoke-WebRequest -Headers $authHeader -Uri $url -Method Patch -Body $body -ContentType "application/json"

($result.Content | Convertfrom-Json)
