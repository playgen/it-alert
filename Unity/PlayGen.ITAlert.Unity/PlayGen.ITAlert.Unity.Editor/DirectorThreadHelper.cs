using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Unity.Simulation;
using UnityEditor;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Editor
{
	public class DirectorThreadHelper : MonoBehaviour
	{
		[SerializeField]
		private Director _director;

		public void Start()
		{
#if UNITY_EDITOR
			LogProxy.Info("Ading callback for editor state change");
			EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
#endif
		}

		private void PlaymodeStateChanged()
		{
			var unpausedAndPlaying = EditorApplication.isPlayingOrWillChangePlaymode;
			var paused = EditorApplication.isPaused;

			if (!unpausedAndPlaying && !paused)
			{
				_director.StopWorker();
			}
		}
	}
}
