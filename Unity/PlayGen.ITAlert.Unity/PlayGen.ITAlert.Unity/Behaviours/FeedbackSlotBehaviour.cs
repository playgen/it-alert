using UnityEngine;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	public class FeedbackSlotBehaviour : MonoBehaviour
	{

		private string _currentList;

		public string CurrentList => _currentList;

	    public void SetList(string listName)
		{
			_currentList = listName;
		}
	}
}