Set-StrictMode -Version Latest
function Invoke-Administrator([String] $FilePath, [String[]] $ArgumentList = '') 
{
  $Current = [Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()
  $Administrator = [Security.Principal.WindowsBuiltInRole]::Administrator
  if (-not $Current.IsInRole($Administrator)) 
  {
    $PowerShellPath = (Get-Process -Id $PID).Path
    $Command = "" + $FilePath + "$ArgumentList" + ""
    Start-Process $PowerShellPath "-NoProfile -ExecutionPolicy Bypass -File $Command" -Verb RunAs
    exit
  } 
  else 
  {
    Set-ExecutionPolicy -Scope Process -ExecutionPolicy ByPass
  }
}

Invoke-Administrator $PSCommandPath
