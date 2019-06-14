@echo off
powershell.exe -ExecutionPolicy Unrestricted %~dp0\release.ps1 %*
EXIT /B %ERRORLEVEL%