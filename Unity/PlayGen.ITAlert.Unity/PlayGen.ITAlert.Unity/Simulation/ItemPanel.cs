using System.Collections.Generic;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;

using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation
{
	public class ItemPanel
	{
		private GameObject _activeItemContainer;

		private GameObject _inventoryItemContainer;

		private List<GameObject> _itemContainers;

		public ItemPanel()
		{
			_activeItemContainer = GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/ActiveItemContainer");

			_inventoryItemContainer = GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/InventoryItemContainer");

			_itemContainers = new List<GameObject>();
			var foundItemContainer = true;

			while (foundItemContainer)
			{
				try
				{
					_itemContainers.Add(GameObjectUtilities.FindGameObject("Game/Graph/ItemPanel/ItemContainer" + _itemContainers.Count));
				}
				catch
				{
					foundItemContainer = false;
				}
			}
		}
	}
}
