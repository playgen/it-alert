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

	public void SetPopup(string title, string middleButtonText, UnityAction middleButtonClick)
	{
		//set the title
		SetTitle(title);

		//set the button container to be active
		SetButtonContainers();

		SetActiveContainer(1);

		_buttons = new ButtonList("PopupPanelContainer/ButtonContainer/1ButtonContainer");

		SetButton(_buttons.GetButton("ButtonContainer"), middleButtonText, middleButtonClick);
	}

	public void SetPopup(string title, string leftButtonText, UnityAction leftButtonClick, string rightButtonText, UnityAction rightButtonClick)
	{
		//set the title
		SetTitle(title);

		//set the button container to be active
		SetButtonContainers();

		SetActiveContainer(2);

		_buttons = new ButtonList("PopupPanelContainer/ButtonContainer/2ButtonContainer");

		SetButton(_buttons.GetButton("LeftButtonContainer"), leftButtonText, leftButtonClick);
		SetButton(_buttons.GetButton("RightButtonContainer"), rightButtonText, rightButtonClick);
	}

	public void SetPopup(string title, string leftButtonText, UnityAction leftButtonClick, string middleButtonText, UnityAction middleButtonClick, string rightButtonText, UnityAction rightButtonClick)
	{
		//set the title
		SetTitle(title);

		//set the button container to be active
		SetButtonContainers();

		SetActiveContainer(3);

		_buttons = new ButtonList("PopupPanelContainer/ButtonContainer/3ButtonContainer");

		SetButton(_buttons.GetButton("LeftButtonContainer"), leftButtonText, leftButtonClick);
		SetButton(_buttons.GetButton("MiddleButtonContainer"), middleButtonText, middleButtonClick);
		SetButton(_buttons.GetButton("RightButtonContainer"), rightButtonText, rightButtonClick);
	}

	private void SetTitle(string title)
	{
		_title = GameObjectUtilities.Find("TitleContainer/Title").GetComponent<Text>();
		_title.text = title;
	}

	private void SetButtonContainers()
	{
		_buttonContainer1 = GameObjectUtilities.Find("PopupPanelContainer/ButtonContainer/1ButtonContainer").gameObject;
		_buttonContainer2 = GameObjectUtilities.Find("PopupPanelContainer/ButtonContainer/2ButtonContainer").gameObject;
		_buttonContainer3 = GameObjectUtilities.Find("PopupPanelContainer/ButtonContainer/3ButtonContainer").gameObject;
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

#if UNITY_EDITOR
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SetPopup("1 Button", "Middle", TextOutput);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SetPopup("2 Button", "Left", TextOutput, "Right", TextOutput);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SetPopup("3 Button", "Left", TextOutput, "Middle", TextOutput, "Right", TextOutput);
		}
	}

	void TextOutput()
	{
		Debug.Log(Time.time);
	}
#endif
}
