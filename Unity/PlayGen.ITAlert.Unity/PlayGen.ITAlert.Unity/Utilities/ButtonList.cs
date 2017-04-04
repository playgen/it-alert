using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Utilities
{
	public class ButtonList
	{
		private readonly GameObject[] _buttons;

        public GameObject[] Buttons
        {
            get { return _buttons; }
        }

		public ButtonList(string menuPath)
		{
			_buttons = GameObjectUtilities.FindAllChildren(menuPath);
		}

        public Button GetButton(string containerName)
		{
			var button = _buttons.First(o => o.name.Equals(containerName));
			return button.GetComponentInChildren<Button>();
		}
	}
}