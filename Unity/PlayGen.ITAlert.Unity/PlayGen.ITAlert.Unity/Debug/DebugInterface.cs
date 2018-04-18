using UnityEngine;

namespace PlayGen.ITAlert.Unity.Debug
{
	public class DebugInterface : MonoBehaviour
	{
		private bool _showInterface;

		private void Update()
		{
			if (Application.isEditor)
			{
				if (Input.GetKeyDown(KeyCode.F5))
				{
					_showInterface = !_showInterface;
				}
			}
		}

		private void OnGUI()
		{
			if (_showInterface)
			{
				if (GUI.Button(new Rect(0, 0, 150, 20), "Increase Speed"))
				{
					DebugCommands.ChangePlayerSpeedOffset(0.1m);
				}
				if (GUI.Button(new Rect(0, 25, 150, 20), "Decrease Speed"))
				{
					DebugCommands.ChangePlayerSpeedOffset(-0.1m);
				}
			}
		}
	}
}
