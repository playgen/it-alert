using System;
using UnityEngine;
using System.Collections.Generic;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Client;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class PopupController
	{
		private readonly GameObject _popupPanel;
		private readonly PopupBehaviour _popupBehaviour;
		private readonly ITAlertPhotonClient _photonClient;

		private ColourPickerBehaviour _colourPickerBehaviour;

		public PopupController(ITAlertPhotonClient photonClient)
		{
			_popupPanel = GameObject.Find("PopupContainer").transform.GetChild(0).gameObject;
			_popupBehaviour = _popupPanel.GetComponent<PopupBehaviour>();
			_photonClient = photonClient;
		}

		//public void ShowErrorPopup(string msg)
		//{
		//	// Show error on popup
		//	var errorPanel = UnityEngine.Object.Instantiate(Resources.Load("ErrorContentPanel")) as GameObject;
		//	var errorMsg = errorPanel.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
		//	errorMsg.text = msg;

		//	_popupBehaviour.ClearContent();
		//	_popupBehaviour.SetPopup(Localization.Get("ERROR_LABEL_TITLE"), new[] {new PopupBehaviour.Output(Localization.Get("ERROR_BUTTON_CLOSE"), null)}, PopupClosed);
		//	_popupBehaviour.SetContent(errorPanel.GetComponent<RectTransform>());

		//	_popupPanel.gameObject.SetActive(true);
		//}

		//public void ShowLoadingPopup( /*UnityAction cancelAction = null*/)
		//{
		//	// Show the loading popup along with a button to cancel
		//	var loadingPanel = UnityEngine.Object.Instantiate(Resources.Load("LoadingContentPanel")) as GameObject;

		//	_popupBehaviour.ClearContent();
		//	_popupBehaviour.SetPopup("", null, PopupClosed);
		//	_popupBehaviour.SetContent(loadingPanel.GetComponent<RectTransform>());

		//	_popupPanel.gameObject.SetActive(true);
		//}

		//public void HideLoadingPopup()
		//{
		//	PopupClosed();
		//}

		public void ShowColorPickerPopup(Action<PlayerColour> callback, IEnumerable<ITAlertPlayer> players, ITAlertPlayer currentPlayer)
		{
			var colorPanel = UnityEngine.Object.Instantiate(Resources.Load("ColorPickerContentPanel")) as GameObject;
			if (colorPanel != null)
			{
				colorPanel.name = "ColorPickerContentPanel";
				_popupBehaviour.ClearContent();

				_colourPickerBehaviour = colorPanel.GetComponent<ColourPickerBehaviour>();
				_colourPickerBehaviour.GenerateColorPicker(players, currentPlayer);

				_photonClient.CurrentRoom.PlayerListUpdatedEvent += _colourPickerBehaviour.UpdateSelectedGlyphs;
				_popupBehaviour.SetPopup(Localization.Get("COLOUR_PICKER_LABEL_TITLE"),
					new[]
					{
						new PopupBehaviour.Output(Localization.Get("COLOUR_PICKER_BUTTON_CANCEL"), null),
						new PopupBehaviour.Output(Localization.Get("COLOUR_PICKER_BUTTON_SELECT"), () => ColorCallback(callback))
					},
					PopupClosed
				);
				_popupBehaviour.SetContent(colorPanel.GetComponent<RectTransform>());
			}

			_popupPanel.gameObject.SetActive(true);
		}

		private void ColorCallback(Action<PlayerColour> callback)
		{
			//get the selected color from the color picker behaviour
			var playerColour = GameObject.Find("ColorPickerContentPanel").GetComponent<ColourPickerBehaviour>().GetPlayerColour();
			callback(playerColour);
		}

		private void PopupClosed()
		{
			_photonClient.CurrentRoom.PlayerListUpdatedEvent -= _colourPickerBehaviour.UpdateSelectedGlyphs;
			_popupPanel.gameObject.SetActive(false);
		}
	}
}