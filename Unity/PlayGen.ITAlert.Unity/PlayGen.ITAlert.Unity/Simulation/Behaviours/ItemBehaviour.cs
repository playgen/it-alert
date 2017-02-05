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

		private bool _dragging;

		private int _dragCount;

		public bool Dragging => _dragging || _dragCount > 0;

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
			UpdatePosition();
			UpdateItemColor();
			UpdateActivationTimer();
		}

		private void UpdatePosition()
		{
			//UIEntity currentLocationEntity;
			//if (Director.TryGetEntity(_currentLocation.Value, out currentLocationEntity))
			//{
			//	if (currentLocationEntity.Type == EntityType.Subsystem)
			//	{
			//		//Item
			//	}
			//	else
			//	{
					
			//	}
			//}
		}

		private void UpdateItemColor()
		{
			//bool isWhite = GetComponent<SpriteRenderer>().color == Color.white ? true : false;
			//if (_owner.Value.HasValue && isWhite)
			//{
			//	UIEntity owner;
			//	if (Director.TryGetEntity(_owner.Value.Value, out owner))
			//	{
			//		var playerColour = owner.GameObject.GetComponent<SpriteRenderer>().color;
			//		_iconRenderer.color = playerColour;
			//		_activationTimerImage.color = playerColour;
			//	}
			//	//TriggerHint();
			//}
			//else if (!_owner.Value.HasValue && !isWhite)
			//{
			//	_iconRenderer.color = Color.white;
			//	_activationTimerImage.color = new Color(1f, 1f, 1f, 0.7f);
			//}
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

		private bool PlayerOwnsItem()
		{
			return Director.Player != null
					&& _owner.Value.HasValue
					&& _owner.Value.Value == Director.Player.Id;
		}

		public void OnClick(bool dragging)
		{
			Debug.Log("Item OnClick");

			if (CanActivate())
			{
				if (PlayerOwnsItem())
				{
					if (dragging == false)
					{
						PlayerCommands.DisownItem(this.Id);
					}
				}
				else
				{
					//PlayerCommands.PickupItem(Id, EntityState.CurrentNode.Value);
				}
			}
		}

		public bool CanActivate()
		{
			return false;
				//EntityState.Active == false 
				//&& EntityState.CanActivate 
				//&& (EntityState.Owner.HasValue == false || EntityState.Owner.Value == Director.Player.Id);
		}

		public void Deactivate()
		{
			throw new NotImplementedException();
		}

		public void Activate()
		{
			// TODO: Start Item Animation
			throw new NotImplementedException();
		}

		public void DragStart()
		{
			_dragging = true;
		}

		public void DragStop()
		{
			_dragging = false;
			_dragCount = 2;
		}

		#endregion

	}
}