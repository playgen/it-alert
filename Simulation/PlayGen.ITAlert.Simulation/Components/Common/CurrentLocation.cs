using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Common
{
	public class CurrentLocation : IComponent
	{
		private int? _value;

		public int? Value
		{
			get => _value;
			set
			{
				if (value != _value)
				{
					TicksAtLocation = 0;
				}
				_value = value;
			}
		}

		public int TicksAtLocation { get; private set; }

		public void Tick()
		{
			TicksAtLocation++;
		}
	}
}
