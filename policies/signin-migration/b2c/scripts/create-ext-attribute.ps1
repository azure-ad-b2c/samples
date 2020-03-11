param (
    [Parameter(Mandatory=$True)][Alias('t')][string]$Tenant = "yourtenant",
    [Parameter(Mandatory=$True)][Alias('o')][string]$appObjectId = "", 
    [Parameter(Mandatory=$True)][Alias('n')][string]$attributeName = "", 
    [Parameter(Mandatory=$False)][Alias('d')][string]$dataType = "String"
    )

if ( !($Tenant -imatch ".onmicrosoft.com") ) {
    $Tenant = $Tenant + ".onmicrosoft.com"
}

$url = "https://graph.windows.net/$tenant/applications/$AppObjectID/extensionProperties?api-version=1.6"
#Define the extension attribute
$body = @"
{ 
 "name": "$attributeName", 
 "dataType": "$dataType", 
 "targetObjects": ["User"]
}
"@

#Generate the authentication header and make the request
$authHeader = @{"Authorization"= $env:OAUTH_access_token; }
$result = Invoke-WebRequest -Headers $authHeader -Uri $url -Method Post -Body $body -ContentType "application/json"

($result.Content | Convertfrom-Json).name

