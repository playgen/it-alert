using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackSlotBehaviour : MonoBehaviour {

	private string _currentList;
	public string CurrentList
	{
		get { return _currentList; }
	}

	public void SetList(string listName)
	{
		_currentList = listName;
	}
}
