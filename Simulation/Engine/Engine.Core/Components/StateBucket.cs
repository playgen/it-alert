using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Components
{
	public class StateBucket
	{
		public int EntityId { get; }

		private readonly Dictionary<Type, IComponentState> _stateDictionary;

		public StateBucket(int entityId)
			: this (entityId, new Dictionary<Type, IComponentState>())
		{
		}

		public StateBucket(int entityId, Dictionary<Type, IComponentState> stateDictionary)
		{
			EntityId = entityId;
			_stateDictionary = stateDictionary;
		}

		public void Add(IComponentState state)
		{
			_stateDictionary.Add(state.GetType(), state);
		}

		public TState Get<TState>() 
			where TState : class, IComponentState
		{
			IComponentState state;
			if (_stateDictionary.TryGetValue(typeof(TState), out state))
			{
				return (TState) state;
			}
			throw new KeyNotFoundException();
		}

		public bool TryGet<TState>(out TState typedState) 
			where TState : class, IComponentState
		{
			typedState = null;
			IComponentState state;
			if (_stateDictionary.TryGetValue(typeof(TState), out state))
			{
				typedState = state as TState;
				return true;
			}
			return false;
		}
	}
}
