using System;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.UI.Components.Items;
using PlayGen.ITAlert.Unity.Exceptions;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class ItemBehaviour : EntityBehaviour
	{
		#region editor fields

		[SerializeField]
		private Image _activationTimerImage;

		[SerializeField]
		private SpriteRenderer _foregroundSprite;

		[SerializeField]
		private SpriteRenderer _midgroundSprite;

		[SerializeField]
		private SpriteRenderer _backgroundSprite;
		#endregion

		#region unity components

		// required
		private CurrentLocation _currentLocation;
		private Owner _owner;
		private IItemType _itemType;
		private Activation _activation;

		// optional components
		private TimedActivation _timedActivation;

		private Antivirus _antivirus;

		#endregion

		#region public events

		public event Action<ItemBehaviour> Click;
		
		#endregion

		public bool ScaleUp { private get; set; }

		public bool ClickEnable { get; set; }


		#region Initialization

		public void Start()
		{
			gameObject.transform.SetParent(Director.Graph.transform, false);
		}

		public void Awake()
		{

		}

		protected override void OnInitialize()
		{
			if (Entity.TryGetComponent(out _itemType)
				&& Entity.TryGetComponent(out _currentLocation)
				&& Entity.TryGetComponent(out _owner)
				&& Entity.TryGetComponent(out _activation))
			{
				var spriteName = _itemType.GetType().Name.ToLowerInvariant();
				_foregroundSprite.sprite = Resources.Load<Sprite>(spriteName);

				Entity.TryGetComponent(out _timedActivation);
				Entity.TryGetComponent(out _antivirus);
			}
			else
			{
				throw new EntityInitializationException($"Could not load all required components for Entity Id {Entity.Id}");
			}
		}

		#endregion

		#region Unity Update

		protected override void OnFixedUpdate()
		{
		}

		protected override void OnUpdate()
		{
//			_dragCount = Math.Max(0, --_dragCount);
		}

		#endregion

		#region State Update

		protected override void OnStateUpdated()
		{
			UpdateColour();
			UpdateScale();
			UpdateActivationTimer();
		}

		private void UpdateScale()
		{
			transform.localScale = ScaleUp
				? UIConstants.ItemPanelItemScale
				: UIConstants.DefaultItemScale;
		}

		private void UpdateForegroundColour()
		{
		}

		private void UpdateMidgroundColour()
		{
		}

		private void UpdateColour()
		{
			if (_owner?.Value.HasValue ?? false)
			{
				UIEntity owner;
				if (Director.TryGetEntity(_owner.Value.Value, out owner))
				{
					var playerColour = owner.GameObject.GetComponent<SpriteRenderer>().color;
					_activationTimerImage.color = playerColour;
					_backgroundSprite.color = playerColour;

					if (_antivirus != null)
					{
						_foregroundSprite.color = _antivirus.GetColourForGenome();
					}
					else
					{
						_foregroundSprite.color = playerColour;
					}
				}
			}
			else 
			{
				_activationTimerImage.color = new Color(1f, 1f, 1f, 0.7f);
				_backgroundSprite.color = new Color(1f, 1f, 1f, 1f);
				if (_antivirus != null)
				{
					_foregroundSprite.color = _antivirus.GetColourForGenome();
				}
				else
				{
					_foregroundSprite.color = new Color(1f, 1f, 1f, 1f);
				}
			}
		}

		private void UpdateActivationTimer()
		{
			if (_timedActivation != null)
			{
				if (_activation.ActivationState == ActivationState.Active)
				{
					_activationTimerImage.fillAmount = 1f - _timedActivation.GetActivationProportion();
				}
				else
				{
					_activationTimerImage.fillAmount = 0f;
				}

			}
		}

		#endregion
		
		private void TriggerHint()
		{
			//TODO: reimplement
			//UIManager.instance.DisplayHint(GetComponent<SVGRenderer>().vectorGraphics, _hintText);
		}

		#region player interaction

		public bool CanActivate => _activation.ActivationState == ActivationState.NotActive;

		public void OnClick()
		{
			Debug.Log("Item OnClick");

			if (ClickEnable)
			{
				Click?.Invoke(this);
			}
		}

		#endregion
	}
}