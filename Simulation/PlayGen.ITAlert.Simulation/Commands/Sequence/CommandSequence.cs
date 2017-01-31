using System.Linq;
using Engine.Commands;

namespace PlayGen.ITAlert.Simulation.Commands.Sequence
{
	public class CommandSequence
	{
		private readonly CommandSequenceEntry[] _sequence;
		private int _currentIndex;
		private int _currentTick;

		public bool HasPendingCommands => _currentIndex < _sequence.Length;

		public CommandSequence(CommandSequenceEntry[] sequence)
		{
			_sequence = sequence.OrderBy(e => e.Tick).ToArray();
		}

		public ICommand[] Tick()
		{
			ICommand[] commands = null;
			_currentTick++;

			if (_currentIndex < _sequence.Length && _sequence[_currentIndex].Tick == _currentTick)
			{
				commands = _sequence[_currentIndex].Commands;
				_currentIndex++;
			}

			return commands;
		}
	}
}
