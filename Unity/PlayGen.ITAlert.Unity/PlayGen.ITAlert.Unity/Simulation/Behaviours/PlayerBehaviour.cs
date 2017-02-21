using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class PlayerBehaviour : ActorBehaviour
	{
		public int? InventoryItem => null;

		private Color _playerColor;
		
		#region Initialization

		public void Start()
		{
			gameObject.transform.SetParent(Director.Graph.transform, false);

		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
		}

		#endregion
		public void SetColor(string colour)
		{
			if (colour.IndexOf("#", StringComparison.Ordinal) == -1)
			{
				colour = $"#{colour}";
			}
			if (ColorUtility.TryParseHtmlString(colour, out _playerColor))
			{
				GetComponent<SpriteRenderer>().color = _playerColor;
				GetComponent<TrailRenderer>().startColor = _playerColor;
				GetComponent<TrailRenderer>().endColor = new Color(_playerColor.r, _playerColor.g, _playerColor.b, 0.875f);
			}
		}

		#region State Update

		protected override void OnStateUpdated()
		{
			UpdatePosition();

			////TODO: if inventory has changed
			//_historicPositions.Enqueue(transform.position);
			//if (_historicPositions.Count == PositionHistory)
			//{
			//	_historicPositions.Dequeue();
			//	ManageInventory(_historicPositions.Count < InventoryPositionHistory ? _historicPositions.Last() : _historicPositions.ToArray()[InventoryPositionHistory - 1]);
			//}
			//DrawTrail();
		}

		#endregion
	}
}