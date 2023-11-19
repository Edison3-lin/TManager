################## Build Log function ##########################
function process_log($log)
{
   $timestamp = Get-Date -Format "[yyyy-MM-dd HH:mm:ss] "
   $timestamp+$log | Add-Content $logfile
}
function result_log($log)
{
    Add-Content -Path $outputfile -Value (Get-Date -Format "[yyyy-MM-dd HH:mm:ss] ")
    $timestamp+$log | Add-Content $outputfile
}

$file = Get-Item $PSCommandPath
$Directory = Split-Path -Path $PSCommandPath -Parent
$baseName = $file.BaseName
$logfile = $Directory+'\'+$baseName+"_process.log"
$tempfile = $Directory+'\temp.log'
$outputfile = $Directory+'\'+$baseName+'_result.log'

# Define the FTP server and credentials
$ftpServer = "ftp://127.0.0.1:21"
$username = "test"
$password = "123"

# 本地目录和FTP目录
$localFilePath = "c:\TestManager\ResultUpload\"
$remoteFilePath = '/'

# Create a WebClient object and set credentials
$webClient = New-Object System.Net.WebClient
$webClient.Credentials = New-Object System.Net.NetworkCredential($username, $password)

$files = Get-ChildItem -Path $localFilePath

foreach ($f in $files) {
    $sourceFilePath = $localFilePath+$f
    $destinationFilePath = $remoteFilePath+$f
    try {
        # 使用WebClient上传文件
        $webClient.UploadFile("$ftpServer$destinationFilePath", $sourceFilePath)
    }
    catch {
        process_log "!!!<$f>: $($_.Exception.Message)"
    }
    process_log "<$f> upload to $localFilePath"
}

process_log  "======Upload finished======"
# 釋放 WebClient 資源
$webClient.Dispose()
return 0
