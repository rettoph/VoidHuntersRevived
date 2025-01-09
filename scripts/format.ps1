Set-Location "$($PSScriptRoot)/..";
dotnet format -v diag --severity info --exclude libraries --exclude-diagnostics IDE0130