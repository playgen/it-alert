# Running the Server
## Configuring the IP
Make sure to set the correct IP for the host in the Photon.LoadBalancing.dll.config file.

In the file: 
1. Find and set "MasterIPAddress" to the server's IP.
2. Find "PublicIPAddress". If there is a comment saying you can leave it blank for auto-lookup, do so otherwise set it to the server's IP.

## As a Service:
1. Remove previous  
1.1. Check if Photon Control is running by checking the system tray  
1.2. Open it, go to LoadBallancing/Remove service.  
1.3. Quit Photon Control
2. Replace the contents of deploy/
3. Run deploy/bin_Win64/PhotonControl.exe
4. In Photon Control  
4.1. LoadBallancing/Install service    
4.2. LoadBallancing/Start service  
5. In Windows's "Services" manager:  
5.1. Locate the "Photon Socket Server. LoadBalancing" service.  
5.2. Right Click and select "Properties".    
5.3. Select the "Recovery" tab and set all failure options to "Restart the service".  
5.4. Click "Apply".  
