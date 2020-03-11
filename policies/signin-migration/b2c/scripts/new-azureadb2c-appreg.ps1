param (
    [Parameter(Mandatory=$false)][Alias('n')][string]$DisplayName = "",
    [Parameter(Mandatory=$false)][Alias('w')][string]$WriteAccess = $false,
    [Parameter(Mandatory=$false)][Alias('r')][System.Array]$ReplyUrls = @("http://localhost")
    )

# -------------------------------------------------------------------------------------------------------------------------------    
# assumes you have run .\Connect-AzureAdB2C -t "yourtenant" before this

write-host "Getting Tenant info..."
$tenant = Get-AzureADTenantDetail
$tenantName = $tenant.VerifiedDomains[0].Name
write-host "$tenantName`n$($tenant.ObjectId)"

# -------------------------------------------------------------------------------------------------------------------------------    
# This generate the client_secret for the app
function Create-AesManagedObject($key, $IV) {
    $aesManaged = New-Object "System.Security.Cryptography.AesManaged"
    $aesManaged.Mode = [System.Security.Cryptography.CipherMode]::CBC
    $aesManaged.Padding = [System.Security.Cryptography.PaddingMode]::Zeros
    $aesManaged.BlockSize = 128
    $aesManaged.KeySize = 256
    if ($IV) {
        if ($IV.getType().Name -eq "String") {
            $aesManaged.IV = [System.Convert]::FromBase64String($IV)
        } else {
            $aesManaged.IV = $IV
        }
    }
    if ($key) {
        if ($key.getType().Name -eq "String") {
            $aesManaged.Key = [System.Convert]::FromBase64String($key)
        } else {
            $aesManaged.Key = $key
        }
    }
    $aesManaged
}

function Create-AesKey() {
    $aesManaged = Create-AesManagedObject
    $aesManaged.GenerateKey()
    [System.Convert]::ToBase64String($aesManaged.Key)
}

$AppClientSecret = Create-AesKey

$psadCredential = New-Object Microsoft.Azure.Commands.Resources.Models.ActiveDirectory.PSADPasswordCredential
$startDate = Get-Date
$psadCredential.StartDate = $startDate
$psadCredential.EndDate = $startDate.AddYears(1)
$psadCredential.KeyId = [guid]::NewGuid()
$psadCredential.Password = $AppClientSecret

# -------------------------------------------------------------------------------------------------------------------------------    
Function CreateRequiredResourceAccess([string]$ResourceAppId,[string]$ResourceAccessId, [string]$Type) {
    $req = New-Object -TypeName "Microsoft.Open.AzureAD.Model.RequiredResourceAccess"
    $req.ResourceAppId = $ResourceAppId
    $req.ResourceAccess = New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" -ArgumentList $ResourceAccessId,$Type
    return $req
}


write-host "Creating App..."
$app = New-AzureADApplication -DisplayName $displayName -IdentifierUris "http://$tenantName/$displayName" -ReplyUrls $ReplyUrls -PasswordCredentials $psadCredential
write-host "ObjectID:`t$($App.ObjectID)`nClientID:`t`t$($app.AppId)`nSecret:`t$AppClientSecret"

# Add the Required Permissions "Microsoft Graph (Delegated permision)" and "Windows Azure Active Directory (Application permission - read/write directory data)"
write-host "Adding RequiredResourceAccess..."
$req1 = CreateRequiredResourceAccess -ResourceAppId "00000002-0000-0000-c000-000000000000" -ResourceAccessId "78c8a3c8-a07e-4b9e-af1b-b5ccab50a175" -Type "Role"
$req2 = CreateRequiredResourceAccess -ResourceAppId "00000003-0000-0000-c000-000000000000" -ResourceAccessId "e1fe6dd8-ba31-4d61-89e7-88639da4683d" -Type "Scope"

# update the required permissions
Set-AzureADApplication -ObjectId $app.ObjectId -RequiredResourceAccess @($req1, $req2)

write-host "Creating ServicePrincipal..."
$sp = New-AzureADServicePrincipal -AccountEnabled $true -AppId $App.AppId -AppRoleAssignmentRequired $false -DisplayName $displayName 
<#
$App = Get-AzureADApplication -SearchString $displayName
$sp = Get-AzureADServicePrincipal -SearchString $displayName
#>

if ( $WriteAccess -eq $true ) {
    write-host "Adding ServicePrincipal to role DirectoryWriter..."
    $roleWriter = Get-AzureADDirectoryRole | Where-Object { $_.DisplayName -eq "Directory Writers" } 
    Add-AzureADDirectoryRoleMember -ObjectId $roleWriter.ObjectId -RefObjectId $sp.ObjectId
}
