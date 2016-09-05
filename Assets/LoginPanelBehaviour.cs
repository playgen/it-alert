using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class LoginPanelBehaviour : MonoBehaviour
{

	public InputField UsernameInputField;
	public InputField PasswordInputField;


	public void ResetFields()
	{
        UsernameInputField.text = "";
        PasswordInputField.text = "";

	}
}
