param (
    [Parameter(Mandatory=$False)][Alias('u')][string]$UserName = ""
    )

# invoke the Azure Function and pass userid/password to authenticate the user  
#$url = "http://localhost:7071/api/UserMigrationValidateUser"    # if you run it in vscode
$url = "https://your-azfunc-api-name.azurewebsites.net/api/ValidateUserAwsCognito?code=iR...A=="    

# ask for credentials
$cred = Get-Credential -UserName $UserName -Message "Enter userid for $Tenant"

try {
    $body = @{email=$cred.UserName;password=$cred.GetNetworkCredential().Password;mobile="+46111222333"}
    $result = Invoke-RestMethod $url -Method Post -Body (ConvertTo-Json $body) -ContentType "application/json" 
} catch {
    # NotFound = user is not found in the Azure STorage Table for migrated users
    # Conflict = password was wrong
    #$status = $_.Exception.Response.StatusCode
    $status = $_.Exception.Response.StatusCode.value__
    $result = $_.Exception.Response.GetResponseStream()
	$reader = New-Object System.IO.StreamReader($result)
	$reader.BaseStream.Position = 0
	$reader.DiscardBufferedData()
	$result = $reader.ReadToEnd();
	$result = $status.ToString() + ": " + ($result | ConvertFrom-Json)    
}

write-host $result
