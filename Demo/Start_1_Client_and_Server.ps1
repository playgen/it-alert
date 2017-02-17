# Must be run with the folder it is located in as the active directory

# Kill old instances
Stop-Process -name "IT Alert!_StandaloneWindows64" 2>$null
Stop-Process -name "PhotonSocketServer" 2>$null

# Spawn new instances
& ".\Client\IT Alert!_StandaloneWindows64.exe" -a -u ITAlert_User1 -p t0pSECr3t -s SUGAR

Write-Output "Launched Clients"

Push-Location .\Server\bin_Win64
& .\PhotonSocketServer.exe /run LoadBalancing
Pop-Location

Write-Output "Launched Server"

Write-Output "Done with the launching"