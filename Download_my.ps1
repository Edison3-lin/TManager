# TypeName: System.Net.WebClient

# Name                      MemberType Definition                                                               
# ----                      ---------- ----------                                                               
# AllowReadStreamBuffering  Property   bool AllowReadStreamBuffering {get;set;}                                 
# AllowWriteStreamBuffering Property   bool AllowWriteStreamBuffering {get;set;}                                
# BaseAddress               Property   string BaseAddress {get;set;}                                            
# CachePolicy               Property   System.Net.Cache.RequestCachePolicy CachePolicy {get;set;}               
# Container                 Property   System.ComponentModel.IContainer Container {get;}                        
# Credentials               Property   System.Net.ICredentials Credentials {get;set;}                           
# Encoding                  Property   System.Text.Encoding Encoding {get;set;}                                 
# Headers                   Property   System.Net.WebHeaderCollection Headers {get;set;}                        
# IsBusy                    Property   bool IsBusy {get;}                                                       
# Proxy                     Property   System.Net.IWebProxy Proxy {get;set;}                                    
# QueryString               Property   System.Collections.Specialized.NameValueCollection QueryString {get;set;}
# ResponseHeaders           Property   System.Net.WebHeaderCollection ResponseHeaders {get;}                    
# Site                      Property   System.ComponentModel.ISite Site {get;set;}                              
# UseDefaultCredentials     Property   bool UseDefaultCredentials {get;set;}                                    

# CancelAsync               Method     void CancelAsync()                                                                
# CreateObjRef              Method     System.Runtime.Remoting.ObjRef CreateObjRef(type requestedType)                   
# Dispose                   Method     void Dispose(), void IDisposable.Dispose()                                        
# DownloadData              Method     byte[] DownloadData(string address), byte[] DownloadData(uri address)             
# DownloadDataAsync         Method     void DownloadDataAsync(uri address), void DownloadDataAsync(uri address, System...
# DownloadDataTaskAsync     Method     System.Threading.Tasks.Task[byte[]] DownloadDataTaskAsync(string address), Syst...
# DownloadFile              Method     void DownloadFile(string address, string fileName), void DownloadFile(uri addre...
# DownloadFileAsync         Method     void DownloadFileAsync(uri address, string fileName), void DownloadFileAsync(ur...
# DownloadFileTaskAsync     Method     System.Threading.Tasks.Task DownloadFileTaskAsync(string address, string fileNa...
# DownloadString            Method     string DownloadString(string address), string DownloadString(uri address)         
# DownloadStringAsync       Method     void DownloadStringAsync(uri address), void DownloadStringAsync(uri address, Sy...
# DownloadStringTaskAsync   Method     System.Threading.Tasks.Task[string] DownloadStringTaskAsync(string address), Sy...
# Equals                    Method     bool Equals(System.Object obj)                                                    
# GetHashCode               Method     int GetHashCode()                                                                 
# GetLifetimeService        Method     System.Object GetLifetimeService()                                                
# GetType                   Method     type GetType()                                                                    
# InitializeLifetimeService Method     System.Object InitializeLifetimeService()                                         
# OpenRead                  Method     System.IO.Stream OpenRead(string address), System.IO.Stream OpenRead(uri address) 
# OpenReadAsync             Method     void OpenReadAsync(uri address), void OpenReadAsync(uri address, System.Object ...
# OpenReadTaskAsync         Method     System.Threading.Tasks.Task[System.IO.Stream] OpenReadTaskAsync(string address)...
# OpenWrite                 Method     System.IO.Stream OpenWrite(string address), System.IO.Stream OpenWrite(uri addr...
# OpenWriteAsync            Method     void OpenWriteAsync(uri address), void OpenWriteAsync(uri address, string metho...
# OpenWriteTaskAsync        Method     System.Threading.Tasks.Task[System.IO.Stream] OpenWriteTaskAsync(string address...
# ToString                  Method     string ToString()                                                                 
# UploadData                Method     byte[] UploadData(string address, byte[] data), byte[] UploadData(uri address, ...
# UploadDataAsync           Method     void UploadDataAsync(uri address, byte[] data), void UploadDataAsync(uri addres...
# UploadDataTaskAsync       Method     System.Threading.Tasks.Task[byte[]] UploadDataTaskAsync(string address, byte[] ...
# UploadFile                Method     byte[] UploadFile(string address, string fileName), byte[] UploadFile(uri addre...
# UploadFileAsync           Method     void UploadFileAsync(uri address, string fileName), void UploadFileAsync(uri ad...
# UploadFileTaskAsync       Method     System.Threading.Tasks.Task[byte[]] UploadFileTaskAsync(string address, string ...
# UploadString              Method     string UploadString(string address, string data), string UploadString(uri addre...
# UploadStringAsync         Method     void UploadStringAsync(uri address, string data), void UploadStringAsync(uri ad...
# UploadStringTaskAsync     Method     System.Threading.Tasks.Task[string] UploadStringTaskAsync(string address, strin...
# UploadValues              Method     byte[] UploadValues(string address, System.Collections.Specialized.NameValueCol...
# UploadValuesAsync         Method     void UploadValuesAsync(uri address, System.Collections.Specialized.NameValueCol...
# UploadValuesTaskAsync     Method     System.Threading.Tasks.Task[byte[]] UploadValuesTaskAsync(string address, Syste...


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

function download_file($file)
{
    $localFilePath = $localPath+$file
    $remoteFilePath = '/'+$file
    # process_log "FTP: $ftpServer$remoteFilePath"
    # process_log "Down: $localFilePath"
   
    # Download the file
    $webClient.DownloadFile("$ftpServer$remoteFilePath", $localFilePath)
    process_log "File downloaded to $localFilePath"
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

# Specify the remote file to download
# $remoteFile = @("abt1.ps1","abt2.ps1","abt3.ps1")

# Specify the local destination for the downloaded file
$localPath = "c:\TestManager\Download\"

# Create a WebClient object and set credentials
$webClient = New-Object System.Net.WebClient
$webClient.Credentials = New-Object System.Net.NetworkCredential($username, $password)

process_log "remoteFile: $remoteFile"

foreach ($f in $remoteFile) {
    try {
        download_file($f)
    }
    catch {
        process_log "!!!<$f>: $($_.Exception.Message)"
    }
}

process_log  "======Download finished======"
# 釋放 WebClient 資源
$webClient.Dispose()
return 0
