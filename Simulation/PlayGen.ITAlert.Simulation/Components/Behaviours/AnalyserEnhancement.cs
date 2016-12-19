using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Properties;
using PlayGen.ITAlert.UI.Common;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{
	[ComponentDependency(typeof(ItemStorage))]
	public class AnalyserEnhancement : Component
	{
		private ItemStorage _itemStorage;

		private AnalysisiTargetItemContainer _itemContainer;

		private ItemActivator _itemActivator;

		public AnalyserEnhancement()
		{
			_itemContainer = new AnalysisiTargetItemContainer();
		}

		public override void Initialize(Entity entity)
		{
			base.Initialize(entity);

			_itemStorage = Entity.GetComponent<ItemStorage>();
			_itemStorage.SetCustomContainer(2, _itemContainer);

			_itemActivator = Entity.GetComponent<ItemActivator>();
		}

		public bool CanActivate => _itemContainer.HasItem && _itemContainer.Item.TestComponent<EntityTypeProperty>(et => et.Value == EntityType.Npc) && _itemContainer.Item.HasComponent<MalwareGenome>();

		//public void Activate()
		//{
		//	if (CanActivate)
		//	{
		//		//TODO: timer
		//		_itemStorage.TryAddItem()
		//	}
		//}
	}

	public class AnalysisiTargetItemContainer : ItemContainer
	{
		public override string SpriteName => SpriteConstants.ItemContainerAnalysisSample;

		public override bool CanPickup { get; }
	}

	public class AnalysisiActivatorItemContainer : ItemContainer
	{
		public override string SpriteName => SpriteConstants.ItemContainerAnalysisSample;

		public override bool CanPickup { get; }
	}
}
