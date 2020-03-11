param (
    [Parameter(Mandatory=$False)][Alias('c')][string]$client_id = "...",
    [Parameter(Mandatory=$False)][Alias('u')][string]$email = "",
    [Parameter(Mandatory=$False)][Alias('r')][string]$region = "eu-west-1"
    )

$cred = Get-Credential -UserName $email -Message "Enter AWS Cognito username/email"

$email = $cred.UserName;
$pwd = $cred.GetNetworkCredential().Password

$body = @"
{
  "AuthParameters" : {
    "USERNAME" : "$email",
    "PASSWORD" : "$pwd"
  },
  "AuthFlow" : "USER_PASSWORD_AUTH",
  "ClientId" : "$client_id"
}
"@

$headers = @{"X-Amz-Target"= "AWSCognitoIdentityProviderService.InitiateAuth";"Content-Type"="application/x-amz-json-1.1";"ContentLength"=$body.length }
$url = "https://cognito-idp.$region.amazonaws.com/"

try {
    $resp = Invoke-WebRequest -Headers $headers -Uri $url -Method Post -Body $body
    $resp
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

