using System;

using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Components.Player;
using PlayGen.ITAlert.Unity.Exceptions;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class PlayerBehaviour : ActorBehaviour
	{
		public int? InventoryItem => null;

		private Color _playerColor;

		public Color PlayerColor => _playerColor;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private TrailRenderer _trailRenderer;

		[SerializeField]
		private Image _glow;

		[SerializeField]
		private Canvas _canvas;

		private PlayerBitMask _playerBitMask;

		private PlayerColour _playerColour;

		public PlayerBitMask BitMask => _playerBitMask;

		private Destination _destination;

		public int? DestinationSystemId => _destination.Value;

		public int PhotonId { get; set; }

		public int ExternalId { get; set; }

		#region Initialization

		public void Awake()
		{
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			_trailRenderer.Clear();
			_trailRenderer.enabled = true;

			if (Entity.TryGetComponent(out _playerBitMask)
				&& Entity.TryGetComponent(out _playerColour)
				&& Entity.TryGetComponent(out _destination))
			{
				SetColor(_playerColour.HexColour);
				SetGlyph(_playerColour.PlayerGlyph);
			}
			else { 
				throw new SimulationIntegrationException("Mandatory Player components missing");
			}
		}

		private void SetGlyph(string playerGlyph)
		{
			_image.sprite = Resources.Load<Sprite>($"playerglyph_{playerGlyph}");
		}

		#endregion

		private void SetColor(string colour)
		{
			if (colour.IndexOf("#", StringComparison.Ordinal) == -1)
			{
				colour = $"#{colour}";
			}
			if (ColorUtility.TryParseHtmlString(colour, out _playerColor))
			{
			    _glow.color = _playerColor;
				_trailRenderer.startColor = _playerColor;
				_trailRenderer.endColor = new Color(_playerColor.r, _playerColor.g, _playerColor.b, 0.875f);
				_trailRenderer.sortingLayerName = _canvas.sortingLayerName;
				_trailRenderer.sortingOrder += Id;
				_canvas.sortingOrder = 10 + Id;
			}
		}

		public void OnEnable()
		{
            transform.localScale = Vector3.one;
		}
		#region State Update

		protected override void OnStateUpdated()
		{
			UpdatePosition();
			_image.transform.eulerAngles = Vector3.zero;
		}

		#endregion
	}
}