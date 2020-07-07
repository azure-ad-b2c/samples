param (
    [Parameter(Mandatory=$True)][Alias('p')][string]$CognitoUserPoolId = "",
    [Parameter(Mandatory=$False)][Alias('f')][string]$Path = ".\users.csv",
    [Parameter(Mandatory=$False)][Alias('d')][string]$Delimiter = ";" # the delimiter used in file 
    )


if ( Test-Path $Path -PathType leaf ) {
    remove-item $Path
}
  

$header = "id;userName;emailAddress;emailVerified;accountEnabled;displayName;surname;givenname;mobile;phoneNumberVerified".Replace(";",$Delimiter)
Add-Content -Path $Path -Value $header
#write-output $header

function GetAttributeValue( $usr, $attrName ) {
    $value=($usr.Attributes | Where-object {$_.Name -eq $attrName}).Value
    if ( $null -ne $value ) {
        return $value
    } else {
        return ""
    }
}

# migrate AWS Cognito users into Azure AD B2C. Note that if you got the user in B2C "Deleted users" a new user will be created with a new objectid
function ExportUsers( $users ) {
    foreach( $usr in $users.Users ) {
        $username=$usr.Username
        $enabled=$usr.Enabled.ToString().ToLower()
        $email_verified = ($usr.Attributes | Where-object {$_.Name -eq "email_verified"}).Value
        $email=($usr.Attributes | Where-object {$_.Name -eq 'email'}).Value
        $name = GetAttributeValue $usr "name"
        $lastname = GetAttributeValue $usr "family_name"
        $givenname = GetAttributeValue $usr "given_name"
        $phone_number = GetAttributeValue $usr "phone_number"
        $phone_number_verified = GetAttributeValue $usr "phone_number_verified"
        $sub = GetAttributeValue $usr "sub"

        $row = "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}" -f $sub, $username, `
                                $email, $email_verified, $enabled, $name, $lastname, $givenname, $phone_number, $phone_number_verified
        $row = $row.Replace(";",$Delimiter)                                
        #write-output $row
        Add-Content -Path $Path $row
    }
}

$count = 0   # count AWS Cognito users processed
$limit = 10  # batch size

$users = (aws cognito-idp list-users --user-pool-id $CognitoUserPoolId --limit $limit --output "json") | ConvertFrom-json
while ($null -ne $Users.PaginationToken) {
    ExportUsers $users
    $count += $users.Users.Count
    write-host "$count users processed, $countW created"
    $users = (aws cognito-idp list-users --user-pool-id $CognitoUserPoolId --limit $limit --pagination-token $Users.PaginationToken --output "json") | ConvertFrom-json
} 
ExportUsers $users
$count += $users.Users.Count
write-host "$count users processed"


