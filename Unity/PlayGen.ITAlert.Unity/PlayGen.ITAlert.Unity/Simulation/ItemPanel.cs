using System;
using System.Collections.Generic;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
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
		private class ItemPanelContainer
		{
			const float ItemScale = 1.6f;
			const string SortingLayerOverride = "UI";

			private GameObject GameObject { get; }

			public ItemContainerBehaviour ContainerBehaviour { get; }

			public ItemContainer ItemContainer { private get; set; }

			private UIEntity ItemEntity { get; }

			private readonly bool _proxyItem;

			public ItemPanelContainer(GameObject gameObject, ItemContainer itemContainer = null, bool proxyItem = true)
			{
				GameObject = gameObject;
				ContainerBehaviour = gameObject.GetComponent<ItemContainerBehaviour>();

				ItemContainer = itemContainer;
				if (itemContainer != null)
				{
					ContainerBehaviour.Initialize(ItemContainer);
				}

				if (proxyItem)
				{
					ItemEntity = new UIEntity(nameof(Item));
					Director.AddUntrackedEntity(ItemEntity);

					ItemEntity.GameObject.SetActive(false);
					ItemEntity.GameObject.transform.position = GameObject.transform.position;
					ItemEntity.GameObject.GetComponent<SpriteRenderer>().sortingLayerName = SortingLayerOverride;
					ItemEntity.GameObject.transform.localScale = new Vector2(ItemScale, ItemScale);
				}

				_proxyItem = proxyItem;
			}

			public void Update()
			{
				UIEntity item;
				if (_proxyItem)
				{
					if (ItemContainer?.Item != null
						&& Director.TryGetEntity(ItemContainer.Item.Value, out item))
					{
						ContainerBehaviour.Initialize(ItemContainer);
						var itemBehaviour = (ItemBehaviour)ItemEntity.EntityBehaviour;
						itemBehaviour.Initialize(item.EntityBehaviour.Entity);
						ItemEntity.GameObject.SetActive(true);
					}
					else 
					{
						ItemEntity.GameObject.SetActive(false);
					}
				}
				else
				{
					if (ItemContainer?.Item != null)
					{
						//TODO: the followingl line is only necessary because the serializer isnt merging components properties when therse are object references
						ContainerBehaviour.Initialize(ItemContainer);

						if (Director.TryGetEntity(ItemContainer.Item.Value, out item))
						{
							item.GameObject.transform.position = GameObject.transform.position;
						}
					}					
				}
				ContainerBehaviour.Update();
			}
		}

		private const int ItemCount = SimulationConstants.SubsystemMaxItems;

		private ItemPanelContainer _inventoryItem;

		private readonly ItemPanelContainer[] _systemItems;
		
		private PlayerBehaviour _player;

		private int _playerLocationLast = -1;

		public ItemPanel()
		{
			_systemItems = new ItemPanelContainer[ItemCount];
		}

		public void Initialize()
		{
			// get player entity item storage component
			_player = Director.Player;
			ItemStorage itemStorage;
			if (_player.Entity.TryGetComponent(out itemStorage) == false)
			{
				throw new SimulationIntegrationException("No item storage found on player");
			}
			var inventoryGameObject = GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/InventoryItemContainer");
			var inventoryItemContainer = itemStorage.Items[0] as InventoryItemContainer;

			_inventoryItem = new ItemPanelContainer(inventoryGameObject, inventoryItemContainer, false);
			_inventoryItem.ContainerBehaviour.ClickEnable = true;
			_inventoryItem.ContainerBehaviour.CanCapture = true;
			_inventoryItem.ContainerBehaviour.Click += InventoryItemContainerBehaviourOnClick;

			for (var i = 0; i < ItemCount; i++)
			{
				var gameObject = GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/ItemContainer" + i);
				_systemItems[i] = new ItemPanelContainer(gameObject);
				_systemItems[i].ContainerBehaviour.DefaultSprite = UIConstants.PanelItemContainerDefaultSpriteName;
				_systemItems[i].ContainerBehaviour.Click += SystemContaineBehaviourOnClick;
			}
		}

		public void Update()
		{
			if (_player == null)
			{
				throw new SimulationIntegrationException($"Player is unassigned");
			}

			// TODO: the following shouldnt be necessary when the container reference isnt changed
			ItemStorage itemStorage;
			if (_player.Entity.TryGetComponent(out itemStorage))
			{
				var inventoryItemContainer = itemStorage.Items[0] as InventoryItemContainer;
				_inventoryItem.ItemContainer = inventoryItemContainer;
				// TODO: keep this when above fixed
				_inventoryItem.Update();
			}

			if (_player.CurrentLocationEntity.Id != _playerLocationLast)
			{
				_playerLocationLast = _player.CurrentLocationEntity.Id;

				var currentLocation = _player.CurrentLocationEntity;
				var subsystemBehaviour = currentLocation.EntityBehaviour as SubsystemBehaviour;
				if (subsystemBehaviour != null && subsystemBehaviour.ItemStorage != null)
				{
					for (var i = 0; i < ItemCount; i++)
					{
						// the current subsystem has less item containers than the item panel
						if (i > subsystemBehaviour.ItemStorage.Items.Length - 1)
						{
							_systemItems[i].ItemContainer = null;
						}
						else
						{
							_systemItems[i].ItemContainer = subsystemBehaviour.ItemStorage.Items[i];
						}
						_systemItems[i].Update();
					}
				}
				else
				{
					for (var i = 0; i < ItemCount; i++)
					{
						_systemItems[i].ItemContainer = null;
						_systemItems[i].Update();
					}
				}
			}
		}

		private void SystemContaineBehaviourOnClick(ItemContainerBehaviour itemContainerBehaviour)
		{
			ItemBehaviour item;
			switch (_inventoryItem.ContainerBehaviour.State)
			{
				case ContainerState.Capturing:
					if (itemContainerBehaviour.TryGetItem(out item))
					{
						PlayerCommands.PickupItem(item.Id);
					}
					break;
				case ContainerState.Releasing:
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



	}
}
