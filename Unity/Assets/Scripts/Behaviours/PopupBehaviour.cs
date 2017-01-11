using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PopupBehaviour : MonoBehaviour
{
	private GameObject _buttonContainer;
	private List<GameObject> _buttonsGameObjects = new List<GameObject>();

	private Text _title;

	private GameObject _contentPanel;

	/// <summary>
	/// Set the title and buttons of the popup containier
	/// </summary>
	/// <param name="title">The title of the popup</param>
	/// <param name="output">The desired button Output from left to right</param
	public void SetPopup(string title, Output[] outputs, Action closePopupAction)
	{
		// set the title
		SetTitle(title);

	    if (outputs == null)
	        return;

		_buttonContainer = GameObjectUtilities.Find("PopupContainer/PopupPanelContainer/ButtonContainer").gameObject;
        
		var buttonPrefab = Resources.Load("ButtonContainer");
		foreach (var buttonOutput in outputs)
		{
			var buttonObject = Instantiate(buttonPrefab) as GameObject;
			SetButton(buttonObject.GetComponent<Button>(), buttonOutput.Name, buttonOutput.Action, closePopupAction);
			
			//now set the parent of the object
			buttonObject.transform.SetParent(_buttonContainer.transform, false);
			_buttonsGameObjects.Add(buttonObject);
		}
	}


	/// <summary>
	/// Set the content panel inside the popup
	/// </summary>
	/// <param name="contentParent">The UI rect transform contains all the content required</param>
	public void SetContent(RectTransform contentParent)
	{
		//find the content panel
		_contentPanel = GameObjectUtilities.Find("PopupContainer/PopupPanelContainer/PopupContentContainer").gameObject;

		//set the content to be child of the content panel
		contentParent.transform.SetParent(_contentPanel.transform, false);

		//set the position and padding of the content panel	
		contentParent.anchorMin = new Vector2(0f, 0f);
		contentParent.anchorMax = new Vector2(1f, 1f);

		contentParent.offsetMin = new Vector2(0f,0f);
		contentParent.offsetMax = new Vector2(0f,0f);
	}

	public void ClearContent()
	{
		// find the Content panel
		_contentPanel = GameObjectUtilities.Find("PopupContainer/PopupPanelContainer/PopupContentContainer").gameObject;
		if (_contentPanel.transform.childCount > 0)
		{
			var content = _contentPanel.transform.GetChild(0).gameObject;
			// Destroy the content
			Destroy(content);
			foreach (var button in _buttonsGameObjects)
			{
				Destroy(button.gameObject);
			}
			_buttonsGameObjects.Clear();
		}
	}

	private void SetTitle(string title)
	{
		_title = GameObjectUtilities.Find("PopupContainer/PopupPanelContainer/TitleContainer/Title").GetComponent<Text>();
		_title.text = title;
	}

	private void SetButton(Button button, string text, Action action, Action defaultAction)
	{
        // check if there is a custom action
	    if (action != null)
	    {
	        button.onClick.AddListener(() => action());
	    }
        // Add the default on click action
	    button.onClick.AddListener(() => defaultAction());
		button.gameObject.GetComponentInChildren<Text>().text = text;
	}
	public struct Output
	{
		public string Name;
		public Action Action;

		public Output(string name, Action action)
		{
			Name = name;
			Action = action;
		}
	}
}
