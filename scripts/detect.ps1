$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
& $scriptDir\buildpack.exe "detect" $args
exit $LASTEXITCODE