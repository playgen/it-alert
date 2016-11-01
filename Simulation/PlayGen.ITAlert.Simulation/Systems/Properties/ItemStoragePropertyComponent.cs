using Engine.Components;
using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
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
