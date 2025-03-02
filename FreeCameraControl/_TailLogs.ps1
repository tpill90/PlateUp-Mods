$modName = "CameraPlus"
$logFile = "$env:USERPROFILE\AppData\LocalLow\It's Happening\PlateUp\Player.log"
Get-Content -Path $logFile -Wait | Where-Object { $_ -match $modName }