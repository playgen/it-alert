using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Unity.Behaviours;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class ItemContainerBehaviour : MonoBehaviour
	{
		#region public events

		public event Action<ItemContainerBehaviour> Click;

		public event Action<ItemContainerBehaviour, ItemBehaviour> Drag;

		public event Action<ContainerState> StateChanged;

		#endregion

		#region game elements

		protected Director Director { get; private set; }

		[SerializeField]
		private Image _containerImage;
		[SerializeField]
		private BlinkBehaviour _blinkBehaviour;

		private bool _beingClicked { get; set; }

		#endregion

		public bool ClickEnable { get; set; }

		// TODO: push this down int othe item containers

		private ContainerState _state = ContainerState.Empty;
		public ContainerState State => _state;

		private ItemContainer _itemContainer;

		public string SpriteOverride { private get; set; }

		#region initialization

		public void Start()
		{
			//Canvas.ForceUpdateCanvases();
		}

		public void Initialize(ItemContainer itemContainer, Director director)
		{
			Director = director;
			if (itemContainer != _itemContainer)
			{
				_itemContainer = itemContainer;

				var itemContainerTypeName = itemContainer?.GetType().Name.ToLowerInvariant();
				var sprite = String.IsNullOrEmpty(SpriteOverride)
					? string.IsNullOrEmpty(itemContainerTypeName)
						? Resources.Load<Sprite>(UIConstants.ItemContainerDefaultSpriteName)
						: Resources.Load<Sprite>(itemContainerTypeName) ?? Resources.Load<Sprite>(UIConstants.ItemContainerDefaultSpriteName)
					: Resources.Load<Sprite>(SpriteOverride);
				_containerImage.sprite = sprite;
			}
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
				ClickEnable = state == ContainerState.HasItem || state == ContainerState.Empty;
				_containerImage.color = state == ContainerState.Disabled
					? UIConstants.ItemContainerDisabledColor
					: UIConstants.ItemContainerEnabledColor;

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
			var hasItem = _itemContainer?.Item != null;
			var isEnabled = _itemContainer?.Enabled == true;

			if (hasItem)
			{
				Transition(ContainerState.HasItem);
			}
			else if (isEnabled)
			{
				Transition(ContainerState.Empty);
			}
			else
			{
				Transition(ContainerState.Disabled);
			}
		}

		public bool OnClickDown()
		{
			_beingClicked = true;
			return _itemContainer.CanRelease;
		}

		public void OnClickUp(ItemBehaviour item = null, bool isDrag = false)
		{
			if (_beingClicked && !isDrag)
			{
				LogProxy.Info("ItemContainer OnClick");

				Click?.Invoke(this);
			}
			else if (item && _itemContainer?.Item == null)
			{
				item.gameObject.SetActive(false);
				Drag?.Invoke(this, item);
			}
			ClickReset();
		}

		public void ClickReset()
		{
			_beingClicked = false;
		}

		#endregion

		protected virtual void OnStateChanged(ContainerState obj)
		{
			StateChanged?.Invoke(obj);
		}


	}
}
