using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class PlayerBehaviour : ActorBehaviour
	{
		public int? InventoryItem => null;

		private Color _playerColor;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private TrailRenderer _trailRenderer;

		[SerializeField]
		private Light _light;

		#region Initialization

		protected override void OnInitialize()
		{
			base.OnInitialize();
			_trailRenderer.enabled = true;
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
				_image.color = _playerColor;
				_light.color = _playerColor;
				_trailRenderer.startColor = _playerColor;
				_trailRenderer.endColor = new Color(_playerColor.r, _playerColor.g, _playerColor.b, 0.875f);
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