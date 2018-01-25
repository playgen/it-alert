using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class NpcBehaviour : ActorBehaviour
	{
		public int? InventoryItem => null;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private Canvas _canvas;

		#region Initialization

		public void Awake()
		{
		}

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