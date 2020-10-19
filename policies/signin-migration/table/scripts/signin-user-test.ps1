param (
    )


$credential = Get-Credential -Message "Enter Table Storage credentials"

$url = "https://yourazfunc.azurewebsites.net/api/TableStorageUser?code=..."

$body = @{ 
    email=$credential.GetNetworkCredential().username;
    password=$credential.GetNetworkCredential().password;
}
    
Invoke-RestMethod -Method Post -Uri $url -Body ($body | ConvertTo-json) -ContentType "application/json"
