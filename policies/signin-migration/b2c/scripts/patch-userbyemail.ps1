param (
    [Parameter(Mandatory=$True)][Alias('t')][string]$Tenant = "yourtenant",
    [Parameter(Mandatory=$True)][Alias('e')][string]$email = "",
    [Parameter(Mandatory=$True)][Alias('a')][string]$attributeName = "", 
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

#$body = "{`"$attributeName`": `"$attributeValue`" }"
$body = @"
        {
          "$attributeName": "$attributeValue"
        }
"@
write-host $body
$url = "https://graph.windows.net/$tenant/users/$userObjectId/?api-version=1.6"
$authHeader = @{"Authorization"= $env:OAUTH_access_token; }
$result = Invoke-WebRequest -Headers $authHeader -Uri $url -Method Patch -Body $body -ContentType "application/json"

($result.Content | Convertfrom-Json)
