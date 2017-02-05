using System;
using GameWork.Core.States.Tick.Input;
using GameWork.Unity.Engine.Components.Utilities;
using GameWork.Unity.Engine.Transform.Utilities;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.States.Error
{
	public class ErrorStateInput : TickStateInput
	{
		private readonly GameErrorContainer _gameErrorContainer;

		public event Action BackClickedEvent;

		private ButtonList _buttons;
		private Button _backButton;
		private Transform _panelVisibility;
		private Text _messageText;

		public ErrorStateInput(GameErrorContainer gameGameErrorContainer)
		{
			_gameErrorContainer = gameGameErrorContainer;
		}

		protected override void OnInitialize()
		{
			var panel = TransformFinder.Find("Menu/ErrorContainer");
			_panelVisibility = panel.GetChild(0);

			_buttons = new ButtonList("ErrorContainer/ErrorPanelContainer/ButtonPanel");

			_messageText = ComponentFinder.Find<Text>("ErrorPanel/Message", _panelVisibility);

			_backButton = _buttons.GetButton("BackButtonContainer");
			_backButton.onClick.AddListener(OnBackButtonClick);
		}
		
		protected override void OnEnter()
		{
			_messageText.text = _gameErrorContainer?.Exception?.Message;
			_panelVisibility.gameObject?.SetActive(true);
			_buttons.BestFit();
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