using System;
using System.Collections.Generic;
using System.Linq;
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

		private Vector3 _scale;

		#region Initialization

		public void Awake()
		{
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
		}

		#endregion

		public void OnEnable()
		{
			_scale = ((GameObject)Resources.Load("Player")).GetComponent<RectTransform>().localScale;
			transform.localScale = new Vector3(_scale.x / transform.parent.localScale.x, _scale.y / transform.parent.localScale.y, 1);
		}

		#region State Update

		protected override void OnStateUpdated()
		{
			UpdatePosition();
		}

		#endregion
	}
}