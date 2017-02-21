using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Utilities
{
	public class ButtonList
	{
		private readonly GameObject[] _buttons;

		public ButtonList(string menuPath)
		{
			_buttons = GameObjectUtilities.FindAllChildren(menuPath);
		}

		public void BestFit()
		{
			if (_buttons != null && _buttons.Length > 0)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) _buttons[0].transform.parent);
			}
			int smallestFontSize = 1;
			foreach (var textObj in _buttons)
			{
				var text = textObj.GetComponentInChildren<Text>();
				if (!text)
				{
					continue;
				}
				text.resizeTextForBestFit = true;
				text.resizeTextMinSize = 1;
				text.resizeTextMaxSize = 48;
				text.cachedTextGenerator.Invalidate();
				text.cachedTextGenerator.Populate(text.text, text.GetGenerationSettings(text.rectTransform.rect.size));
				text.resizeTextForBestFit = false;
				var newSize = text.cachedTextGenerator.fontSizeUsedForBestFit;
				var newSizeRescale = text.rectTransform.rect.size.x / text.cachedTextGenerator.rectExtents.size.x;
				if (text.rectTransform.rect.size.y / text.cachedTextGenerator.rectExtents.size.y < newSizeRescale)
				{
					newSizeRescale = text.rectTransform.rect.size.y / text.cachedTextGenerator.rectExtents.size.y;
				}
				newSize = Mathf.FloorToInt(newSize * newSizeRescale);
				if (newSize < smallestFontSize || smallestFontSize == 1)
				{
					smallestFontSize = newSize;
				}
			}
			foreach (var textObj in _buttons)
			{
				var text = textObj.GetComponentInChildren<Text>();
				if (!text)
				{
					continue;
				}
				text.fontSize = smallestFontSize;
			}
		}

		public Button GetButton(string containerName)
		{
			var button = _buttons.First(o => o.name.Equals(containerName));
			return button.GetComponentInChildren<Button>();
		}
	}
}