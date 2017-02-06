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
	// ReSharper disable CheckNamespace
	public class ItemBehaviour : EntityBehaviour
	{
		[SerializeField]
		private Image _activationTimerImage;

		[SerializeField]
		private SpriteRenderer _iconRenderer;

		public bool ClickEnable { get; set; }

		#region components

		// required
		private CurrentLocation _currentLocation;
		private Owner _owner;
		private IItemType _itemType;
		private Activation _activation;

		// optional components
		private TimedActivation _timedActivation;

		#endregion

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
				GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spriteName);

				Entity.TryGetComponent(out _timedActivation);
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
			//TODO: if owner has changed
			UpdateItemColor();
			UpdateActivationTimer();
		}

		private void UpdateItemColor()
		{
			bool isWhite = GetComponent<SpriteRenderer>().color == Color.white;
			if ((_owner?.Value.HasValue ?? false) && isWhite)
			{
				UIEntity owner;
				if (Director.TryGetEntity(_owner.Value.Value, out owner))
				{
					var playerColour = owner.GameObject.GetComponent<SpriteRenderer>().color;
					GetComponent<SpriteRenderer>().color = playerColour;
					_activationTimerImage.color = playerColour;
				}
			}
			else if ((_owner?.Value.HasValue ?? false) == false && isWhite == false)
			{
				GetComponent<SpriteRenderer>().color = Color.white;
				_activationTimerImage.color = new Color(1f, 1f, 1f, 0.7f);
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

			if (ClickEnable && CanActivate)
			{
				PlayerCommands.ActivateItem(Id);
			}
		}

		#endregion
	}
}