using System;
using UnityEngine;
using System.Collections.Generic;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;
using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class PopupController
	{
		private readonly GameObject _popupPanel;
		private readonly PopupBehaviour _popupBehaviour;

		public PopupController()
		{
			_popupPanel = GameObject.Find("PopupContainer").transform.GetChild(0).gameObject;
			_popupBehaviour = _popupPanel.GetComponent<PopupBehaviour>();
			PopupUtility.PopupController = this;
		}

		public void ShowErrorPopup(string msg)
		{
			// Show error on popup
			var errorPanel = Object.Instantiate(Resources.Load("ErrorContentPanel")) as GameObject;
			var errorMsg = errorPanel.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
			errorMsg.text = msg;

			_popupBehaviour.ClearContent();
			_popupBehaviour.SetPopup("Error", new[] {new PopupBehaviour.Output("OK", null)}, PopupClosed);
			_popupBehaviour.SetContent(errorPanel.GetComponent<RectTransform>());

			_popupPanel.gameObject.SetActive(true);
		}

		public void ShowLoadingPopup( /*UnityAction cancelAction = null*/)
		{
			// Show the loading popup along with a button to cancel
			var loadingPanel = Object.Instantiate(Resources.Load("LoadingContentPanel")) as GameObject;

			_popupBehaviour.ClearContent();
			_popupBehaviour.SetPopup("", null, PopupClosed);
			_popupBehaviour.SetContent(loadingPanel.GetComponent<RectTransform>());

			_popupPanel.gameObject.SetActive(true);
		}

		public void HideLoadingPopup()
		{
			PopupClosed();
		}

		public void ShowColorPickerPopup(Action<Color> callback, List<Color> selectedColors)
		{
			var colorPanel = Object.Instantiate(Resources.Load("ColorPickerContentPanel")) as GameObject;
			colorPanel.name = "ColorPickerContentPanel";
			_popupBehaviour.ClearContent();
			colorPanel.GetComponent<ColorPickerBehaviour>().GenerateColorPicker(selectedColors);
			_popupBehaviour.SetPopup("Change Player Colour",
				new[]
				{
					new PopupBehaviour.Output("Cancel", null),
					new PopupBehaviour.Output("Set Colour", () => ColorCallback(callback))
				},
				PopupClosed
			);
			_popupBehaviour.SetContent(colorPanel.GetComponent<RectTransform>());

			_popupPanel.gameObject.SetActive(true);
		}

		private void ColorCallback(Action<Color> callback)
		{
			//get the selected color from the color picker behaviour
			var color = GameObject.Find("ColorPickerContentPanel").GetComponent<ColorPickerBehaviour>().GetColor();
			callback(color);
		}

		private void PopupClosed()
		{
			_popupPanel.gameObject.SetActive(false);
		}
	}
}