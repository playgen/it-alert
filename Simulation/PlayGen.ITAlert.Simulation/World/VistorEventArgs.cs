using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Interfaces;

namespace PlayGen.ITAlert.Simulation.World
{
	public class VistorEventArgs : EventArgs
	{
		public IVisitor Visitor { get; }

		public INode Source { get; }

		public VistorEventArgs(IVisitor visitor, INode source)
		{
			Visitor = visitor;
			Source = source;
		}
	}
}
