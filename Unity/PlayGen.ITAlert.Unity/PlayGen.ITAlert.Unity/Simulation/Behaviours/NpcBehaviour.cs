using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class NpcBehaviour : ActorBehaviour
	{
		public int? InventoryItem => null;

		#region Initialization

		public void OnEnable()
		{
		    transform.localScale = Vector3.one;
        }
	    #endregion

		#region State Update

		protected override void OnStateUpdated()
		{
			UpdatePosition();
		}

		#endregion
	}
}