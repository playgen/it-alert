using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Serialization;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class NodeSequence
	{
		private readonly NodeSequenceEnumerator _enumerator;

		public NodeSequence(EntityConfig[] sequence, bool cyclical = true)
		{
			_enumerator = new NodeSequenceEnumerator(sequence, cyclical);
		}

		public bool TryGetNext(out EntityConfig next)
		{
			if (_enumerator.MoveNext())
			{
				next = _enumerator.Current;
				return true;
			}
			next = null;
			return false;
		}

		private class NodeSequenceEnumerator : IEnumerator<EntityConfig>
		{
			private readonly EntityConfig[] _sequence;

			private readonly bool _cyclical;

			private int _currentIndex = -1;

			private EntityConfig _current;

			public NodeSequenceEnumerator(EntityConfig[] sequence, bool cyclical)
			{
				_sequence = sequence;
				_cyclical = cyclical;
			}

			public void Dispose()
			{
				// nothing to do
			}

			public bool MoveNext()
			{
				_currentIndex = _cyclical
					? _currentIndex + 1 % _sequence.Length
					: _currentIndex + 1;

				if (_currentIndex >= _sequence.Length)
				{
					return false;
				}
				else
				{
					_current = _sequence[_currentIndex];
				}
				return true;
			}

			public void Reset()
			{
				_currentIndex = -1;
			}

			public EntityConfig Current => _current;

			object IEnumerator.Current => _current;
		}
	}
}
