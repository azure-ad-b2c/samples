$region = "eu-west-1"
if ( $env:PATH -imatch "/usr/bin" ) {
    $uniquePrefix="$($env:USER)$(Get-Random)".ToLower()         # Mac/Linux
} else {
    $uniquePrefix="$($env:USERNAME)$(Get-Random)".ToLower()     # Windows
}
$poolName = "$($uniquePrefix)DemoPool"
$clientName = "$($uniquePrefix)App"
$clientNameRopc = "$($uniquePrefix)AppRopc"
$scopeResName = "$($uniquePrefix)AppScopes"
$scopeRead = "demo.read"
$scopeWrite = "demo.write"
$replyUrl = "[http://localhost:3000/]"

# https://medium.com/@robert.broeckelmann/openid-connect-authorization-code-flow-with-aws-cognito-246997abd11a
# https://lobster1234.github.io/2018/05/31/server-to-server-auth-with-amazon-cognito/

# create user pool
$newPool = (aws cognito-idp create-user-pool --region $region --output "json" --pool-name $poolName --policies "PasswordPolicy={MinimumLength=8,RequireUppercase=true,RequireLowercase=true,RequireNumbers=true,RequireSymbols=true}" --mfa-configuration "OFF" --auto-verified-attributes "email" --alias-attributes "email") | ConvertFrom-json

$env:AWS_UserPoolId = $newPool.UserPool.Id # set envvar for reference

$newScope = (aws cognito-idp create-resource-server --region $region --output "json" --user-pool-id $newPool.UserPool.Id `
             --name $scopeResName --identifier $scopeResName `
             --scopes "ScopeName=$scopeRead,ScopeDescription=read permission" "ScopeName=$scopeWrite,ScopeDescription=write permission") | ConvertFrom-json

$newDomain = (aws cognito-idp create-user-pool-domain --region $region --output "json" --user-pool-id $newPool.UserPool.Id --domain $clientName.ToLower()) | ConvertTo-json

# create client app for WebApp
$newClient = (aws cognito-idp create-user-pool-client --region $region --output "json" --user-pool-id $newPool.UserPool.Id --client-name $clientName --generate-secret `
            --explicit-auth-flows "ALLOW_ADMIN_USER_PASSWORD_AUTH" "ALLOW_CUSTOM_AUTH" "ALLOW_REFRESH_TOKEN_AUTH" "ALLOW_USER_PASSWORD_AUTH" "ALLOW_USER_SRP_AUTH" `
            --allowed-o-auth-scopes "$scopeResName/$scopeRead" ) | ConvertFrom-json

# create client app for ROPC (has no client secret which is key to success!)
$newClientRopc = (aws cognito-idp create-user-pool-client --region $region --output "json" --user-pool-id $newPool.UserPool.Id --client-name $clientNameRopc --no-generate-secret `
                --explicit-auth-flows "ALLOW_ADMIN_USER_PASSWORD_AUTH" "ALLOW_CUSTOM_AUTH" "ALLOW_REFRESH_TOKEN_AUTH" "ALLOW_USER_PASSWORD_AUTH" "ALLOW_USER_SRP_AUTH" ) | ConvertFrom-json

#aws cognito-idp update-user-pool-client --region $region --user-pool-id $newPool.UserPool.Id --client-id $newClient.UserPoolClient.ClientId --default-redirect-uri $replyUrl 
#$newClient = (aws cognito-idp create-user-pool-client --region $region --output "json" --user-pool-id $newPool.UserPool.Id --client-name $clientName --no-generate-secret --explicit-auth-flows "ALLOW_ADMIN_USER_PASSWORD_AUTH" "ALLOW_CUSTOM_AUTH" "ALLOW_REFRESH_TOKEN_AUTH" "ALLOW_USER_PASSWORD_AUTH" "ALLOW_USER_SRP_AUTH" --allowed-o-auth-scopes "$scopeResName/$scopeRead" --allowed-o-auth-flows "code" "implicit" --allowed-o-auth-flows-user-pool-client --default-redirect-uri $replyUrl) | ConvertFrom-json

write-host "https://cognito-idp.$region.amazonaws.com/$($newPool.UserPool.Id)/.well-known/openid-configuration"

$resp = Invoke-WebRequest -Uri "https://cognito-idp.$region.amazonaws.com/$($newPool.UserPool.Id)/.well-known/openid-configuration"
$oidccfg = ($resp.Content | ConvertFrom-json)
$oidccfg.authorization_endpoint  # these endpoints does a 302 redirect to /login, so if you invoke you need to honour redirects
$oidccfg.token_endpoint

# manual steps that I couldn't script
write-host "TODO in the AWS Portal:"
write-host "For App client $clientName : "
write-host "   1. Enable OAuth 2.0 auth code grant, check scope 'email', 'openid' and 'profile'"
write-host "   2. Check Scopes demo.read + demo.write "
write-host "   3. Add Callback URLs as needed, like http://localhost:3000/callback, https://www.getpostman.com/oauth2/callback"
write-host "   4. Save"
write-host ""
write-host "then, import users by running script .\import-user-to-aws-cognito.ps1 -p [<pool-id>] -f .\users.csv"

# to verify the users you imported to AWS
# find user pool id
<#
$pools = (aws cognito-idp list-user-pools --max-results 60) | ConvertFrom-json
$pool = $pools | where-object {$_.UserPools.Name -eq "$poolName"}

# list users
$limit = 10
$users = (aws cognito-idp list-users --user-pool-id $pool.UserPools.Id --limit $limit --output "json") | ConvertFrom-json
$users.Users
while ($null -ne $Users.PaginationToken) {
    $users.Users
    $users = (aws cognito-idp list-users --user-pool-id $pool.UserPools.Id --limit $limit --pagination-token $Users.PaginationToken --output "json") | ConvertFrom-json
} 
#>

# next step is to run script .\migrate-users-from-aws-cognito.ps1 to migrate the users

