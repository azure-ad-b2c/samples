param (
    [Parameter(Mandatory=$False)][Alias('t')][string]$Tenant = "",
    [Parameter(Mandatory=$False)][Alias('c')][string]$client_id = "",
    [Parameter(Mandatory=$False)][Alias('s')][string]$client_secret = ""
    )

$env:OAUTH_access_token=""
if ( "" -eq $client_id ) { $client_id = $env:client_id }
if ( "" -eq $client_secret ) { $client_secret = $env:client_secret }

if ( "" -eq $Tenant ) {
    $tenant = (Get-AzureADTenantDetail).VerifiedDomains[0].Name
}

if ( $Tenant.Length -eq 36 -and $Tenant.Contains("-") -eq $true)  {
    $b2ctenant = $Tenant
} else {
    if ( !($Tenant -imatch ".onmicrosoft.com") ) {
        $b2ctenant = $Tenant + ".onmicrosoft.com"
    } else {
        $b2ctenant = $Tenant
    }
}

# Get an OAuth 2 access token based on client id, secret and tenant
$body = @{grant_type="client_credentials";client_id=$client_id;client_secret=$client_secret;resource="https://graph.windows.net"}
$oauth = Invoke-RestMethod -Method Post -Uri "https://login.microsoftonline.com/$b2ctenant/oauth2/token?api-version=1.0" -Body $body

$env:OAUTH_access_token=$oauth.access_token
write-host $b2ctenant
write-host "Environment variable set `$env:OAUTH_access_token"
write-host "Access Token valid until " (Get-Date -Date "01/01/1970").AddSeconds($oauth.expires_on).ToString("s") 

