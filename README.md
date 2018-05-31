Make sure that Submodules have been pulled properly. Source Tree likes to ignore the Newtonsoft.Json Submodule

# Development
## Run Instructions
1. Build Simulation/Engine/Ephermis.ECS-Alpha.sln
2. Build Simulation/PlayGen.ITAlert.sln
3. Run Server/PlayGen.ITAlert.Photon.sln (Make sure PlayGen.ITAlert.Photon.Plugin is set as startup project)
4. Build Unity/PlayGen.ITAlert.Unity/PlayGen.ITAlert.Unity.sln
5. Open and run Unity/

## SUGAR
### Build Instructions
Currently using the unity client built from commit: 65b3f05407eb5f58096c1ae862da18ab8d643e1c

1. Build SUGAR-Unity
2. Copy build output of SUGAR-Unity build to lib/SUGAR/bin

OR

1. Build the SUGAR-Unity unitypackage
2. Import into IT Alert

# Deployment
## Run Instructions
To run the Photon Server as a service, see this [ReadMe](Server/deploy/ReadMe.md).