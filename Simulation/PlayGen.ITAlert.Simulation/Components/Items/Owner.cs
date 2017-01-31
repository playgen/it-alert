using System.ComponentModel;
using IComponent = Engine.Components.IComponent;

namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public class Owner : IComponent
	{
		[DefaultValue(-1)]
		public int? Value { get; set; }
	}
}
