$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
& $scriptDir\buildpack.exe "finalize" $args
exit $LASTEXITCODE