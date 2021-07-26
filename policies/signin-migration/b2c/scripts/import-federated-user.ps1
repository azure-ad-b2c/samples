param (
    [Parameter(Mandatory=$False)][Alias('t')][string]$Tenant = "",
    [Parameter(Mandatory=$False)][Alias('i')][string]$Issuer = "google.com",
    [Parameter(Mandatory=$False)][Alias('u')][string]$IssuerUserId = "107...80",
    [Parameter(Mandatory=$False)][Alias('d')][string]$DisplayName = "Alice Contoso (gmail)",
    [Parameter(Mandatory=$False)][Alias('f')][string]$FirstName = "Alice",
    [Parameter(Mandatory=$False)][Alias('l')][string]$LastName = "Contoso",
    [Parameter(Mandatory=$False)][Alias('e')][string]$email = "alicecontoso@gmail.com"
    )

if ( $null -eq $env:OAUTH_access_token ) {
    write-error "environment variable OAUTH_access_token not set"
    exit 1
}

$Bytes = [System.Text.Encoding]::UTF8.GetBytes($IssuerUserId)
$IssuerUserId64 =[Convert]::ToBase64String($Bytes)

$body = @"
        {
          "accountEnabled": false,
          "displayName": "$DisplayName",
          "surname": "$LastName",
          "givenname": "$FirstName",
          "mailNickname": "unknown",
          "otherMails": [
            "$email"
            ],
          "passwordPolicies": "DisablePasswordExpiration",
          "passwordProfile": null,
          "userIdentities": [
            {
                "issuer": "$Issuer",
                "issuerUserId": "$IssuerUserId64"
            }
          ],
          "userPrincipalName": "cpim_$([guid]::NewGuid())@$Tenant"
        }
"@
  
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



