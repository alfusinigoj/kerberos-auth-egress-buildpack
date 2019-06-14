@echo off
powershell.exe -ExecutionPolicy Unrestricted %~dp0\detect.ps1 %*
EXIT /B %ERRORLEVEL%