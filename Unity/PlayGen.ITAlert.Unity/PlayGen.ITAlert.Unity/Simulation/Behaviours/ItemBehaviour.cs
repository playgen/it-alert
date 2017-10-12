using Engine.Systems.Activation.Components;

using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
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
		private ConsumableActivation _consumableActivation;
		private Capture _capture;

		#endregion

		public bool ClickEnable { get; set; }

		#region Initialization

		protected override void OnInitialize()
		{
			_antivirus = null;
			_capture = null;
			_timedActivation = null;
			_consumableActivation = null;

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
				Entity.TryGetComponent(out _consumableActivation);

				UpdateColour();

				_midgroundSprite.enabled = false;
				_midgroundSprite.type = Image.Type.Simple;
				
				//TODO: extract item specific logic to subclasses
				InitializeAntivirus();
				InitializeCapture();
			}
			else
			{
				throw new EntityInitializationException($"Could not load all required components for Entity Id {Entity.Id}");
			}
		}

		#endregion

		#region State Update

		protected override void OnStateUpdated()
		{
			base.OnStateUpdated();
			UpdateColour();

			//TODO: extract item specific logic to subclasses
			UpdateAntivirus();
			UpdateCapture();

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
			if ((_owner?.Value.HasValue ?? false)
                && _activation.ActivationState == ActivationState.Active)
			{
				UIEntity owner;
				if (Director.TryGetEntity(_owner.Value.Value, out owner))
				{
					var playerColour = ((PlayerBehaviour)owner.EntityBehaviour).PlayerColor;
					_activationTimerImage.color = playerColour;
					_backgroundSprite.color = playerColour;
					_foregroundSprite.color = playerColour;
				}
			}
			else
			{
				_activationTimerImage.color = new Color(1f, 1f, 1f, 0.7f);
				_backgroundSprite.color = new Color(1f, 1f, 1f, 1f);
				_foregroundSprite.color = new Color(1f, 1f, 1f, 1f);
			}
		}

		private void InitializeAntivirus()
		{
			if (_antivirus != null)
			{
				_midgroundSprite.sprite = Resources.Load<Sprite>("antivirus-full");
				if (_consumableActivation != null)
				{
					_midgroundSprite.type = Image.Type.Filled;
					_midgroundSprite.fillMethod = Image.FillMethod.Vertical;
				}
				_midgroundSprite.enabled = true;
				UpdateAntivirus();
			}
		}

		private void UpdateAntivirus()
		{
			if (_antivirus != null)
			{
				_midgroundSprite.color = _antivirus.TargetGenome.GetColourForGenome();
				if (_consumableActivation != null)
				{
					_midgroundSprite.fillAmount = (float)_consumableActivation.ActivationsRemaining / _consumableActivation.TotalActivations;
				}
			}
		}

		private void InitializeCapture()
		{
			if (_capture != null)
			{
				_midgroundSprite.sprite = Resources.Load<Sprite>("virus");
				_midgroundSprite.enabled = true;
				UpdateCapture();
			}
		}

		private void UpdateCapture()
		{
			if (_capture != null)
			{
				if (_capture.CapturedGenome == 0)
				{
					_midgroundSprite.enabled = false;
				}
				else
				{
					_midgroundSprite.enabled = true;
					_midgroundSprite.color = _capture.CapturedGenome.GetColourForGenome();
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

		#region player interaction

		public bool CanActivate => _activation.ActivationState == ActivationState.NotActive;
		#endregion
	}
}