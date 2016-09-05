using System;
using System.Collections;
using System.Linq;
using GameWork.Interfacing;
using UnityEngine;
using UnityEngine.UI;

public class LoginStateInterface : StateInterface
{

	private GameObject _loginPanel;

	private void OnLoginClick()
	{
		Debug.Log("WOOTJSAKDJ");
		EnqueueCommand(new LoginCommand());
	}

	public override void Initialize()
	{
		_loginPanel = GameObject.Find("LoginContainer").transform.GetChild(0).gameObject;
		var buttons = GameObjectUtilities.FindAllChildren("LoginContainer/LoginPanel/LoginFieldsPanel/ButtonPanel");

		var loginButton = buttons.First(o => o.name.Equals("LoginButton")).GetComponent<Button>();
		var registerButton = buttons.First(o => o.name.Equals("RegisterButton")).GetComponent<Button>();
		loginButton.onClick.AddListener(OnLoginClick);
	}


	public override void Enter()
	{
		_loginPanel.SetActive(true);
	}

	public override void Exit()
	{
		_loginPanel.GetComponent<LoginPanelBehaviour>().ResetFields();
		_loginPanel.SetActive(false);
	}
}
