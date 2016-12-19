using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
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
