using System;
using GameWork.Core.States.Tick.Input;
using GameWork.Unity.Engine.Components.Utilities;
using GameWork.Unity.Engine.Transform.Utilities;

using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Loading;
using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.ITAlert.Unity.States.Error
{
	public class ErrorStateInput : TickStateInput
	{

		public event Action BackClickedEvent;

		private ButtonList _buttons;
		private Button _backButton;
		private Transform _panelVisibility;
		private InputField _messageText;
		
		protected override void OnInitialize()
		{
			var panel = TransformFinder.Find("Menu/ErrorContainer");
			_panelVisibility = panel.GetChild(0);

			_buttons = new ButtonList("ErrorContainer/ErrorPanelContainer/ButtonPanel");

			_messageText = ComponentFinder.Find<InputField>("ErrorPanel/Scroll View/Viewport/Content", _panelVisibility);

			_backButton = _buttons.GetButton("BackButtonContainer");
			_backButton.onClick.AddListener(OnBackButtonClick);
		}
		
		protected override void OnEnter()
		{
			try
			{
				var exceptionString = GameExceptionHandler.Exception?.ToString();
				_messageText.text = exceptionString?.Substring(0, Math.Min(exceptionString.Length, 3000)) ?? Localization.Get("SERVER_DISCONNECT");
				_panelVisibility.gameObject.SetActive(true);
				_buttons.Buttons.BestFit();

				Loading.Stop();
				GameExceptionHandler.ClearException();
			}
			catch (Exception ex)
			{ 
				LogProxy.Error($"Error entering ErrorStateInput: {ex}");
			}
		}

		protected override void OnExit()
		{
			_panelVisibility.gameObject.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnBackButtonClick();
			}
		}

		private void OnBackButtonClick()
		{
			BackClickedEvent?.Invoke();
		}
	}
}