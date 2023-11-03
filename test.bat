@REM powershell start-process c:\Users\edison\Downloads\NewRepo-master\TestManager\TestManager\bin\Debug\TestManager.exe -verb RunAs
powershell set-executionpolicy Bypass -scope CurrentUser
powershell set-executionpolicy Bypass -scope process
@REM powershell start-process c:\TestManager\TestManager\TestManager\bin\Debug\TestManager.exe -WindowStyle Hidden -verb RunAs
powershell start-process c:\TestManager\TestManager\TestManager\bin\Debug\TestManager.exe -verb RunAs
exit