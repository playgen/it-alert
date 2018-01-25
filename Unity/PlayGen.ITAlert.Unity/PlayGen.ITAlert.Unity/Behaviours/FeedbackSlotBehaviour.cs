using UnityEngine;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	public class FeedbackSlotBehaviour : MonoBehaviour
	{
		public string CurrentList { get; private set; }

		public void SetList(string listName)
		{
			CurrentList = listName;
		}
	}
}