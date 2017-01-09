using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Activation
{
	public interface IActivation : IComponent
	{
		bool CanActivate { get; }

		bool CanDeactivate { get; }

		bool IsActive { get; }

		void Activate();

		void Deactivate();

	}
}
