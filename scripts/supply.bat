@echo off
powershell.exe -ExecutionPolicy Unrestricted %~dp0\supply.ps1 %*
EXIT /B %ERRORLEVEL%