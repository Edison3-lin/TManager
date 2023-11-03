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
$P = '/password "6315" '
$A = $P+'/get "VMD Controller"'
$B = $P+'/set "VMD Controller=1"'
$C = $P+'/set "VMD Controller=0"'
$T1 = $P+'/get "Touchpad"'
$T2 = $P+'/set "Touchpad=1"'
$s = ' /sleep /help'
try {
    process_log $PSCommandPath
    $process = Start-Process -FilePath "C:\TestManager\Common\abst64.exe" -ArgumentList $A -NoNewWindow -Wait -PassThru -RedirectStandardOutput $tempfile
    $process.WaitForExit()
    result_log (Get-Content $tempfile)

    if ($process.ExitCode -eq 0) {
        process_log "Process execution is finished!"
    } else {
        $Exit_Error = $process.ExitCode.ToString()
        process_log "!!!<Error code>: $Exit_Error"
    }
} catch {
    process_log "!!!<Exception>: $($_.Exception.Message)"
}

