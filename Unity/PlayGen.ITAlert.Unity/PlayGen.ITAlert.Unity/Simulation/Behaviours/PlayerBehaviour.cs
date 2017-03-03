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

		#region Initialization

		public void Awake()
		{
			_scale = ((GameObject) Resources.Load("Player")).GetComponent<RectTransform>().localScale;
		}

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
				_trailRenderer.sortingLayerName = _canvas.sortingLayerName;
			}
		}

		public void OnEnable()
		{
			transform.localScale = new Vector3(_scale.x / transform.parent.localScale.x, _scale.y / transform.parent.localScale.y, _scale.z / transform.parent.localScale.z);
		}

		#region State Update

		protected override void OnStateUpdated()
		{
			UpdatePosition();
		}

		#endregion
	}
}