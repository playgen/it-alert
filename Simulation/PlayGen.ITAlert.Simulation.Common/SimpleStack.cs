using System.Collections.Generic;
using System.Linq;
using Engine.Core.Serialization;

namespace PlayGen.ITAlert.Simulation.Common
{
	/// <summary>
	/// Non iterable stack, for intent serialization
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SimpleStack<T>
	{
		[SyncState(StateLevel.Full)]
		public List<T> Items { get; private set;  } = new List<T>();

		public int Count => Items.Count;

		public bool Any()
		{
			return Count > 0;
		}

		public void Clear()
		{
			Items.Clear();
		}

		public void Push(T item)
		{
			Items.Add(item);
		}

		public T Pop()
		{
			var item = Items.Last();
			Items.RemoveAt(Items.Count - 1);
			return item;
		}

		public T Peek()
		{
			return Items.Last();
		}
	}
}
