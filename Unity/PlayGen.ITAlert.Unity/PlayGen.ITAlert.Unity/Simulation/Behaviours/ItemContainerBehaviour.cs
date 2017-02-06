using System;
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
		public event Action<ItemContainerBehaviour> Click;

		public event Action<ContainerState> StateChanged;
		
		private ContainerState _state = ContainerState.Empty;
		public ContainerState State => _state;
		
		public const string Prefab = "ItemContainer";
		public const string DefaultSprite = "ItemContainer";


		private BlinkBehaviour _blinkBehaviour;


		public bool CanCapture { get; set; }


		#region game elements

		private SpriteRenderer _containerImage;

		#endregion

		public bool ClickEnable { get; set; }

		private ItemContainer _itemContainer;

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
		//	var sprite = Resources.Load<Sprite>(DefaultSprite);
		//	_containerImage.sprite = sprite;

		//}

		#endregion

		#region player interaction

		public bool Capturing { get; set; }

		private bool CanActivate => true;

		private void Transition(ContainerState state)
		{
			_state = state;
			_blinkBehaviour.enabled = state == ContainerState.Capturing || state == ContainerState.Releasing;
			ClickEnable = CanCapture || state == ContainerState.HasItem || state == ContainerState.Empty;
			OnStateChanged(state);
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

		public void SetItem(ItemBehaviour item)
		{
			_itemContainer.Item = item?.Id;
			Transition(_itemContainer.Item.HasValue
				? ContainerState.HasItem
				: ContainerState.Empty);
		}
		
		public void OnClick()
		{
			Debug.Log("ItemContainer OnClick");

			if (ClickEnable)
			{
				switch (_state)
				{
					case ContainerState.Empty:
						if (CanCapture)
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
						if (CanCapture)
						{
							Transition(ContainerState.Releasing);
						}
						break;
					case ContainerState.Releasing:
						Transition(ContainerState.HasItem);
						break;
				}

				OnClick(this);
			}

		}

		#endregion

		protected virtual void OnClick(ItemContainerBehaviour obj)
		{
			Click?.Invoke(obj);
		}

		protected virtual void OnStateChanged(ContainerState obj)
		{
			StateChanged?.Invoke(obj);
		}
	}
}
