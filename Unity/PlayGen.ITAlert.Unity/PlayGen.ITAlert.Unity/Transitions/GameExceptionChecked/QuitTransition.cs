using GameWork.Core.States.Event;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Transitions.GameExceptionChecked
{
	public class QuitTransition : EventStateTransition
	{
		public void Quit()
		{
			// todo find a graceful way to do this that terminates from this state controller up the tree
			Application.Quit();
		}
	}
}