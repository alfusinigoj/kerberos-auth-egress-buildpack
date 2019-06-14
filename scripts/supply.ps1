$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
& $scriptDir\buildpack.exe "supply" $args
exit $LASTEXITCODE