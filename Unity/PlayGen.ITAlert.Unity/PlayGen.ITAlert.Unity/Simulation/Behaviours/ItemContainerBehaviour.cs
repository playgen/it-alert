using System;

using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Localization;

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

		private int? _containerIndex;

		private int? _subsystemId;

		#endregion

		public int ContainerIndex => _containerIndex ?? -10;

		// TODO: push this down int othe item containers

		public ContainerState State { get; private set; } = ContainerState.Empty;

		private ItemContainer _itemContainer;

		private ItemBehaviour _moveItem;

		private ItemContainerBehaviour _moveItemContainer;

		private bool _moveHighlightGrow;

		[SerializeField]
		private GameObject _selectionOptions;

		[SerializeField]
		private GameObject _leftButton;

		[SerializeField]
		private GameObject _rightButton;

		[SerializeField]
		private Text _descriptionText;

		private string _currentAnim;

		public bool CanRelease => _itemContainer.CanRelease;

		public bool IsEnabled => _itemContainer.Enabled;

		public string SpriteOverride { private get; set; }

		#region initialization

		public void Initialize(ItemContainer itemContainer, Director director, int containerIndex, int? subsystemId)
		{
			_itemContainer = itemContainer;
			Director = director;
			_containerIndex = containerIndex;
			_subsystemId = subsystemId;
			if (_selectionOptions)
			{
				_selectionOptions.SetActive(false);
				_leftButton.GetComponent<Button>().onClick.AddListener(MoveAndUse);
				_leftButton.GetComponentInChildren<Text>().text = Localization.Get("PLACE_AND_USE_BUTTON");
				_leftButton.GetComponentInChildren<TextLocalization>().Key = "PLACE_AND_USE_BUTTON";
				_rightButton.GetComponent<Button>().onClick.AddListener(Move);
				_rightButton.GetComponentInChildren<Text>().text = Localization.Get("PLACE_BUTTON");
				_rightButton.GetComponentInChildren<TextLocalization>().Key = "PLACE_BUTTON";
			}
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

		public bool CanContain(int itemId)
		{
			return _itemContainer.CanContain(itemId);
		}

		public void Update()
		{
			var hasItem = _itemContainer?.Item != null;

			if (_moveItem && IsEnabled && CanContain(_moveItem.Id) && (!hasItem || (_moveItemContainer && _moveItemContainer.CanContain(_itemContainer.Item.Value))))
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
			else if (IsEnabled)
			{
				Transition(ContainerState.Empty);
			}
			else
			{
				Transition(ContainerState.Disabled);
			}

			if (_selectionOptions && _selectionOptions.activeSelf && !IsInvoking("OptionsDelay") && !IsInvoking(nameof(ResetOptions)) && !IsInvoking("EnableOptions"))
			{
				if (hasItem || Input.GetMouseButtonUp(0) || Director.Player.CurrentLocationEntity.EntityBehaviour.Entity.CreatedFromArchetype == nameof(ConnectionNode))
				{
					var optionAnim = _selectionOptions.GetComponent<Animation>();
					var clipName = _currentAnim;
					optionAnim[clipName].time = optionAnim[clipName].length;
					optionAnim[clipName].speed = -1;
					optionAnim.Play(clipName);
					Invoke(nameof(ResetOptions), 0.34f);
					Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
				}
			}
		}

		#endregion

		protected virtual void OnStateChanged(ContainerState obj)
		{
			StateChanged?.Invoke(obj);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			var description = !string.Equals(_containerImage.sprite.name, UIConstants.ItemContainerDefaultSpriteName, StringComparison.CurrentCultureIgnoreCase) ? _containerImage.sprite.name.ToUpperInvariant() + "_DESCRIPTION" : string.Empty;
			GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel/ItemContainer_Inventory").GetComponent<ItemContainerBehaviour>().TryGetItem(out var inventory);
			if (_moveItem)
			{
				if (inventory && inventory.Id == _moveItem.Id)
				{
					if (_moveItemContainer != null)
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
						if (TryGetItem(out var containerItem) == false)
						{
							PlayerCommands.DropAndActivateItem(_moveItem.Id, ContainerIndex);
						}
						else
						{
							PlayerCommands.SwapAndActivateInventoryItem(containerItem.Id, ContainerIndex, _moveItem.Id);
						}
					}
				}
				else if (_moveItemContainer != null)
				{
					var moveItemIndexValue = _moveItemContainer.ContainerIndex;
					if (TryGetItem(out var containerItem))
					{
						PlayerCommands.SwapSubsystemItem(_moveItem.Id, moveItemIndexValue, containerItem.Id, ContainerIndex);
					}
					else
					{
						PlayerCommands.MoveItem(_moveItem.Id, moveItemIndexValue, ContainerIndex);
					}
				}
			}
			else if (TryGetItem(out var item))
			{
				if (item.CurrentLocation.Value == Director.Player.CurrentLocationEntity.EntityBehaviour.Id)
				{
					item.OnPointerClick(this, Director);
				}
				else if (_subsystemId == null && item.CurrentLocation.Value == null)
				{
					GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel").GetComponentInChildren<ItemBehaviour>().OnPointerClick(this, Director);
				}
				else if (_subsystemId != null)
				{
					PlayerCommands.Move(_subsystemId.Value);
				}
			}
			else if (_subsystemId != null)
			{
				if (_subsystemId != Director.Player.CurrentLocationEntity.EntityBehaviour.Id)
				{
					PlayerCommands.Move(_subsystemId.Value);
				}
				else if (_selectionOptions && !_selectionOptions.activeSelf && (inventory || !string.IsNullOrEmpty(description)) && !IsInvoking("OptionsDelay") && !IsInvoking(nameof(ResetOptions)) && !IsInvoking("EnableOptions"))
				{
					var canContain = IsEnabled && (!inventory || CanContain(inventory.Id));
					_leftButton.SetActive(true);
					_rightButton.SetActive(true);
					_leftButton.transform.localScale = Vector3.one;
					_rightButton.transform.localScale = Vector3.one;
					_descriptionText.transform.localScale = Vector3.one;
					GetComponent<Canvas>().sortingOrder = 100;
					_selectionOptions.SetActive(true);
					Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
					Invoke("EnableOptions", 0.34f);
					var optionAnim = _selectionOptions.GetComponent<Animation>();
					_currentAnim = inventory ? !string.IsNullOrEmpty(description) ? "ItemContainerSelection" : "ItemContainerNoDescription" : "ItemContainerOnlyDescription";
					_descriptionText.GetComponent<TextLocalization>().Key = description;
					_descriptionText.GetComponent<TextLocalization>().Set();
					var bestFitSize = _selectionOptions.GetComponentsInChildren<Button>().BestFit();
					_descriptionText.fontSize = bestFitSize;
					_leftButton.SetActive(canContain && Director.CommandSystem.TryGetHandler(typeof(DropAndActivateItemCommand), out var dropAndActivateHandler) && dropAndActivateHandler.Enabled && Director.CommandSystem.TryGetHandler(typeof(SwapInventoryItemAndActivateCommand), out dropAndActivateHandler) && dropAndActivateHandler.Enabled);
					_rightButton.SetActive(canContain && Director.CommandSystem.TryGetHandler(typeof(DropItemCommand), out var dropHandler) && dropHandler.Enabled && Director.CommandSystem.TryGetHandler(typeof(SwapInventoryItemCommand), out dropHandler) && dropHandler.Enabled);
					optionAnim[_currentAnim].time = 0;
					optionAnim[_currentAnim].speed = 1;
					optionAnim.Play(_currentAnim);
				}
			}
		}

		private void ResetOptions()
		{
			GetComponent<Canvas>().sortingOrder = 0;
			_leftButton.transform.localScale = Vector3.one;
			_rightButton.transform.localScale = Vector3.one;
			_selectionOptions.SetActive(false);
		}

		public void Highlight(ItemBehaviour item, ItemContainerBehaviour container)
		{
			_moveItem = item;
			_moveItemContainer = container;
			_moveHighlightGrow = false;
		}

		public void RemoveHighlight()
		{
			_moveItem = null;
			_moveItemContainer = null;
			transform.localScale = Vector3.one;
			if (TryGetItem(out var item))
			{
				item.transform.localScale = Vector3.one;
			}
		}

		private void Move()
		{
			if (!IsInvoking("DisableOptions") && !IsInvoking("EnableOptions"))
			{
				Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
				ResetOptions();
				if (GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel/ItemContainer_Inventory").GetComponent<ItemContainerBehaviour>().TryGetItem(out var inventory))
				{
					PlayerCommands.DropItem(inventory.Id, ContainerIndex);
				}
			}
		}

		private void MoveAndUse()
		{
			if (!IsInvoking("DisableOptions") && !IsInvoking("EnableOptions"))
			{
				Invoke("OptionsDelay", Time.smoothDeltaTime * 2);
				ResetOptions();
				if (GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel/ItemContainer_Inventory").GetComponent<ItemContainerBehaviour>().TryGetItem(out var inventory))
				{
					PlayerCommands.DropAndActivateItem(inventory.Id, ContainerIndex);
				}
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
