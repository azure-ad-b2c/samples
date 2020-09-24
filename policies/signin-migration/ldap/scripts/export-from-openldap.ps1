param (
    [Parameter(Mandatory=$False)][Alias('s')][string]$LDAPServer = "127.0.0.1",
    [Parameter(Mandatory=$False)][string]$dn = "dc=shihadeh,dc=intern",
    [Parameter(Mandatory=$False)][Alias('u')][string]$user = "admin",
    [Parameter(Mandatory=$True)][Alias('p')][string]$password = "",
    [Parameter(Mandatory=$False)][Alias('f')][string]$Path = ".\users.csv",
    [Parameter(Mandatory=$False)][Alias('g')][string]$PathGroups = ".\groups.csv",
    [Parameter(Mandatory=$False)][Alias('d')][string]$Delimiter = ";" # the delimiter used in file 
    )
# https://github.com/wshihadeh/ldap_server

$domain = "LDAP://$($LDAPServer):389/$($dn)"
$useragent = "cn=$user,$dn"

if ( Test-Path $Path -PathType leaf ) {
    remove-item $Path
}
if ( Test-Path $PathGroups -PathType leaf ) {
    remove-item $PathGroups
}
  
$header = "id;userName;emailAddress;emailVerified;accountEnabled;displayName;surname;givenname;mobile;phoneNumberVerified;password".Replace(";",$Delimiter)
Add-Content -Path $Path -Value $header

Add-Content -Path $PathGroups -Value "id;groupName;member".Replace(";",$Delimiter)

$auth = [System.DirectoryServices.AuthenticationTypes]::FastBind
#$auth = [System.DirectoryServices.AuthenticationTypes]::Secure

write-host "`nConnecting to LDAP server $domain`n"
$root = New-Object -TypeName System.DirectoryServices.DirectoryEntry($domain, $useragent, $password, $auth)
$query = New-Object System.DirectoryServices.DirectorySearcher($root, "(objectclass=*)")
$objClass = $query.findall()
write-host "Total objects found in schema: " $objClass.Count

$count = 0   # count users processed
$countG = 0   # count groups processed

$email_verified = "true"
$enabled = "true"
$phone_number = ""
$phone_number_verified = "false"

foreach( $obj in $objClass ) {
    if ( $obj.Properties.objectclass.Contains("inetOrgPerson") ) {
        $displayName = [string]$obj.Properties.displayName
        if ( "" -eq $displayName ) {
            $displayName = [string]$obj.Properties.cn
        }
        $usrpwd = [System.Text.Encoding]::UTF8.GetString($obj.Properties.userpassword[0])
        $row = "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}" -f "", [string]$obj.Properties.cn, `
                                [string]$obj.Properties.mail, $email_verified, $enabled, $displayName, `
                                [string]$obj.Properties.sn, [string]$obj.Properties.givenname, $phone_number, $phone_number_verified, $usrpwd

        $row = $row.Replace(";",$Delimiter)                                
        Add-Content -Path $Path $row
        $count++
    }
}

foreach( $obj in $objClass ) {
    if ( $obj.Properties.objectclass.Contains("groupOfUniqueNames") ) {
        $members = $obj.Properties.uniquemember.split(" ")
        $countMembers = 0
        foreach( $member in $members ) {
            $parts = $member.split(",")
            $memberUserName = $parts[0].split("=")[1]
            $row = "{0};{1};{2}" -f "", [string]$obj.Properties.cn, $memberUserName
            $row = $row.Replace(";",$Delimiter)                                
            Add-Content -Path $PathGroups $row
            $countMembers++
        }
        # no group members - write a record anyway so we can create an empty group
        if ( 0 -eq $countMembers) {
            $row = "{0};{1};{2}" -f "", [string]$obj.Properties.cn, ""
            $row = $row.Replace(";",$Delimiter)                                
            Add-Content -Path $PathGroups $row
        }
        $countG++
    }
}


write-host "$count users processed"
write-host "$countG groups processed"