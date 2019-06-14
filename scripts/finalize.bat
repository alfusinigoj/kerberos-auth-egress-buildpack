@echo off
powershell.exe -ExecutionPolicy Unrestricted %~dp0\finalize.ps1 %*
EXIT /B %ERRORLEVEL%