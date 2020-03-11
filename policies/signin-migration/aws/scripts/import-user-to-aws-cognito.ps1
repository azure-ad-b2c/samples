param (
    [Parameter(Mandatory=$False)][Alias('f')][string]$Path = ".\users.csv",
    [Parameter(Mandatory=$False)][Alias('d')][string]$Delimiter = ";", # the delimiter used in file 
    [Parameter(Mandatory=$False)][Alias('p')][string]$CognitoUserPoolId = "",
    [Parameter(Mandatory=$False)][Alias('t')][string]$TempPassword = "YadaYada1!"
    )

$csv = import-csv -path $path -Delimiter $Delimiter

foreach( $usr in $csv ) {
    write-host "Creating:" $usr.username
    $newUser = (aws cognito-idp admin-create-user --user-pool-id $CognitoUserPoolId --username $usr.username --temporary-password $TempPassword `
                --user-attributes Name=email,Value="$($usr.email)" Name=email_verified,Value=true Name=phone_number,Value="$($usr.phone_number)" Name=phone_number_verified,Value=true Name=name,Value="$($usr.name)" Name=given_name,Value="$($usr.given_name)" Name=family_name,Value="$($usr.family_name)" `
                --message-action SUPPRESS) | ConvertFrom-json
}

