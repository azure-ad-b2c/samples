param (
    [Parameter(Mandatory=$false)][Alias('t')][string]$Tenant = ""
    )

if ( $Tenant.Length -eq 36 -and $Tenant.Contains("-") -eq $true)  {
    $TenantID = $Tenant
} else {
    if ( !($Tenant -imatch ".onmicrosoft.com") ) {
        $Tenant = $Tenant + ".onmicrosoft.com"
    }
    $url = "https://login.windows.net/$Tenant/v2.0/.well-known/openid-configuration"
    $resp = Invoke-RestMethod -Uri $url
    $TenantID = $resp.authorization_endpoint.Split("/")[3]    
    write-output $TenantID
}

$startTime = Get-Date

$ctx = Connect-AzureAD -tenantid $TenantID

$finishTime = Get-Date
$TotalTime = ($finishTime - $startTime).TotalSeconds
Write-Output "Time: $TotalTime sec(s)"        

write-output $ctx

$host.ui.RawUI.WindowTitle = "PS AAD - $($ctx.Account.Id) - $($ctx.TenantDomain)"
