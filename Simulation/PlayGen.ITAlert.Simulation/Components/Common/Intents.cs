using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.Common
{
	public class IntentsProperty : IComponent
	{
		//TODO: make this a reference to a more memory efficient storage pool
		private readonly SimpleStack<IIntent> _intents;

		public IntentsProperty() 
		{
			_intents = new SimpleStack<IIntent>();
		}

		public bool HasIntents => _intents.Any();


		public void Enqueue(IIntent intent)
		{
			_intents.Push(intent);
		}

		public void Replace(IIntent intent)
		{
			_intents.Clear();
			Enqueue(intent);
		}

		public IIntent Pop()
		{
			return _intents.Pop();
		}

		public bool TryPeek(out IIntent last)
		{
			return _intents.TryPeek(out last);
		}

		public bool TryPeekIntent<TIntent>(out TIntent last) 
			where TIntent : class, IIntent
		{
			IIntent intent;
			if (TryPeek(out intent))
			{
				last = intent as TIntent;
				return last != null;
			}
			last = null;
			return false;
		}
	}
}
