# Note ADB in the android sdk platform tools folder needs to be available in the cmd console. 
# To do this: add the platform tools folder to your system path environment variable.

# Install and -r to replace previous version
adb install -r ITAlert.apk

# launch app bundle and activity name which is found in the android manifest (Temp/StagingArea)
adb shell am start -n com.PlayGen.ITAlert/com.unity3d.player.UnityPlayerActivity

PAUSE