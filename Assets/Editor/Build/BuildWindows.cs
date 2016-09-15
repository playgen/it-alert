using UnityEngine;
using System.Collections;
using UnityEditor;

public class BuildWebGL : MonoBehaviour {

	[MenuItem("Build/Windows Dev Build")]
	static void Build()
	{
		string[] scenes = { "Assets/Scenes/Network.unity" };
		BuildPipeline.BuildPlayer(scenes, @"Build/Windows", BuildTarget.StandaloneWindows64, BuildOptions.None);
	}

}
