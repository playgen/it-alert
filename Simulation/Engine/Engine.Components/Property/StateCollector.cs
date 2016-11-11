using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Entities;

namespace Engine.Components.Property
{
	public class StateCollector : ReadOnlyProperty<StateBucket>
	{
		protected StateCollector(IEntity entity) 
			: base(entity)
		{
			ValueGetter = GetStateBucket;

		}

		private StateBucket GetStateBucket()
		{
			var stateDict = Entity.GetComponentsImplmenting<IEmitState>().Select(c => c.GetState()).ToDictionary(k => k.GetType(), v => v);
			return new StateBucket(stateDict);
		}
	}
}
