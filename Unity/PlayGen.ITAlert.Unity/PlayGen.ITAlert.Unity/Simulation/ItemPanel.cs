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

		private GameObject _inventoryItemContainer;
		private ItemContainerBehaviour _inventoryItemBehaviour;
		private UIEntity _inventoryItem;

		private GameObject[] _systemItemContainers;
		private UIEntity[] _systemItems;
		
		private PlayerBehaviour _player;

		private int _playerLocationLast = -1;

		public ItemPanel()
		{
			_systemItemContainers = new GameObject[ItemCount];
			_systemItems = new UIEntity[ItemCount];
		}

		public void Initialize()
		{
			_player = Director.Player;
			ItemStorage itemStorage;
			if (_player.Entity.TryGetComponent(out itemStorage) == false)
			{
				throw new SimulationIntegrationException("No item storage found on player");
			}
			var inventoryContainer = itemStorage.Items[0] as InventoryItemContainer;

			_inventoryItemContainer = GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/InventoryItemContainer");
			_inventoryItemBehaviour = _inventoryItemContainer.GetComponent<ItemContainerBehaviour>();
			_inventoryItemBehaviour.Initialize(inventoryContainer);
			_inventoryItemBehaviour.ClickEnable = true;

			_inventoryItem = new UIEntity(nameof(Item));
			_inventoryItem.GameObject.SetActive(false);
			
			for (var i = 0; i < ItemCount; i++)
			{
				_systemItemContainers[i] = (GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/ItemContainer" + 0));

				var itemPanelItem = new UIEntity(nameof(Item));
				Director.AddUntrackedEntity(itemPanelItem);
				_systemItems[i] = itemPanelItem;
				_systemItems[i].GameObject.SetActive(false);
				_systemItems[i].GameObject.transform.position = _systemItemContainers[i].transform.position;
				((ItemBehaviour) itemPanelItem.EntityBehaviour).ClickEnable = true;
			}

		}

		private void DeactivateItem(UIEntity itemEntity)
		{
			itemEntity.GameObject.SetActive(false);
			itemEntity.EntityBehaviour.Uninitialize();
		}

		public void Update()
		{
			if (_player == null)
			{
				throw new SimulationIntegrationException($"Player is unassigned");
			}

			_inventoryItemBehaviour.HandlePulse();

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
							DeactivateItem(_systemItems[i]);
							continue;
						}

						var itemContainer = subsystemBehaviour.ItemStorage.Items[i];
						if ((itemContainer == null || itemContainer.Item.HasValue == false) && _systemItems[i].GameObject.activeSelf)
						{
							DeactivateItem(_systemItems[i]);
						}
						else if (itemContainer?.Item != null)
						{
							UIEntity itemEntity;
							if (Director.TryGetEntity(itemContainer.Item.Value, out itemEntity)
								&& itemContainer.Item.Value != (_systemItems[i].EntityBehaviour.Entity?.Id ?? -1))
							{
								_systemItems[i].EntityBehaviour.Initialize(itemEntity.EntityBehaviour.Entity);
								_systemItems[i].GameObject.SetActive(true);
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
