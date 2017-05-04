using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Components;
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
		private Light _light;

		[SerializeField]
		private Canvas _canvas;

		private Vector3 _scale;

		private PlayerBitMask _playerBitMask;

		private PlayerColour _playerColour;

		public PlayerBitMask BitMask => _playerBitMask;

		private Destination _destination;

		public int? DestinationSystemId => _destination.Value;

		public int PhotonId { get; set; }

		#region Initialization

		public void Awake()
		{
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
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
				_image.color = _playerColor;
				_light.color = _playerColor;
				_trailRenderer.startColor = _playerColor;
				_trailRenderer.endColor = new Color(_playerColor.r, _playerColor.g, _playerColor.b, 0.875f);
				_trailRenderer.sortingLayerName = _canvas.sortingLayerName;
			}
		}

		public void OnEnable()
		{
			_scale = ((GameObject)Resources.Load("Player")).GetComponent<RectTransform>().localScale;
			if (_scale == null)
			{
				LogProxy.Warning("PlayerBehaviour OnEnable: _scale is null");
			}
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