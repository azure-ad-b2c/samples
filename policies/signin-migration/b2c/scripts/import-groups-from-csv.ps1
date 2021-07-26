param (
    [Parameter(Mandatory=$False)][Alias('t')][string]$Tenant = "",
    [Parameter(Mandatory=$True)][Alias('f')][string]$Path = ".\groups.csv",
    [Parameter(Mandatory=$False)][Alias('d')][string]$Delimiter = ";"  # the delimiter used in file 
    )

if ( $null -eq $env:OAUTH_access_token ) {
    write-error "environment variable OAUTH_access_token not set"
    exit 1
}

if ( !($Tenant -imatch ".onmicrosoft.com") ) {
    $Tenant = $Tenant + ".onmicrosoft.com"
}

function GrapAPI_GET( $url ) {
    $authHeader = @{"Authorization"= $env:OAUTH_access_token; }
    $result = Invoke-WebRequest -Headers $authHeader -Uri $url -Method GET -ContentType "application/json"
    return $result
}
function GraphAPI_POST( $url, $body ) {
    $objectID = ""
    $authHeader = @{"Authorization"= $env:OAUTH_access_token;"Content-Type"="application/json";"ContentLength"=$body.length }
    try {
        $resp = Invoke-WebRequest -Headers $authHeader -Uri $url -Method Post -Body $body    
        $objectID = ($resp.Content | ConvertFrom-json).objectId
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
    return $objectId
}

$currentGroupName = ""
$currentGroupObjectID = ""

$csv = import-csv -path $path -Delimiter $Delimiter

$count = 0
$countGroups = 0
$countMembers = 0
foreach( $group in $csv ) {

    if ( $currentGroupName -ne $group.groupName ) {
        $createNew = $true
        # check to see if Group exists already        
        $url = "https://graph.windows.net/$tenant/groups?`$filter=displayName eq '$($group.groupName)'&`$select=objectId,displayName&api-version=1.6"
        $result = GrapAPI_GET $url
        if ($result.StatusCode -eq 200 ) {
            $grp=($result.Content | ConvertFrom-json)
            if ( $grp.value.Count -gt 0 ) {
                $currentGroupObjectID = $grp.value[0].objectId
                $createNew = $false
            }
        }
        if ( $True -eq $createNew ) {
            write-host "Creating group: $($group.groupName)"
            $body = @"
        {
          "displayName": "$($group.groupName)",
          "mailNickname": "$($group.groupName)",
          "mailEnabled": false,
          "securityEnabled": true
        }
"@
            #write-host $body
            $url = "https://graph.windows.net/$tenant/groups?api-version=1.6"
            $currentGroupObjectID = GraphAPI_POST $url $body 
            write-host -BackgroundColor Black -ForegroundColor Green "$currentGroupObjectID"         
            $countGroups++   
        }
        $currentGroupName = $group.groupName
    }
    # add member
    if ( "" -ne $group.member ) {
        # find userObject by username
        $url = "https://graph.windows.net/$tenant/users?`$filter=signInNames/any(x:x/value eq '$($group.member)')&`$select=objectId&api-version=1.6"
        $result = GrapAPI_GET $url
        if ($result.StatusCode -eq 200 ) {
            $user=($result.Content | ConvertFrom-json)
            if ( $user.value.Length -gt 0 ) {
                $userObjectId = $user.value.objectId
                # add member
                $url = "https://graph.windows.net/$tenant/groups/$currentGroupObjectID/`$links/members?api-version=1.6"
                $body = "{`"url`":`"https://graph.windows.net/$tenant/directoryObjects/$userObjectId`"}"
                $ret = GraphAPI_POST $url $body 
                if ( "" -ne $ret ) {
                    write-host "Add member $($group.member) to group $($group.groupName)"
                    $countMembers++
                }
            } else {
                write-host "User $($group.member) not found. Can't add to group $($group.groupName)"
            }
        }
    }

    $count++
}
write-output "$count rows read"
write-output "Imported $countGroups groups and $countMembers members"
