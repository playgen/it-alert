using GameWork.Commands.Accounts;
using GameWork.Interfacing;
using UnityEngine;

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
		loginButton.onClick.AddListener(OnLoginClick);
		var registerButton = buttons.GetButton("RegisterButtonContainer");
		registerButton.onClick.AddListener(OnRegisterClick);
	}

	private void OnRegisterClick()
	{
		var loginDetails = _loginPanel.GetComponent<LoginPanelBehaviour>().GetLoginDetails();
		EnqueueCommand(new RegisterCommand(loginDetails.username, loginDetails.password));
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

	public void OnRegisterSucceeded()
	{
		_loginPanel.GetComponent<LoginPanelBehaviour>().SetStatusText("Register Succeeded, Please Login");
	}
}
