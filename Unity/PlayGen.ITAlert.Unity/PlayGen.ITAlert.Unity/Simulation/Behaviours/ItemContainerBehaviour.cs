﻿using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Unity.Behaviours;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class ItemContainerBehaviour : MonoBehaviour
	{
		#region public events

		public event Action<ItemContainerBehaviour> Click;

		public event Action<ContainerState> StateChanged;

		#endregion

		#region game elements

		private SpriteRenderer _containerImage;
		private BlinkBehaviour _blinkBehaviour;

		#endregion

		public bool CanCapture { get; set; }

		public bool ClickEnable { get; set; }

		// TODO: push this down int othe item containers

		private ContainerState _state = ContainerState.Empty;
		public ContainerState State => _state;

		private ItemContainer _itemContainer;

		public string DefaultSprite { private get; set; } = UIConstants.ItemContainerDefaultSpriteName;

		#region initialization

		public void Initialize(ItemContainer itemContainer)
		{
			_blinkBehaviour = GetComponent<BlinkBehaviour>();
			_containerImage = GetComponent<SpriteRenderer>();

			_itemContainer = itemContainer;

			var itemContainerTypeName = itemContainer.GetType().Name.ToLowerInvariant();
			var sprite = Resources.Load<Sprite>(itemContainerTypeName) ?? Resources.Load<Sprite>(DefaultSprite);
			_containerImage.sprite = sprite;
		}

		//public void Uninitialize()
		//{
		//	var sprite = Resources.Load<Sprite>(DefaultSpriteName);
		//	_containerImage.sprite = sprite;

		//}

		#endregion

		#region player interaction

		private void Transition(ContainerState state)
		{
			if (_state != state)
			{
				_state = state;
				_blinkBehaviour.enabled = state == ContainerState.Capturing || state == ContainerState.Releasing;
				ClickEnable = CanCapture || state == ContainerState.HasItem || state == ContainerState.Empty;
				OnStateChanged(state);
			}
		}

		public bool TryGetItem(out ItemBehaviour itemBehaviour)
		{
			if (_itemContainer.Item.HasValue)
			{
				UIEntity itemEntity;
				if (Director.TryGetEntity(_itemContainer.Item.Value, out itemEntity))
				{
					itemBehaviour = itemEntity.EntityBehaviour as ItemBehaviour;
					return itemBehaviour != null;
				}
			}
			itemBehaviour = null;
			return false;
		}

		public void Update()
		{
			switch (_state)
			{
				case ContainerState.Capturing:
					if (_itemContainer?.Item != null)
					{
						Transition(ContainerState.HasItem);
					}
					break;
				case ContainerState.Releasing:
					if (_itemContainer?.Item == null)
					{
						Transition(ContainerState.Empty);
					}
					break;
				default:
					if (_itemContainer?.Enabled == false)
					{
						Transition(ContainerState.Disabled);
					}
					else if (_itemContainer?.Item != null)
					{
						Transition(ContainerState.HasItem);
					}
					else if (_itemContainer?.Item == null)
					{
						Transition(ContainerState.Empty);
					}
					break;

			}

		}

		public void OnClick()
		{
			Debug.Log("ItemContainer OnClick");

			if (ClickEnable)
			{
				switch (_state)
				{
					case ContainerState.Empty:
						if (CanCapture && (_itemContainer?.CanCapture() ?? false))
						{
							Transition(ContainerState.Capturing);
						}
						break;
					case ContainerState.Capturing:
						Transition(ContainerState.Empty);
						break;
					case ContainerState.Disabled:
						break;
					case ContainerState.HasItem:
						if (CanCapture && (_itemContainer?.CanRelease ?? false))
						{
							Transition(ContainerState.Releasing);
						}
						break;
					case ContainerState.Releasing:
						Transition(ContainerState.HasItem);
						break;
				}

				Click?.Invoke(this);

			}

		}

		#endregion

		protected virtual void OnStateChanged(ContainerState obj)
		{
			StateChanged?.Invoke(obj);
		}


	}
}
