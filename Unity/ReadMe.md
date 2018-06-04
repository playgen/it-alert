# Configuration
In Unity/Assets/StreamingAssets you can find configurations for the following:

## Photon Client
The Unity/Assets/StreamingAssets/Photon.config file is loaded at startup and used to configure the [Photon Unity Client](https://www.photonengine.com/en-us/PUN).

## SUGAR Client
In the ITAlert.unity scene, you can configure the SUGAR client by selecting the SUGAR game object in the scene hierarchy view.

The SugarUnityManager component has the various configurable settings including the SUGAR server url to use.

For more info on the SUGAR Unity Client see [here](http://api.sugarengine.org/v1/unity-client/index.html).

# Build
## Android
At the time of writing, builds need to be done using the Unity Build Setting "Internal" rather than "Gradle".