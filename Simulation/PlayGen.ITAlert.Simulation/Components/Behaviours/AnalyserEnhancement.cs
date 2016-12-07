﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{
	[ComponentDependency(typeof(ItemStorage))]
	public class AnalyserEnhancement : Component
	{
		private ItemStorage _itemStorage;

		public AnalyserEnhancement()
		{
		}

		public override void Initialize(Entity entity)
		{
			base.Initialize(entity);
			_itemStorage = Entity.GetComponent<ItemStorage>();
		}
	}
}