using System.Collections.Generic;
using System.Linq;

namespace PlayGen.ITAlert.Simulation.Common
{
	/// <summary>
	/// Non iterable stack, for intent serialization
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SimpleStack<T>
	{
		private readonly List<T> _items = new List<T>();

		public int Count => _items.Count;

		public bool Any()
		{
			return Count > 0;
		}

		public void Clear()
		{
			_items.Clear();
		}

		public void Set(IEnumerable<T> items)
		{
			_items.Clear();
			_items.AddRange(items);
		}

		public void Push(T item)
		{
			_items.Add(item);
		}

		public T Pop()
		{
			var item = _items.Last();
			_items.RemoveAt(_items.Count - 1);
			return item;
		}

		public T Peek()
		{
			return _items.Last();
		}

		public bool TryPeek(out T last)
		{
			if (_items.Count > 0)
			{
				last = Peek();
				return true;
			}
			last = default(T);
			return false;
		}
	}
}
