param (
    [Parameter(Mandatory=$true)][Alias('h')][string]$HostName,      # "waws-prod-xxxxxxxx.ftp.azurewebsites.windows.net"
    [Parameter(Mandatory=$true)][Alias('u')][string]$FtpUserid,     # get this from the publishing profile
    [Parameter(Mandatory=$true)][Alias('p')][string]$FtpPassword,   # get this from the publishing profile
    [Parameter(Mandatory=$true)][Alias('l')][string]$LocalFile,     # full path to file to upload
    [Parameter(Mandatory=$true)][Alias('r')][string]$RemoteFile     # "/site/wwwroot/TableStorageUser/run.csx"
    )

write-host "ftp $LocalFile --> $HostName/$RemoteFile"
$ftp = [System.Net.FtpWebRequest]::Create("ftp://$HostName/$RemoteFile")
$ftp = [System.Net.FtpWebRequest]$ftp
$ftp.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
$ftp.Credentials = new-object System.Net.NetworkCredential($FtpUserid,$FtpPassword)
$ftp.UseBinary = $true
$ftp.UsePassive = $true
$content = [System.IO.File]::ReadAllBytes($LocalFile)
$ftp.ContentLength = $content.Length
$rs = $ftp.GetRequestStream()
$rs.Write($content, 0, $content.Length)
$rs.Close()
$rs.Dispose()
