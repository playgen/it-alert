using System.Diagnostics;
using GameWork.Unity.Editor.Build;
using UnityEditor;

public static class BuildLaunchAndPlay
{
	[MenuItem("Tools/Build, Launch and Play/1")]
	public static void BuildPlayAndLaunch1()
	{
		BuildPlayAndLaunch(1);
	}
	
	[MenuItem("Tools/Build, Launch and Play/2")]
	public static void BuildPlayAndLaunch2()
	{
		BuildPlayAndLaunch(2);
	}

	[MenuItem("Tools/Build, Launch and Play/3")]
	public static void BuildPlayAndLaunch3()
	{
		BuildPlayAndLaunch(3);
	}

	[MenuItem("Tools/Build, Launch and Play/4")]
	public static void BuildPlayAndLaunch4()
	{
		BuildPlayAndLaunch(4);
	}

	public static void BuildPlayAndLaunch(int count)
	{
		var buildPath = Builder.Build();
		LaunchBuilds(buildPath, count);
		EditorApplication.isPlaying = true;
	}

	public static void LaunchBuilds(string buildPath, int count)
	{
		for (var i = 1; i <= count; i++)
		{
			var startInfo = new ProcessStartInfo()
			{
				FileName = buildPath,
				Arguments = "-a -u ITAlert_User" + i + " -p t0pSECr3t -s SUGAR",
				UseShellExecute = false,
			};

			Process.Start(startInfo);
		}
	}
}
