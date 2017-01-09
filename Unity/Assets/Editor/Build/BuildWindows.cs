using UnityEngine;
using UnityEditor;

public class BuildWebGL : MonoBehaviour {

	[MenuItem("Build/Windows Dev Build")]
	static void Build()
	{
		string[] scenes = { "Assets/Scenes/IT Alert.unity" };
		BuildPipeline.BuildPlayer(scenes, @"Build/Windows.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
	}

}
