param (
    [Parameter(Mandatory=$False)][Alias('t')][string]$Tenant = "",
    [Parameter(Mandatory=$False)][Alias('p')][string]$CognitoUserPoolId = "",
    [Parameter(Mandatory=$False)][Alias('c')][string]$client_id = "" # the Client ID used to register the attribute
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

# function to return "a-value" or null as a text string
function GetAttributeValueOrNull( $usr, $attrName ) {
    $value=($usr.Attributes | Where-object {$_.Name -eq $attrName}).Value
    if ( $null -ne $value ) {
        return "`"$value`""
    } else {
        return "null"
    }
}
# migrate AWS Cognito users into Azure AD B2C. Note that if you got the user in B2C "Deleted users" a new user will be created with a new objectid
function CreateUsersInB2C( $users ) {

    $countCreated = 0
    foreach( $usr in $users.Users ) {
        $username=$usr.Username
        $enabled=$usr.Enabled.ToString().ToLower()
        # this is a tough one - if email isn't verified, should we really migrate the user to B2C? B2C assumes the user is verified
        # and we have no means of proving the user owns the acount if we don't have a verified email and we can't do password reset, etc
        # if we migrate it, the user will still be able to signin
        # for now, we do nothing and continue. One thing would perhaps be to flip $enabled to false. 
        $email_verified = ($usr.Attributes | Where-object {$_.Name -eq "email_verified"}).Value
        $email=($usr.Attributes | Where-object {$_.Name -eq 'email'}).Value
        $name = GetAttributeValueOrNull $usr "name"
        # B2C requires a displayName and AWS Cognito doesn't require a "name", so use the username as plan B
        if ( "null" -eq $name ) {
            $name = "`"$username`""
        }
        $lastname = GetAttributeValueOrNull $usr "family_name"
        $givenname = GetAttributeValueOrNull $usr "given_name"
        $mobile = GetAttributeValueOrNull $usr "phone_number"
        $phone_number_verified = GetAttributeValueOrNull $usr "phone_number_verified"
        # add other attributes you like to migrate. AWS Cognito syntax for custom attributes is "cognito:attrname"
        # if the passwords are known and weak and you want to import them, add DisableStrongPassword tp passwordPolicies
        $body = @"
        {
          "accountEnabled": $enabled,
          "creationType": "LocalAccount",
          "displayName": $name,
          "surname": $lastname,
          "givenname": $givenname,
          "mobile": $mobile,
          "passwordPolicies": "DisablePasswordExpiration",
          "passwordProfile": {
            "password": "$tmpPwd",
            "forceChangePasswordNextLogin": false
          },
          "signInNames": [
            {
                "type": "userName",
                "value": "$username"
            },  
            {
              "type": "emailAddress",
              "value": "$email"
            }
          ],
          "$requiresMigrationAttributeName": true,
          "$phoneNumberVerifiedAttributeName": $phone_number_verified
        }
"@
    
        write-host "Creating user: $username / $email"
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
    return $countCreated
}

$countW = 0  # count users created
$count = 0   # count AWS Cognito users processed
$limit = 10  # batch size

$users = (aws cognito-idp list-users --user-pool-id $CognitoUserPoolId --limit $limit --output "json") | ConvertFrom-json
while ($null -ne $Users.PaginationToken) {
    $countW += CreateUsersInB2C $users
    $count += $users.Users.Count
    write-host "$count users processed, $countW created"
    $users = (aws cognito-idp list-users --user-pool-id $CognitoUserPoolId --limit $limit --pagination-token $Users.PaginationToken --output "json") | ConvertFrom-json    
} 
$countW += CreateUsersInB2C $users
$count += $users.Users.Count
write-host "$count users processed, $countW created"


