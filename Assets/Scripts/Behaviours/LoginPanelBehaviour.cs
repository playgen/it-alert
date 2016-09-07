using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class LoginPanelBehaviour : MonoBehaviour
{
    public InputField UsernameInputField;
	public InputField PasswordInputField;
	public Text StatusText;

	public void ResetFields()
	{
		UsernameInputField.text = "";
		PasswordInputField.text = "";
	    StatusText.text = "";
	}

	public LoginDetails GetLoginDetails()
	{
		return new LoginDetails(UsernameInputField.text, PasswordInputField.text);
	}

    public void SetStatusText(string status)
    {
        StatusText.text = status;
    }

	public struct LoginDetails
	{
		public string username;
		public string password;

		public LoginDetails(string username, string password)
		{
			this.username = username;
			this.password = password;
		}
	}
}
