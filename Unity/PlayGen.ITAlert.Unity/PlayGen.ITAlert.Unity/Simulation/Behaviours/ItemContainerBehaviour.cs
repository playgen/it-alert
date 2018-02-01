using System;

using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class ItemContainerBehaviour : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
	{
		#region public events

		public event Action<ContainerState> StateChanged;

		#endregion

		#region game elements

		protected Director Director { get; private set; }

		[SerializeField]
		private Image _containerImage;
		[SerializeField]
		private BlinkBehaviour _blinkBehaviour;

		private int? _containerIndex;

		#endregion

		public int ContainerIndex => _containerIndex ?? -10;

		// TODO: push this down int othe item containers

		public ContainerState State { get; private set; } = ContainerState.Empty;

		private ItemContainer _itemContainer;

		private ItemBehaviour _moveItem;

		private int _moveItemIndex;

		private bool _moveHighlightGrow;

		public bool CanRelease => _itemContainer.CanRelease;

		public string SpriteOverride { private get; set; }

		#region initialization
		
		public void Start()
		{
			//Canvas.ForceUpdateCanvases();
		}

		public void Initialize(Director director, int containerIndex)
		{
			Director = director;
			_containerIndex = containerIndex;
		}

		public void Initialize(ItemContainer itemContainer, Director director, int containerIndex)
		{
			_itemContainer = itemContainer;
			Director = director;
			_containerIndex = containerIndex;

			UpdateImage();
		}

		public bool TryUpdate(ItemContainer itemContainer)
		{
			if (itemContainer != _itemContainer)
			{
				_itemContainer = itemContainer;
				UpdateImage();

				return true;
			}

			return false;
		}

		private void UpdateImage()
		{
			var itemContainerTypeName = _itemContainer?.GetType().Name.ToLowerInvariant();
			var sprite = string.IsNullOrEmpty(SpriteOverride)
				? string.IsNullOrEmpty(itemContainerTypeName)
					? Resources.Load<Sprite>(UIConstants.ItemContainerDefaultSpriteName)
					: Resources.Load<Sprite>(itemContainerTypeName) ?? Resources.Load<Sprite>(UIConstants.ItemContainerDefaultSpriteName)
				: Resources.Load<Sprite>(SpriteOverride);
			_containerImage.sprite = sprite;
			if (GetComponentInChildren<HoverObject>())
			{
				GetComponentInChildren<HoverObject>().SetHoverText(!string.Equals(sprite.name, UIConstants.ItemContainerDefaultSpriteName, StringComparison.CurrentCultureIgnoreCase) ? sprite.name.ToUpperInvariant() + "_DESCRIPTION" : string.Empty);
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
			if (State != state)
			{
				State = state;
				_containerImage.color = state == ContainerState.Disabled
					? UIConstants.ItemContainerDisabledColor
					: UIConstants.ItemContainerEnabledColor;

				OnStateChanged(state);
			}
		}

		public bool TryGetItem(out ItemBehaviour itemBehaviour)
		{
			if (_itemContainer?.Item != null)
			{
				if (Director.TryGetEntity(_itemContainer.Item.Value, out var itemEntity))
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

			if (_moveItem)
			{
				var newScale = transform.localScale.x + ((_moveHighlightGrow ? Time.smoothDeltaTime : -Time.smoothDeltaTime) * 0.25f);
				if (newScale < 0.95f)
				{
					newScale = 0.95f;
					_moveHighlightGrow = true;
				}
				if (newScale > 1.05f)
				{
					newScale = 1.05f;
					_moveHighlightGrow = false;
				}
				transform.localScale = Vector3.one * newScale;
				if (TryGetItem(out var item))
				{
					item.transform.localScale = Vector3.one * newScale;
				}
			}

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

		#endregion

		protected virtual void OnStateChanged(ContainerState obj)
		{
			StateChanged?.Invoke(obj);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (_moveItem)
			{
				if (GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel/ItemContainer_Inventory").GetComponent<ItemContainerBehaviour>().TryGetItem(out var inventory) && inventory.Id == _moveItem.Id)
				{
					if (TryGetItem(out var containerItem) == false)
					{
						PlayerCommands.DropItem(_moveItem.Id, ContainerIndex);
					}
					else
					{
						PlayerCommands.SwapInventoryItem(containerItem.Id, ContainerIndex, _moveItem.Id);
					}
				}
				else
				{
					if (TryGetItem(out var containerItem))
					{
						PlayerCommands.SwapSubsystemItem(_moveItem.Id, _moveItemIndex, containerItem.Id, ContainerIndex);
					}
					else
					{
						PlayerCommands.MoveItem(_moveItem.Id, _moveItemIndex, ContainerIndex);
					}
				}
			}
			else if (TryGetItem(out var item))
			{
				if (item.CurrentLocation.Value == Director.Player.CurrentLocationEntity.EntityBehaviour.Id)
				{
					item.OnPointerClick(this, Director);
				}
				else if (item.CurrentLocation.Value == null)
				{
					GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel").GetComponentInChildren<ItemBehaviour>().OnPointerClick(this, Director);
				}
			}
		}

		public void Highlight(ItemBehaviour item, int index)
		{
			_moveItem = item;
			_moveItemIndex = index;
			_moveHighlightGrow = false;
		}

		public void RemoveHighlight()
		{
			_moveItem = null;
			_moveItemIndex = 0;
			transform.localScale = Vector3.one;
			if (TryGetItem(out var item))
			{
				item.transform.localScale = Vector3.one;
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
		}

		public void OnPointerUp(PointerEventData eventData)
		{
		}
	}
}
