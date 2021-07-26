param (
    [Parameter(Mandatory=$False)][Alias('t')][string]$Tenant = "",
    [Parameter(Mandatory=$True)][Alias('f')][string]$Path = ".\users.csv",
    [Parameter(Mandatory=$False)][Alias('d')][string]$Delimiter = ";", # the delimiter used in file 
    [Parameter(Mandatory=$False)][Alias('c')][string]$client_id = "", # the Client ID used to register the attribute
    [Parameter(Mandatory=$false)][switch]$ImportPassword = $False
    )

if ( $null -eq $env:OAUTH_access_token ) {
    write-error "environment variable OAUTH_access_token not set"
    exit 1
}

if ( !($Tenant -imatch ".onmicrosoft.com") ) {
    $Tenant = $Tenant + ".onmicrosoft.com"
}

# if no appObjectId given, use the standard b2c-extensions-app
if ( "" -eq $client_id ) {
    $appExt = Get-AzureADApplication -SearchString "b2c-extensions-app"
    $client_id = $appExt.AppId   
}

$tmpPwd = "Aa$([guid]::NewGuid())!"

$extId = $client_id.Replace("-", "") # the name is w/o hyphens
$requiresMigrationAttributeName = "extension_$($extId)_requiresMigration"
$phoneNumberVerifiedAttributeName = "extension_$($extId)_phoneNumberVerified"

function CreateUserInB2C( $usr ) {
    $pwd = $tmpPwd
    $requiresMigrationAttribute = "true"
    $passwordPolicies = "`"passwordPolicies`": `"DisablePasswordExpiration`","
    # if we DO have the password in the CSV file, we are good to go and need no further migration
    if ( $True -eq $ImportPassword -and $usr.password.Length -gt 0 ) {
        $pwd = $usr.password
        $requiresMigrationAttribute = "false"
        $passwordPolicies = "`"passwordPolicies`": `"DisablePasswordExpiration,DisableStrongPassword`","
    }

    $mobileLine = ""
    if ( "" -ne $usr.mobile ) {
        $mobileLine = "`"mobile`": `"$($usr.mobile)`","
    }
    $enabled=$usr.accountEnabled.ToLower()
    $body = @"
        {
          "accountEnabled": $enabled,
          "creationType": "LocalAccount",
          "displayName": "$($usr.displayName)",
          "surname": "$($usr.surname)",
          "givenname": "$($usr.givenname)",
          $mobileLine
          $passwordPolicies
          "passwordProfile": {
            "password": "$pwd",
            "forceChangePasswordNextLogin": false
          },
          "signInNames": [
            {
                "type": "userName",
                "value": "$($usr.userName)"
            },  
            {
              "type": "emailAddress",
              "value": "$($usr.emailAddress)"
            }
          ],
          "$requiresMigrationAttributeName": $requiresMigrationAttribute,
          "$phoneNumberVerifiedAttributeName": $($usr.phoneNumberVerified)
        }
"@
    
    write-host "Creating user: $($usr.userName) / $($usr.emailAddress)"
    #write-host $body
    $authHeader = @{"Authorization"= $env:OAUTH_access_token;"Content-Type"="application/json";"ContentLength"=$body.length }
    $url = "https://graph.windows.net/$tenant/users?api-version=1.6"
    try {
        $newUser = Invoke-WebRequest -Headers $authHeader -Uri $url -Method Post -Body $body    
        $userObjectID = ($newUser.Content | ConvertFrom-json).objectId
        write-host -BackgroundColor Black -ForegroundColor Green "$email == $userObjectID"
        #write-host $newUser
        #write-host $newUser.Content
        $countCreated += 1
    } catch {
        $exception = $_.Exception
        write-host -BackgroundColor Black -ForegroundColor Red -NoNewLine "StatusCode: " $exception.Response.StatusCode.value__ " "
        $streamReader = [System.IO.StreamReader]::new($exception.Response.GetResponseStream())
        $streamReader.BaseStream.Position = 0
        $streamReader.DiscardBufferedData()
        $errBody = $streamReader.ReadToEnd()
        $streamReader.Close()
        write-host -BackgroundColor Black -ForegroundColor Red "Error: " $errBody    
    }
}


$csv = import-csv -path $path -Delimiter $Delimiter

$count = 0
foreach( $usr in $csv ) {
    CreateUserInB2C $usr
    $count += 1
}
write-output "Imported $count users"

