using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Resources
{
	public interface IResource : IComponent
	{
		int Value { get; set; }

		int Maximum { get; }
	}
}
