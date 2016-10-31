using System;
using PlayGen.ITAlert.Simulation.Entities.Interfaces;
using PlayGen.ITAlert.Simulation.Entities.Visitors;

namespace PlayGen.ITAlert.Simulation.Entities.World
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
