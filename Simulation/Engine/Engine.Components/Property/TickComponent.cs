using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components.Behaviour;
using Engine.Core.Components;

namespace Engine.Components.Property
{
	public class TickComponent : ReadOnlyProperty<int>, IBehaviourComponent, ITickableComponent
	{
		protected TickComponent(IComponentContainer container, string propertyName, bool includeInState) 
			: base(container, "CurrentTick", true)
		{

		}

		public virtual void Tick(int currentTick)
		{
			if (_value < currentTick)
			{
				_value = currentTick;
				
			}
		}

		protected override int GetValue()
		{
			return _value;
		}
	}
}
