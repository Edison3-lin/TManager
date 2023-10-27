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

#配置資訊
$Database   = 'SIT_TEST'
$Server     = '"172.0.0.9"'
$UserName   = 'Captain001'
$Password   = 'Captaintest2023@SIT'

#建立連線對像
$SqlConn = New-Object System.Data.SqlClient.SqlConnection
 
#使用賬號連線MSSQL
$SqlConn.ConnectionString = "Data Source=$Server;Initial Catalog=$Database;user id=$UserName;pwd=$Password"
 
#打開數據庫連線
try {
    $SqlConn.open()
}
catch {
    process_log "!!!<Exception>: $($_.Exception.Message)"
    return "Unconnected_"
}

$SqlCmd = New-Object System.Data.SqlClient.SqlCommand
$SqlCmd.connection = $SqlConn

$UUID = Get-WmiObject Win32_ComputerSystemProduct | Select-Object -ExpandProperty UUID 
Write-Host $UUID
# Read SQL data
$sqlCmd.CommandText = "SELECT [DP_ID],[DP_UUID],[DP_Status] FROM [SIT_TEST].[DBO].[DUT_Profile]"
$adapter = New-Object System.Data.SqlClient.SqlDataAdapter $sqlCmd
$dataset = New-Object System.Data.DataSet
$NULL = $adapter.Fill($dataSet)
for ($i=0; $i -lt $dataSet.Tables[0].Rows.Count; $i++)
{
    $DP_ID= $dataSet.Tables[0].Rows[$i][0]
    $DP_UUID= $dataSet.Tables[0].Rows[$i][1]
    $DP_Status= $dataSet.Tables[0].Rows[$i][2]

    if( ($DP_UUID -eq $UUID) -and ($DP_Status -eq 'Process_') )
    {
        $SqlCmd.commandtext = "update DUT_Profile set DP_Status='Completed_' where DP_ID="+"'$DP_ID'"
        $NULL = $SqlCmd.executenonquery()
        $SqlCmd.commandtext = 'update DUT_Profile set DP_Create_Datetime=CONVERT (datetime, SYSDATETIME())  where DP_ID='+"'$DP_ID'"
        $NULL = $SqlCmd.executenonquery()
        break
    }
}

#關閉數據庫連線
$SqlConn.close()
return 0