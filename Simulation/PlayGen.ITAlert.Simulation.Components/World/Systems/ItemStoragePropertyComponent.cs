using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Property;
using PlayGen.ITAlert.Simulation.Entities.Visitors.Items;
using PlayGen.ITAlert.Simulation.Entities.World.Systems;

namespace PlayGen.ITAlert.Simulation.Components.World.Systems
{
	public class ItemStoragePropertyComponent : ComponentBase, IPropertyComponent
	{
		public enum OverLimitBehaviour
		{
			Dispose,
			Lock,
		}

		public string PropertyName { get; }
		public bool IncludeInState { get; }

		public ItemContainer[] Items { get; private set; }

		public int ItemLimit { get; set; }

		private readonly OverLimitBehaviour _overLimitBehaviour;

		public ItemStoragePropertyComponent(IComponentContainer container, int maxItems, int itemLimit, OverLimitBehaviour overLimitBehaviour) 
			: base(container)
		{
			PropertyName = "Items";
			IncludeInState = true;

			ItemLimit = itemLimit;
			Items = new ItemContainer[maxItems];
			_overLimitBehaviour = overLimitBehaviour;
		}

		public void SetItemLimit(int limit)
		{
			if (limit < ItemLimit)
			{
				for (var i = Items.Length - 1; i >= ItemLimit; i--)
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
				for (var i = Items.Length - 1; i >= ItemLimit; i--)
				{
					var itemContainer = Items[i];
					itemContainer.Enabled = true;
				}
			}
			ItemLimit = limit;
		}
	}
}
