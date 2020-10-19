param (
    [Parameter(Mandatory=$True)][Alias('f')][string]$Path = ".\newusers.csv",
    [Parameter(Mandatory=$False)][Alias('d')][string]$Delimiter = ";" # the delimiter used in file 
    )

$url = "https://yourazfunc.azurewebsites.net/api/TableStorageUser?code=..."

function CreateUserInTableStorage( $usr ) {

    write-host "Creating user: $($usr.userName) / $($usr.emailAddress)"

    $body = @{ 
        op="save";
        objectId=$usr.id;
        Email=$usr.emailAddress;
        DisplayName=$usr.displayName;
        GivenName=$usr.givenname;
        SurName=$usr.surname;
        Mobile=$usr.mobile;
        Password=$usr.password;
    }
    
    Invoke-RestMethod -Method Post -Uri $url -Body ($body | ConvertTo-json) -ContentType "application/json"
    $countCreated += 1
}

$csv = import-csv -path $path -Delimiter $Delimiter

$count = 0
foreach( $usr in $csv ) {
    CreateUserInTableStorage $usr
    $count += 1
}
write-output "Created users: $count"

