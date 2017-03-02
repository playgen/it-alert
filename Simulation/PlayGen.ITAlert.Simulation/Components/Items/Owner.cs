using System.ComponentModel;
using IComponent = Engine.Components.IComponent;

namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public class Owner : IComponent
	{
		public int? Value { get; set; }

		public bool AllowAll { get; set; }
	}
}
