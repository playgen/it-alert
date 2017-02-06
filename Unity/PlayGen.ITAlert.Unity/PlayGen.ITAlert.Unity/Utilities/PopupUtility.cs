using System;
using UnityEngine;
using System.Collections.Generic;
using PlayGen.ITAlert.Unity.Controllers;

namespace PlayGen.ITAlert.Unity.Utilities
{
	public static class PopupUtility
	{
		public static event Action<string> LogErrorEvent;
		public static event Action StartLoadingEvent;
		public static event Action EndLoadingEvent;
		public static event Action<Color> ColorPickerEvent;

		public static readonly PopupController PopupController = new PopupController();

		public static void LogError(string message)
		{
			LogErrorEvent?.Invoke(message);
		}

		public static void ShowLoadingPopup()
		{
			StartLoadingEvent?.Invoke();
		}

		public static void HideLoadingPopup()
		{
			EndLoadingEvent?.Invoke();
		}

		public static void ShowColorPicker(Action<Color> callbackAction, List<Color> selectedColors)
		{
			//get the controller
			PopupController.ShowColorPickerPopup(callbackAction, selectedColors);
		}
	}
}