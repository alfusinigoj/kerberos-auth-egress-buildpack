$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
& $scriptDir\buildpack.exe "release" $args
exit $LASTEXITCODE