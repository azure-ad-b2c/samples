param (
    [Parameter(Mandatory=$true)][Alias('e')][string]$email = "",
    [Parameter(Mandatory=$true)][Alias('n')][string]$DisplayName = "",
    [Parameter(Mandatory=$false)][Alias('f')][string]$From = "noreply@yourdomain.com",
    [Parameter(Mandatory=$false)][string]$FromName    = "YourCompany Online Services",
    [Parameter(Mandatory=$false)][Alias('k')][string]$SendGridApiKey = ""
)

write-host "Generating link"
$url = "https://your-azurefunctionapp.azurewebsites.net/api/Invite?email=$email&displayName=$displayName"
$link = invoke-restmethod -uri $url -method "GET" 

$Subject     = "Signup to our online service"
$MailBody    = "Dear $displayName,<br/><br/>Please use this link to signup to our <a href=`"$link`" target=`"_blank`">online service</a>. The link is valid for 15 minutes.<br/><br/>Best Regards,<br/>$FromName"
$mailbodyType = "text/HTML"

$body = @{
        "personalizations" = @(
            @{
                "to"      = @(
                    @{
                        "email" = $email
                        "name"  = $displayName
                    }
                )
                "subject" = $Subject
            }
        )
        "content"          = @(
            @{
                "type"  = $mailbodyType
                "value" = $MailBody
            }
        )
        "from"             = @{
            "email" = $From
            "name"  = $FromName
        }
    }
$BodyJson = $body | ConvertTo-Json -Depth 4

write-host "Sending invite email to $email"

$resp = Invoke-RestMethod -Method "POST" -Uri "https://api.sendgrid.com/v3/mail/send" `
                -Headers @{Authorization="Bearer $SendGridApiKey";} -ContentType "application/json" -Body $bodyJson