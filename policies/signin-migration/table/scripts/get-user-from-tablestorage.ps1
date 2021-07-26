param (
    [Parameter(Mandatory=$True)][Alias('u')][string]$email
    )



$url = "https://yourazfunc.azurewebsites.net/api/TableStorageUser?code=..."

$body = @{ 
    op="getbyemail";
    Email=$email;
}
    
Invoke-RestMethod -Method Post -Uri $url -Body ($body | ConvertTo-json) -ContentType "application/json"
