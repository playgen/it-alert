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
		private Image _foregroundSprite;

		[SerializeField]
		private Image _midgroundSprite;

		[SerializeField]
		private Image _backgroundSprite;
		#endregion

		#region components


		// required
		private CurrentLocation _currentLocation;
		private Owner _owner;
		private IItemType _itemType;
		private Activation _activation;

		// optional components
		private TimedActivation _timedActivation;

		private Antivirus _antivirus;
		private Capture _capture;

		#endregion

		#region public events

		public event Action<ItemBehaviour> Click;
		
		#endregion

		public bool ClickEnable { get; set; }
		
		#region Initialization

		protected override void OnInitialize()
		{
			_antivirus = null;
			_capture = null;
			_timedActivation = null;

			if (Entity.TryGetComponent(out _itemType)
				&& Entity.TryGetComponent(out _currentLocation)
				&& Entity.TryGetComponent(out _owner)
				&& Entity.TryGetComponent(out _activation))
			{
				var spriteName = _itemType.GetType().Name.ToLowerInvariant();
				LogProxy.Info($"Creating item type: {spriteName}");
				gameObject.name = $"{Name}_{spriteName}";
				_foregroundSprite.sprite = Resources.Load<Sprite>(spriteName);

				Entity.TryGetComponent(out _timedActivation);
				Entity.TryGetComponent(out _antivirus);
				Entity.TryGetComponent(out _capture);
				UpdateColour();
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
			UpdateActivationTimer();
			UpdateInventory();
		}

		private void UpdateInventory()
		{
			if ((_owner?.Value.HasValue ?? false)
				&& _owner.Value.Value != (Director.Player?.Id ?? -1)
				&& _currentLocation.Value.HasValue == false)
			{
				gameObject.SetActive(false);
			}
			else
			{
				gameObject.SetActive(true);
			}
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
					var playerColour = ((PlayerBehaviour)owner.EntityBehaviour).PlayerColor;
					_activationTimerImage.color = playerColour;
					_backgroundSprite.color = playerColour;
					
					//TODO: extract item specific logic to subclasses
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
				
				//TODO: extract item specific logic to subclasses
				if (_antivirus != null)
				{
					_foregroundSprite.color = _antivirus.GetColourForGenome();
				}
				else
				{
					_foregroundSprite.color = new Color(1f, 1f, 1f, 1f);
				}
			}

			if (_capture == null)
			{
				_midgroundSprite.enabled = false;
			}
			else
			{
				if (_capture.CapturedGenome == 0)
				{
					_midgroundSprite.enabled = false;
				}
				else
				{
					_midgroundSprite.enabled = true;
					_midgroundSprite.color = _capture.GetColourForGenome();
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
			LogProxy.Info("Item OnClick");

			if (ClickEnable)
			{
				Click?.Invoke(this);
			}
		}

		#endregion
	}
}