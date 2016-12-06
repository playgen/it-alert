using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Components
{
	public class StateBucket
	{
		private readonly Dictionary<Type, object> _stateDictionary;

		public StateBucket()
			: this (new Dictionary<Type, object>())
		{
		}

		public StateBucket(Dictionary<Type, object> stateDictionary)
		{
			_stateDictionary = stateDictionary;
		}

		public void Add(object state)
		{
			_stateDictionary.Add(state.GetType(), state);
		}

		public TState Get<TState>()
		{
			object state;
			if (_stateDictionary.TryGetValue(typeof(TState), out state))
			{
				return (TState) state;
			}
			throw new KeyNotFoundException();
		}
		public bool TryGet<TState>(out TState typedState) where TState : class
		{
			typedState = null;
			object state;
			if (_stateDictionary.TryGetValue(typeof(TState), out state))
			{
				typedState = state as TState;
				return true;
			}
			return false;
		}
	}
}
