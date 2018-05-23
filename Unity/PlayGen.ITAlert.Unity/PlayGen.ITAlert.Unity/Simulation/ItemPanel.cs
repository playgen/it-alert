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
			public ItemContainerBehaviour ContainerBehaviour { get; }

			public ItemContainer ItemContainer { private get; set; }

			private UIEntity _itemEntity { get; }

			private readonly bool _proxyItem;

			private readonly Director _director;

			public ItemPanelContainer(Director director, GameObject go, int containerIndex, ItemContainer itemContainer = null, bool proxyItem = true)
			{
				_director = director;
				ContainerBehaviour = go.GetComponent<ItemContainerBehaviour>();
				ItemContainer = itemContainer;
				if (itemContainer != null)
				{
					ContainerBehaviour.Initialize(ItemContainer, _director, containerIndex, null);
				}

				_itemEntity = new UIEntity(nameof(Item), "ItemPanelProxy", director);
				director.AddUntrackedEntity(_itemEntity);
				_itemEntity.GameObject.transform.SetParent(_director.ItemPanel.transform, false);
			    _itemEntity.GameObject.GetComponent<RectTransform>().anchorMin = ContainerBehaviour.GetComponent<RectTransform>().anchorMin;
                _itemEntity.GameObject.GetComponent<RectTransform>().anchorMax = ContainerBehaviour.GetComponent<RectTransform>().anchorMax;
				_itemEntity.GameObject.GetComponent<RectTransform>().anchoredPosition = ContainerBehaviour.GetComponent<RectTransform>().anchoredPosition;
				_itemEntity.GameObject.GetComponent<RectTransform>().sizeDelta = ContainerBehaviour.GetComponent<RectTransform>().sizeDelta;
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

		private ItemPanelContainer _inventoryItem;

		[SerializeField]
		private Director _director;

		public void Initialize()
		{
			// get player entity item storage component
			if (_director.Player.Entity.TryGetComponent(out ItemStorage itemStorage) == false)
			{
				throw new SimulationIntegrationException("No item storage found on player");
			}
			var inventoryGameObject = GameObjectUtilities.FindGameObject("Game/Canvas/ItemPanel/ItemContainer_Inventory");
			var inventoryItemContainer = itemStorage.Items[0] as InventoryItemContainer;

			_inventoryItem = new ItemPanelContainer(_director, inventoryGameObject, -1, inventoryItemContainer, false);
		}

		public void ExplicitUpdate()
		{
			LogProxy.Info($"ItemPanel ExplicitUpdate: Director Id {_director.InstanceId} player is null: {_director.Player}");

			if (_director.Player != null)
			{
				// TODO: the following shouldnt be necessary when the container reference isnt changed
				if (_director.Player.Entity.TryGetComponent(out ItemStorage itemStorage))
				{
					var inventoryItemContainer = itemStorage.Items[0] as InventoryItemContainer;
					_inventoryItem.ItemContainer = inventoryItemContainer;
					// TODO: keep this when above fixed
					_inventoryItem.Update();
				}
			}
			else
			{
				LogProxy.Error($"Director {_director.InstanceId} Player is unassigned");
			}
		}
	}
}
