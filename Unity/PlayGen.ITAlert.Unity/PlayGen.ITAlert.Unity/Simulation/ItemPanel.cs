using System;
using System.Collections.Generic;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;

using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation
{
	public class ItemPanel
	{
		private const int ItemCount = SimulationConstants.SubsystemMaxItems;

		private GameObject _inventoryItemContainerObject;
		private ItemContainerBehaviour _inventoryItemContainerBehaviour;
		private UIEntity _inventoryItem;
		private InventoryItemContainer _inventoryItemContainer;

		private readonly GameObject[] _systemItemContainers;
		private readonly ItemContainerBehaviour[] _systemItemContainerBehaviours;
		private readonly UIEntity[] _systemItems;
		
		private PlayerBehaviour _player;

		private int _playerLocationLast = -1;

		public ItemPanel()
		{
			_systemItemContainerBehaviours = new ItemContainerBehaviour[ItemCount];
			_systemItemContainers = new GameObject[ItemCount];
			_systemItems = new UIEntity[ItemCount];
		}

		public void Initialize()
		{
			const float itemScale = 1.6f;
			const string sortingLayerOverride = "UI";

			_player = Director.Player;
			ItemStorage itemStorage;
			if (_player.Entity.TryGetComponent(out itemStorage) == false)
			{
				throw new SimulationIntegrationException("No item storage found on player");
			}
			_inventoryItemContainer = itemStorage.Items[0] as InventoryItemContainer;

			#region inventory container

			_inventoryItemContainerObject = GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/InventoryItemContainer");
			_inventoryItemContainerBehaviour = _inventoryItemContainerObject.GetComponent<ItemContainerBehaviour>();

			_inventoryItemContainerBehaviour.Initialize(_inventoryItemContainer);
			_inventoryItemContainerBehaviour.ClickEnable = true;
			_inventoryItemContainerBehaviour.CanCapture = true;

			_inventoryItem = new UIEntity(nameof(Item));
			_inventoryItem.GameObject.SetActive(false);
			_inventoryItem.GameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerOverride;
			_inventoryItem.GameObject.transform.localScale = new Vector2(itemScale, itemScale);

			_inventoryItemContainerBehaviour.Click += InventoryItemContainerBehaviourOnClick;

			#endregion

			#region system items

			for (var i = 0; i < ItemCount; i++)
			{
				_systemItemContainers[i] = (GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/ItemContainer" + i));
				_systemItemContainerBehaviours[i] = _systemItemContainers[i].GetComponent<ItemContainerBehaviour>();
				_systemItemContainerBehaviours[i].Click += SystemContaineBehaviourOnClick;

				var itemPanelItem = new UIEntity(nameof(Item));
				Director.AddUntrackedEntity(itemPanelItem);

				_systemItems[i] = itemPanelItem;
				_systemItems[i].GameObject.SetActive(false);
				_systemItems[i].GameObject.transform.position = _systemItemContainers[i].transform.position;
				_systemItems[i].GameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerOverride;
				_systemItems[i].GameObject.transform.localScale = new Vector2(itemScale, itemScale);

				var systemItemBehaviour = (ItemBehaviour) itemPanelItem.EntityBehaviour;
				systemItemBehaviour.ClickEnable = true;
			}

			#endregion
		}

		private void SystemContaineBehaviourOnClick(ItemContainerBehaviour itemContainerBehaviour)
		{
			ItemBehaviour item;
			switch (_inventoryItemContainerBehaviour.State)
			{
				case ContainerState.Capturing:
					if (itemContainerBehaviour.TryGetItem(out item))
					{
						_inventoryItemContainerBehaviour.SetItem(item);
					}
					break;
				case ContainerState.Releasing:
					if (_inventoryItemContainerBehaviour.TryGetItem(out item)
						&& itemContainerBehaviour.State == ContainerState.Empty)
					{
						PlayerCommands.PickupItem(item.Id);
					}
					break;
				default:
					switch (itemContainerBehaviour.State)
					{
						case ContainerState.HasItem:
							if (itemContainerBehaviour.TryGetItem(out item))
							{
								PlayerCommands.ActivateItem(item.Id);
							}
							break;
					}
					break;
			}
		}

		private void InventoryItemContainerBehaviourOnClick(ItemContainerBehaviour itemContainerBehaviour)
		{
		}
		
		private void DeactivateItem(UIEntity itemEntity)
		{
			itemEntity.GameObject.SetActive(false);
			itemEntity.EntityBehaviour.Uninitialize();
		}

		private void DeactivateSystemItem(int i)
		{
			DeactivateItem(_systemItems[i]);
			//_systemItemContainerBehaviours[i].Uninitialize();
			//_systemItemContainerBehaviours[i].SetItem(null);
		}

		public void Update()
		{
			if (_player == null)
			{
				throw new SimulationIntegrationException($"Player is unassigned");
			}

			//if (_inventoryItemContainer.Item.HasValue == false 
			//	&& _inventoryItemContainerBehaviour.State == ContainerState.HasItem)
			//{
			//	DeactivateItem(_inventoryItem);
			//	//_systemItemContainerBehaviours[i].Uninitialize();
			//	_inventoryItemContainerBehaviour.SetItem(null);
			//}
			//else if (_inventoryItemContainer.Item != null
			//	&& _inventoryItemContainerBehaviour.State == ContainerState.Empty)
			//{
			//	UIEntity inventoryEntity;
			//	if (Director.TryGetEntity(_inventoryItemContainer.Item.Value, out inventoryEntity))
			//	{
			//		_inventoryItem.EntityBehaviour.Initialize(inventoryEntity.EntityBehaviour.Entity);
			//		_inventoryItem.GameObject.SetActive(true);
			//	}
			//}

			if (_player.CurrentLocationEntity.Id != _playerLocationLast)
			{
				_playerLocationLast = _player.CurrentLocationEntity.Id;

				var currentLocation = _player.CurrentLocationEntity;
				var subsystemBehaviour = currentLocation.EntityBehaviour as SubsystemBehaviour;
				if (subsystemBehaviour != null && subsystemBehaviour.ItemStorage != null)
				{
					for (var i = 0; i < ItemCount; i++)
					{
						if (i > subsystemBehaviour.ItemStorage.Items.Length - 1)
						{
							DeactivateSystemItem(i);
							continue;
						}

						var itemContainer = subsystemBehaviour.ItemStorage.Items[i];
						if ((itemContainer == null || itemContainer.Item.HasValue == false) && _systemItems[i].GameObject.activeSelf)
						{
							DeactivateSystemItem(i);
						}
						else if (itemContainer?.Item != null)
						{
							UIEntity itemEntity;
							if (Director.TryGetEntity(itemContainer.Item.Value, out itemEntity)
								&& itemContainer.Item.Value != (_systemItems[i].EntityBehaviour.Entity?.Id ?? -1))
							{
								_systemItemContainerBehaviours[i].Initialize(itemContainer);
								_systemItems[i].EntityBehaviour.Initialize(itemEntity.EntityBehaviour.Entity);
								_systemItems[i].GameObject.SetActive(true);
								_systemItemContainerBehaviours[i].SetItem(_systemItems[i].EntityBehaviour as ItemBehaviour);
								_systemItemContainerBehaviours[i].ClickEnable = true;
							}
						}
					}
				}
				else
				{
					for (var i = 0; i < ItemCount; i++)
					{
						DeactivateItem(_systemItems[i]);
					}
				}
			}
		}
	}
}
