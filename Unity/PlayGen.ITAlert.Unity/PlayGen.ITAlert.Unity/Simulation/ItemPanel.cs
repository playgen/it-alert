using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;

using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation
{
	public class ItemPanel : MonoBehaviour
	{
		#region item panel container

		private class ItemPanelContainer
		{
			private GameObject GameObject { get; }

			public ItemContainerBehaviour ContainerBehaviour { get; }

			public ItemContainer ItemContainer { private get; set; }

			private UIEntity _itemEntity { get; }

			private readonly bool _proxyItem;

			private readonly Director _director;

			public ItemPanelContainer(Director director, GameObject gameObject, int containerIndex, ItemContainer itemContainer = null, bool proxyItem = true)
			{
				_director = director;
				GameObject = gameObject;
				ContainerBehaviour = gameObject.GetComponent<ItemContainerBehaviour>();
				ItemContainer = itemContainer;
				if (itemContainer != null)
				{
					ContainerBehaviour.Initialize(ItemContainer, _director, containerIndex);
				}

				_itemEntity = new UIEntity(nameof(Item), "ItemPanelProxy", director);
				director.AddUntrackedEntity(_itemEntity);
				_itemEntity.GameObject.transform.SetParent(_director.ItemPanel.transform, false);
			    _itemEntity.GameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(GameObject.transform.localPosition.x, GameObject.transform.localPosition.y, 0);
                _itemEntity.GameObject.GetComponent<RectTransform>().sizeDelta = ((RectTransform)GameObject.transform).rect.size + new Vector2(2, 2);
                _itemEntity.GameObject.AddComponent<ItemDragBehaviour>();
				_itemEntity.GameObject.GetComponent<ItemDragBehaviour>().StartPosition(_itemEntity.GameObject.GetComponent<RectTransform>().anchoredPosition, _director.GetComponentInChildren<Canvas>(true).transform);
			    _itemEntity.GameObject.SetActive(false);

                _proxyItem = proxyItem;
			}

			public void Update()
			{
				ContainerBehaviour.TryUpdate(ItemContainer);

				if (ItemContainer?.Item != null
					&& _director.TryGetEntity(ItemContainer.Item.Value, out var item))
				{
					var itemBehaviour = (ItemBehaviour)_itemEntity.EntityBehaviour;
					if (_itemEntity.GameObject.activeSelf == false)
					{
						_itemEntity.GameObject.SetActive(true);
					}
					if (itemBehaviour.Entity?.Id != item.EntityBehaviour.Entity.Id)
					{
						itemBehaviour.Initialize(item.EntityBehaviour.Entity, _director);
					}
				}
				else
				{
					if (_itemEntity.GameObject.activeSelf)
					{
						_itemEntity.GameObject.SetActive(false);
					}
				}
				if (!_proxyItem)
				{
					//TODO: the following line is only necessary because the serializer isnt merging components properties when therse are object references
					if (ItemContainer?.Item != null
						&& _director.TryGetEntity(ItemContainer.Item.Value, out item))
					{
						item.GameObject.SetActive(false);
					}					
				}
				ContainerBehaviour.Update();
			}
		}

		#endregion

		private const int ItemCount = SimulationConstants.SubsystemMaxItems;

		private ItemPanelContainer _inventoryItem;

		private ItemPanelContainer[] _systemItems;

		[SerializeField]
		private Director _director;

		public void Initialize()
		{
			_systemItems = new ItemPanelContainer[ItemCount];

			// get player entity item storage component
			if (_director.Player.Entity.TryGetComponent(out ItemStorage itemStorage) == false)
			{
				throw new SimulationIntegrationException("No item storage found on player");
			}
			var inventoryGameObject = GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel/ItemContainer_Inventory");
			var inventoryItemContainer = itemStorage.Items[0] as InventoryItemContainer;

			_inventoryItem = new ItemPanelContainer(_director, inventoryGameObject, -1, inventoryItemContainer, false);
			_inventoryItem.ContainerBehaviour.ClickEnable = true;
			_inventoryItem.ContainerBehaviour.Click += InventoryItemContainerBehaviourOnClick;
			_inventoryItem.ContainerBehaviour.Drag += (ic, it, sin) => InventoryItemContainerBehaviourOnDrag(it, sin);

			for (var i = 0; i < ItemCount; i++)
			{
				var go = GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel/ItemContainer_" + i);

				var containerIndex = i;
				_systemItems[i] = new ItemPanelContainer(_director, go, containerIndex);
				_systemItems[i].ContainerBehaviour.ClickEnable = true;
				_systemItems[i].ContainerBehaviour.Click += SystemContainerBehaviourOnClick;
				_systemItems[i].ContainerBehaviour.Drag += (ic, it, sin) => SystemContainerBehaviourOnDrag(it, ic, containerIndex, sin);

				go.GetComponent<ItemContainerBehaviour>().Initialize(_director, i);
			}
		}

		public void ExplicitUpdate()
		{
			LogProxy.Info($"ItemPanel ExplicitUpdate: Director Id {_director.InstanceId} player is null: {_director.Player}");

			if (_director.Player == null)
			{
				throw new SimulationIntegrationException($"Director {_director.InstanceId} Player is unassigned");
			}

			// TODO: the following shouldnt be necessary when the container reference isnt changed
			if (_director.Player.Entity.TryGetComponent(out ItemStorage itemStorage))
			{
				var inventoryItemContainer = itemStorage.Items[0] as InventoryItemContainer;
				_inventoryItem.ItemContainer = inventoryItemContainer;
				// TODO: keep this when above fixed
				_inventoryItem.Update();
			}

			var currentLocation = _director.Player.CurrentLocationEntity;
			var subsystemBehaviour = currentLocation.EntityBehaviour as SubsystemBehaviour;
			if (subsystemBehaviour != null && subsystemBehaviour.ItemStorage != null)
			{
				for (var i = 0; i < ItemCount; i++)
				{
					// the current subsystem has less item containers than the item panel
					_systemItems[i].ItemContainer = i > subsystemBehaviour.ItemStorage.Items.Length - 1 ? null : subsystemBehaviour.ItemStorage.Items[i];
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

		private void SystemContainerBehaviourOnClick(ItemContainerBehaviour itemContainerBehaviour)
		{
			if (itemContainerBehaviour.State == ContainerState.HasItem)
			{
				if (itemContainerBehaviour.TryGetItem(out var item))
				{
					PlayerCommands.ActivateItem(item.Id);
				}
			}
		}

		private void SystemContainerBehaviourOnDrag(ItemBehaviour item, ItemContainerBehaviour itemContainerBehaviour, int destContainerIndex, int sourceContainerIndex)
		{
			if (_inventoryItem.ContainerBehaviour.TryGetItem(out var inventoryItem)
					&& inventoryItem.Id == item.Id)
			{
				if (itemContainerBehaviour.TryGetItem(out var containerItem) == false)
				{
					PlayerCommands.DropItem(item.Id, destContainerIndex);
				}
				else
				{
					PlayerCommands.SwapInventoryItem(containerItem.Id, destContainerIndex, inventoryItem.Id);
				}
			}
			else if (_inventoryItem.ContainerBehaviour != itemContainerBehaviour)
			{
				if (itemContainerBehaviour.TryGetItem(out var containerItem))
				{
					PlayerCommands.SwapSubsystemItem(item.Id, sourceContainerIndex, containerItem.Id, destContainerIndex);
				}
				else
				{
					PlayerCommands.MoveItem(item.Id, sourceContainerIndex, destContainerIndex);
				}
			}
		}

		private void InventoryItemContainerBehaviourOnClick(ItemContainerBehaviour itemContainerBehaviour)
		{
		}

		private void InventoryItemContainerBehaviourOnDrag(ItemBehaviour item, int sourceContainerIndex)
		{
			if (_inventoryItem.ContainerBehaviour.TryGetItem(out var inventoryItem))
			{
				PlayerCommands.SwapInventoryItem(item.Id, sourceContainerIndex, inventoryItem.Id);
			}
			else
			{
				PlayerCommands.PickupItem(item.Id);
			}
		}

	}
}
