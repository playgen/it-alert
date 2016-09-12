using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
	private ButtonList _buttons;

	private GameObject _buttonContainer1;
	private GameObject _buttonContainer2;
	private GameObject _buttonContainer3;

	private Text _title;

	/// <summary>
	/// Set the title and buttons of the popup containier, currently supports 1-3 buttons
	/// </summary>
	/// <param name="title">The title of the popup</param>
	/// <param name="outputs">The desired button outputs from left to right</param
	public void SetPopup(string title, Outputs[] outputs)
	{
		//set the title
		SetTitle(title);

		//set the button container to be active
		FindButtonContainers();

		switch (outputs.Length)
		{
			case 1:
				SetActiveContainer(1);
				_buttons = new ButtonList("PopupContainer/PopupPanelContainer/ButtonContainer/1ButtonContainer");

				SetButton(_buttons.GetButton("ButtonContainer"), outputs[0].Name, outputs[0].Action);

				break;
			case 2:
				SetActiveContainer(2);
				_buttons = new ButtonList("PopupContainer/PopupPanelContainer/ButtonContainer/2ButtonContainer");

				SetButton(_buttons.GetButton("LeftButtonContainer"), outputs[0].Name, outputs[0].Action);
				SetButton(_buttons.GetButton("RightButtonContainer"), outputs[1].Name, outputs[1].Action);

				break;
			case 3:
				SetActiveContainer(3);
				_buttons = new ButtonList("PopupContainer/PopupPanelContainer/ButtonContainer/3ButtonContainer");

				SetButton(_buttons.GetButton("LeftButtonContainer"), outputs[0].Name, outputs[0].Action);
				SetButton(_buttons.GetButton("MiddleButtonContainer"), outputs[1].Name, outputs[1].Action);
				SetButton(_buttons.GetButton("RightButtonContainer"), outputs[2].Name, outputs[2].Action);

				break;
			default:
				Debug.LogError(string.Format("Unsuported length of outputs array for popup box.\nSupported Range: {0}		Provided Outputs: {1}", "1-3", outputs.Length));
				//not supported length of popup
				break;
		}
	}

	private void SetTitle(string title)
	{
		_title = GameObjectUtilities.Find("PopupContainer/PopupPanelContainer/TitleContainer/Title").GetComponent<Text>();
		_title.text = title;
	}

	private void FindButtonContainers()
	{
		_buttonContainer1 = GameObjectUtilities.Find("PopupContainer/PopupPanelContainer/ButtonContainer/1ButtonContainer").gameObject;
		_buttonContainer2 = GameObjectUtilities.Find("PopupContainer/PopupPanelContainer/ButtonContainer/2ButtonContainer").gameObject;
		_buttonContainer3 = GameObjectUtilities.Find("PopupContainer/PopupPanelContainer/ButtonContainer/3ButtonContainer").gameObject;
	}

	private void SetButton(Button button, string text, UnityAction action)
	{
		button.onClick.AddListener(action);
		button.gameObject.GetComponent<Text>().text = text;
	}

	private void SetActiveContainer(int num)
	{
		//ensure num is the is between 1 and 3, this shouldnt be needed
		num = Mathf.Clamp(num, 1, 3);

		// make all object inactive
		_buttonContainer1.gameObject.SetActive(false);
		_buttonContainer2.gameObject.SetActive(false);
		_buttonContainer3.gameObject.SetActive(false);

		switch (num)
		{
			case 1:
				_buttonContainer1.gameObject.SetActive(true);
				return;
			case 2:
				_buttonContainer2.gameObject.SetActive(true);
				return;
			case 3:
				_buttonContainer3.gameObject.SetActive(true);
				return;
		}
	}

	public class Outputs
	{
		public string Name;
		public UnityAction Action;

		public Outputs(string name, UnityAction action)
		{
			Name = name;
			Action = action;
		}
	}
}
