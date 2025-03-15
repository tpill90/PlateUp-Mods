# Getting PlateUp install dir
[xml]$parsedProps = Get-Content Environment.props
$plateupInstallDir = ([string]$parsedProps.Project.PropertyGroup.PlateUpInstallDir).Trim()
$logFile = "$plateupInstallDir\PlateUp\BepInEx\LogOutput.log"

Get-Content -Path $logFile -Wait | ForEach-Object {
    # Filtering out application logs which just clutter the log output
    if (($_ -match "\[APPLICATION\]") -OR ($_ -match "\[HUEY-CORE\]"))
    {
        # Do nothing, don't want to write logs here
        return
    }
    # Colorizing log output to match what the bepinex console does
    if ($_ -match "Warning")
    {
        Write-Host $_ -ForegroundColor Yellow
    }
    elseif ($_ -match "Error")
    {
        Write-Host $_ -ForegroundColor Red
    }
    elseif ($_ -match "Info")
    {
        Write-Host $_ -ForegroundColor DarkGray
    }
    elseif ($_ -match "Message")
    {
        Write-host $_ -ForegroundColor White
    }
    else
    {
        Write-Host $_
    }
}