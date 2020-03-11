param (
    [Parameter(Mandatory=$True)][Alias('t')][string]$Tenant = "yourtenant",
    [Parameter(Mandatory=$False)][Alias('e')][string]$email = ""
    )

if ( $null -eq $env:OAUTH_access_token ) {
    write-error "environment variable OAUTH_access_token not set"
    exit 1
}
    
if ( !($Tenant -imatch ".onmicrosoft.com") ) {
    $Tenant = $Tenant + ".onmicrosoft.com"
}
$url = "https://graph.windows.net/$tenant/users?`$filter=signInNames/any(x:x/value eq '$email')&api-version=1.6"
$authHeader = @{"Authorization"= $env:OAUTH_access_token; }
$result = Invoke-WebRequest -Headers $authHeader -Uri $url -Method GET -ContentType "application/json"

$user=($result.Content | ConvertFrom-json).value
$userObjectId = $user.objectId

$user
