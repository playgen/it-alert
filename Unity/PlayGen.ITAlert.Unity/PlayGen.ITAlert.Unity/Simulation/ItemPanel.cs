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
		#region item panel container

		private class ItemPanelContainer
		{
			private GameObject GameObject { get; }

			public ItemContainerBehaviour ContainerBehaviour { get; }

			public ItemContainer ItemContainer { private get; set; }

			private UIEntity ItemEntity { get; set; }

			private readonly bool _proxyItem;

			private Director _director;

			public ItemPanelContainer(Director director, GameObject gameObject, ItemContainer itemContainer = null, bool proxyItem = true)
			{
				_director = director;
				GameObject = gameObject;
				ContainerBehaviour = gameObject.GetComponent<ItemContainerBehaviour>();

				ItemContainer = itemContainer;
				if (itemContainer != null)
				{
					ContainerBehaviour.Initialize(ItemContainer, _director);
				}

				if (proxyItem)
				{
					ItemEntity = new UIEntity(nameof(Item), director);
					director.AddUntrackedEntity(ItemEntity);

					ItemEntity.GameObject.SetActive(false);
					ItemEntity.GameObject.transform.localPosition = new Vector3(GameObject.transform.localPosition.x, GameObject.transform.localPosition.y, ItemEntity.GameObject.transform.localPosition.z);

					ContainerBehaviour.SpriteOverride = UIConstants.PanelItemContainerDefaultSpriteName;
				}

				_proxyItem = proxyItem;
			}

			public void Reset()
			{
				
			}


			public void Update()
			{
				ContainerBehaviour.Initialize(ItemContainer, _director);
				UIEntity item;
				if (_proxyItem)
				{
					if (ItemContainer?.Item != null
						&& _director.TryGetEntity(ItemContainer.Item.Value, out item))
					{
						if (ItemEntity.GameObject.activeSelf == false)
						{
							var itemBehaviour = (ItemBehaviour) ItemEntity.EntityBehaviour;
							itemBehaviour.Initialize(item.EntityBehaviour.Entity, _director);
							ItemEntity.GameObject.SetActive(true);
							itemBehaviour.ScaleUp = true;
						}
					}
					else 
					{
						if (ItemEntity.GameObject.activeSelf)
						{
							ItemEntity.GameObject.SetActive(false);
						}
					}
				}
				else
				{
					//TODO: the followingl line is only necessary because the serializer isnt merging components properties when therse are object references
					if (ItemContainer?.Item != null
						&& _director.TryGetEntity(ItemContainer.Item.Value, out item))
					{
						item.GameObject.transform.localPosition = new Vector3(GameObject.transform.localPosition.x, GameObject.transform.localPosition.y, ItemEntity.GameObject.transform.localPosition.z);
						ItemEntity = item;
						var itemBehaviour = (ItemBehaviour)ItemEntity.EntityBehaviour;
						itemBehaviour.ScaleUp = true;
					}					
				}
				ContainerBehaviour.Update();
			}
		}

		#endregion

		private const int ItemCount = SimulationConstants.SubsystemMaxItems;

		private ItemPanelContainer _inventoryItem;

		private readonly ItemPanelContainer[] _systemItems;
		
		private PlayerBehaviour _player;

		private int _playerLocationLast = -1;

		private Director _director;

		public ItemPanel(Director director)
		{
			_director = director;
			_systemItems = new ItemPanelContainer[ItemCount];
		}

		public void Initialize()
		{
			// get player entity item storage component
			_player = _director.Player;
			ItemStorage itemStorage;
			if (_player.Entity.TryGetComponent(out itemStorage) == false)
			{
				throw new SimulationIntegrationException("No item storage found on player");
			}
			var inventoryGameObject = GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/InventoryItemContainer");
			var inventoryItemContainer = itemStorage.Items[0] as InventoryItemContainer;

			_inventoryItem = new ItemPanelContainer(_director, inventoryGameObject, inventoryItemContainer, false);
			_inventoryItem.ContainerBehaviour.ClickEnable = true;
			_inventoryItem.ContainerBehaviour.CanCapture = true;
			_inventoryItem.ContainerBehaviour.Click += InventoryItemContainerBehaviourOnClick;

			for (var i = 0; i < ItemCount; i++)
			{
				var gameObject = GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/ItemContainer" + i);
				_systemItems[i] = new ItemPanelContainer(_director, gameObject);
				_systemItems[i].ContainerBehaviour.ClickEnable = true;
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

			//if (_player.CurrentLocationEntity.Id != _playerLocationLast)
			//{
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
			//}
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
					if (itemContainerBehaviour.TryGetItem(out item) == false
						&& _inventoryItem.ContainerBehaviour.TryGetItem(out item))
					{
						PlayerCommands.DropItem(item.Id);
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



	}
}
