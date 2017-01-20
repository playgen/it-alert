using UnityEngine;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	public class FeedbackSlotBehaviour : MonoBehaviour
	{

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
}