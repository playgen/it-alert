using System.CodeDom;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.EventSystems;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift) && EventSystem.current.currentSelectedGameObject != null)
        {
            var next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();

            if (next == null)
                return;

            var inputfield = next.GetComponent<InputField>();
            if (inputfield != null)
            {
                inputfield.OnPointerClick(new PointerEventData(EventSystem.current));
            }
            EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && EventSystem.current.currentSelectedGameObject != null)
        {
            var next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next == null)
                return;

            var inputfield = next.GetComponent<InputField>();
            if (inputfield != null)
            {
                inputfield.OnPointerClick(new PointerEventData(EventSystem.current));
            }
            EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
        }
    }
}
