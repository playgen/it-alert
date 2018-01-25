using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Utilities
{
	public class ButtonList
	{
		public GameObject[] Buttons { get; }

		public ButtonList(string menuPath)
		{
			Buttons = GameObjectUtilities.FindAllChildren(menuPath);
		}

        public Button GetButton(string containerName)
		{
			var button = Buttons.First(o => o.name.Equals(containerName));
			return button.GetComponentInChildren<Button>();
		}
	}
}