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

		public static PopupController PopupController;

		public static void LogError(string message)
		{
			if (LogErrorEvent != null)
			{
				LogErrorEvent(message);
			}
		}

		public static void ShowLoadingPopup()
		{
			if (StartLoadingEvent != null)
			{
				StartLoadingEvent();
			}
		}

		public static void HideLoadingPopup()
		{
			if (EndLoadingEvent != null)
			{
				EndLoadingEvent();
			}
		}

		public static void ShowColorPicker(Action<Color> callbackAction, List<Color> selectedColors)
		{
			//get the controller
			PopupController.ShowColorPickerPopup(callbackAction, selectedColors);
		}
	}
}