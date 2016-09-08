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
		var loginDetails = _loginPanel.GetComponent<LoginPanelBehaviour>().GetLoginDetails();
		EnqueueCommand(new LoginCommand(loginDetails.username, loginDetails.password));
	}

	public override void Initialize()
	{
		_loginPanel = GameObject.Find("LoginContainer").transform.GetChild(0).gameObject;

		var buttons = new ButtonList("LoginContainer/LoginPanelContainer/LoginPanel/ButtonPanel");
		var loginButton = buttons.GetButton("LoginButtonContainer");
		//var registerButton = buttons.First(o => o.name.Equals("RegisterButton")).GetComponent<Button>();
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

	public void OnLoginFailed(string msg)
	{
		_loginPanel.GetComponent<LoginPanelBehaviour>().SetStatusText(msg);
	}
}
