using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Components.Property;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Systems;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class ItemStorageProperty : Component, IPropertyComponent
	{
		public enum OverLimitBehaviour
		{
			Dispose,
			Lock,
		}

		public List<ItemContainer> Items { get; private set; }

		public int ItemLimit { get; set; }

		private OverLimitBehaviour _overLimitBehaviour;

		public ItemStorageProperty() 
			: base()
		{
			Items = new List<ItemContainer>();
		}

		public ItemStorageProperty(int itemLimit, OverLimitBehaviour overLimitBehaviour) 
			: this()
		{
			ItemLimit = itemLimit;
			Items = new List<ItemContainer>();
			SetOverLimitBehaviour(overLimitBehaviour);
		}

		public void SetOverLimitBehaviour(OverLimitBehaviour overLimitBehaviour)
		{
			_overLimitBehaviour = overLimitBehaviour;
		}

		public void SetItemLimit(int limit)
		{
			if (limit < ItemLimit)
			{
				for (var i = Items.Count - 1; i >= ItemLimit; i--)
				{
					var itemContainer = Items[i];
					if (_overLimitBehaviour == OverLimitBehaviour.Dispose)
					{
						if (itemContainer.Item != null)
						{
							itemContainer.Item.Dispose();
							itemContainer.Item = null;
						}
						itemContainer.Enabled = false;
					}
				}
			}
			else if (limit > ItemLimit)
			{
				for (var i = Items.Count - 1; i >= ItemLimit; i--)
				{
					var itemContainer = Items[i];
					itemContainer.Enabled = true;
				}
			}
			ItemLimit = limit;
		}

		#region items 

		///// <summary>
		///// Request an item of specified type if it is available on this subsystem, item will be returned in out parameter if available
		///// </summary>
		///// <param name="itemType">Type of item requested</param>
		///// <param name="requestor"></param>
		///// <param name="item">Item reference if available</param>
		///// <returns>Boolean indiciting whether item was obtained</returns>
		//public bool TryGetItem(ItemType itemType, IEntity requestor, out IEntity item)
		//{
		//	//TODO: probably lock here if we get multiple callers
		//	for (var i = 0; i < ItemLimit; i++)
		//	{
		//		if (TryGetItem(itemType, requestor, i, out item))
		//		{
		//			return true;
		//		}
		//	}
		//	item = null;
		//	return false;
		//}

		///// <summary>
		///// Request an item of specified type if it is available on this subsystem, item will be returned in out parameter if available
		///// </summary>
		///// <param name="itemType">Type of item requested</param>
		///// <param name="itemLocation"></param>
		///// <param name="item">Item reference if available</param>
		///// <param name="requestor"></param>
		///// <returns>Boolean indiciting whether item was obtained</returns>
		//public bool TryGetItem(ItemType itemType, IEntity requestor, int itemLocation, out IItem item)
		//{
		//	var itemContainer = Items[itemLocation];
		//	if (itemContainer != null
		//		&& itemContainer.Item != null
		//		&& itemContainer.Item.ItemType == itemType
		//		&& (itemContainer.Item.HasOwner == false || itemContainer.Item.Owner.Equals(requestor)))
		//	{
		//		item = itemContainer.Item;

		//		return true;
		//	}
		//	item = null;
		//	return false;
		//}

		//public void GetItem(IItem item)
		//{
		//	for (var i = 0; i < ItemLimit; i++)
		//	{
		//		var storedItem = Items[i];
		//		if (item.Equals(storedItem))
		//		{
		//			Items[i] = null;
		//			//TODO: implement with message
		//			//storedItem.OnExitNode(this);
		//		}
		//	}
		//}

		//public bool CanAddItem()
		//{
		//	return Items.Any(ic => ic.HasItem == false && ic.Enabled);
		//}

		//public bool HasItem(IItem item)
		//{
		//	return Items.Any(ic => ic.Item == item);
		//}

		public bool TryAddItem(IEntity item)
		{
			if (item != null)
			{
				for (var i = 0; i < ItemLimit; i++)
				{
					var itemContainer = Items[i];
					if (itemContainer.Item == null)
					{
						Items[i].Item = item;
						//TODO: implement with message
						//item.OnEnterNode(this);
						return true;
					}
				}
			}
			return false;
		}


		#endregion
	}
}
