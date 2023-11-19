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

################## Main ##############################
$Parameter = '/sleep /c:1 /s:3 /d:30 /p:40'
try {
    process_log $PSCommandPath" is executing..."    
    $process = Start-Process -FilePath "C:\\TestManager\\\CommonLibs\\pwrtest.exe" -ArgumentList $Parameter -Wait -PassThru -RedirectStandardOutput $tempfile
    $process.WaitForExit()
    result_log($tempfile)

    if ($process.ExitCode -eq 0) {
        process_log "Process execution is finished!"
    } else {
        $Exit_Error = $process.ExitCode.ToString()
        process_log "!!!<Error code>: $Exit_Error"
    }
} 
catch {
    process_log "!!!<Exception>: $($_.Exception.Message)"
} 
finally {
    Remove-Item $tempfile
}