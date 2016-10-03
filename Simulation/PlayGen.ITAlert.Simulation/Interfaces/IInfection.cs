using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation.Interfaces
{
	public interface IInfection : IVisitor
	{
		bool Visible { get; }

		void SetVisible(bool visible);
	}
}
