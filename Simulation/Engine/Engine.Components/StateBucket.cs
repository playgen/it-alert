using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Components
{
	public class StateBucket
	{
		private readonly Dictionary<Type, object> _stateDictionary;

		public StateBucket(Dictionary<Type, object> stateDictionary)
		{
			_stateDictionary = stateDictionary;
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
	}
}
