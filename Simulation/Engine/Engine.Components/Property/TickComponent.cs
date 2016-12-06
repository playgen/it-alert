using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Engine.Components.Behaviour;
using Engine.Entities;
using Engine.Messaging;

namespace Engine.Components.Property
{
	public class TickState
	{
		public int CurrentTick { get; }

		public TickState(int currentTick)
		{
			CurrentTick = currentTick;
		}
	}

	public class TickComponent : ReadOnlyProperty<int>, IBehaviourComponent, ITickableComponent, IEmitState
	{
		public virtual void Tick(int currentTick)
		{
			if (_value < currentTick)
			{
				_value = currentTick;
			
				OnNext(new TickMessage(currentTick));
			}
		}

		protected override int GetValue()
		{
			return _value;
		}

		public object GetState()
		{
			return new TickState(_value);
		}
	}
}
