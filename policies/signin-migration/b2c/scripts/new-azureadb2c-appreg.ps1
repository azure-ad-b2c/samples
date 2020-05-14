param (
    [Parameter(Mandatory=$false)][Alias('n')][string]$DisplayName = "",
    [Parameter(Mandatory=$false)][Alias('w')][string]$WriteAccess = $false,
    [Parameter(Mandatory=$false)][Alias('r')][System.Array]$ReplyUrls = @("http://localhost")
    )

# -------------------------------------------------------------------------------------------------------------------------------    
write-output "Getting Tenant info..."
$tenant = Get-AzureADTenantDetail
$tenantName = $tenant.VerifiedDomains[0].Name
write-output "$tenantName`n$($tenant.ObjectId)"

# -------------------------------------------------------------------------------------------------------------------------------    
$requiredResourceAccessW=@"
[
    {
        "resourceAppId": "00000002-0000-0000-c000-000000000000",
        "resourceAccess": [
            {
                "id": "78c8a3c8-a07e-4b9e-af1b-b5ccab50a175",
                "type": "Scope"
            }
        ]
    },
    {
        "resourceAppId": "00000003-0000-0000-c000-000000000000",
        "resourceAccess": [
            {
                "id": "e1fe6dd8-ba31-4d61-89e7-88639da4683d",
                "type": "Scope"
            },
            {
                "id": "1bfefb4e-e0b5-418b-a88f-73c46d2cc8e9",
                "type": "Role"
            }
        ]
    }
]
"@ | ConvertFrom-json

$requiredResourceAccessR=@"
[
    {
        "resourceAppId": "00000002-0000-0000-c000-000000000000",
        "resourceAccess": [
            {
                "id": "311a71cc-e848-46a1-bdf8-97ff7156d8e6",
                "type": "Scope"
            }
        ]
    },
    {
        "resourceAppId": "00000003-0000-0000-c000-000000000000",
        "resourceAccess": [
            {
                "id": "e1fe6dd8-ba31-4d61-89e7-88639da4683d",
                "type": "Scope"
            }
        ]
    }
]
"@ | ConvertFrom-json

if ( $WriteAccess -eq $true ) { 
    $requiredResourceAccess = $requiredResourceAccessW 
} else { 
    $requiredResourceAccess = $requiredResourceAccessR
}

$reqAccess=@()
foreach( $resApp in $requiredResourceAccess ) {
    $req = New-Object -TypeName "Microsoft.Open.AzureAD.Model.RequiredResourceAccess"
    $req.ResourceAppId = $resApp.resourceAppId
    foreach( $ra in $resApp.resourceAccess ) {
        $req.ResourceAccess += New-Object -TypeName "Microsoft.Open.AzureAD.Model.ResourceAccess" -ArgumentList $ra.Id,$ra.type
    }
    $reqAccess += $req
}

write-output "Creating App..."
$app = New-AzureADApplication -DisplayName $displayName -IdentifierUris "http://$tenantName/$displayName" -ReplyUrls $ReplyUrls -PasswordCredentials $psadCredential -RequiredResourceAccess $reqAccess
$startDate = Get-Date
$endDate = $startDate.AddYears(1)
$appKey = New-AzureADApplicationPasswordCredential -ObjectId $App.ObjectID -CustomKeyIdentifier "key1" -StartDate $startDate -EndDate $endDate
write-output "ObjectID:`t$($App.ObjectID)`nClientID:`t`t$($app.AppId)`nSecret:`t$($appKey.Value)"

write-output "Creating ServicePrincipal..."
$sp = New-AzureADServicePrincipal -AccountEnabled $true -AppId $App.AppId -AppRoleAssignmentRequired $false -DisplayName $displayName 
<#
$App = Get-AzureADApplication -SearchString $displayName
$sp = Get-AzureADServicePrincipal -SearchString $displayName
#>

if ( $WriteAccess -eq $true ) {
    $env:client_id=$app.AppId
    $env:client_secret=$appKey.Value
    write-output "Adding ServicePrincipal to role DirectoryWriter..."
    $roleWriter = Get-AzureADDirectoryRole | Where-Object { $_.DisplayName -eq "Directory Writers" } 
    Add-AzureADDirectoryRoleMember -ObjectId $roleWriter.ObjectId -RefObjectId $sp.ObjectId
} else {
    $env:web_client_id=$app.AppId
    $env:web_client_secret=$appKey.Value
    write-output "Sleeping 15 seconds to wait for replication"
    Start-Sleep 15
    write-output "Updating SignInAudience to AzureADandPersonalMicrosoftAccount"
    $oauthBody  = @{grant_type="client_credentials";resource="https://graph.microsoft.com/";client_id=$env:client_id;client_secret=$env:client_secret;scope="https://graph.microsoft.com/.default Application.ReadWrite.All"}
    $oauth      = Invoke-RestMethod -Method Post -Uri "https://login.microsoft.com/$tenantName/oauth2/token?api-version=1.0" -Body $oauthBody
    $body = @{ SignInAudience = "AzureADandPersonalMicrosoftAccount" }
    $apiUrl = "https://graph.microsoft.com/v1.0/applications/$($app.objectId)"
    Invoke-RestMethod -Uri $apiUrl -Headers @{Authorization = "Bearer $($oauth.access_token)" }  -Method PATCH -Body $($body | convertto-json) -ContentType "application/json"
}
