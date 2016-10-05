using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Interfaces
{
	public interface INode : IEntity
	{
		Dictionary<int, NodeDirection> ExitNodePositions { get; }

		Dictionary<int, NodeDirection> EntranceNodePositions { get; }

		Dictionary<int, VisitorPosition> VisitorPositions { get; }

		void AddVisitor(IVisitor visitor, INode origin, int overflow);

		bool HasVisitor(IVisitor actor);

		bool HasVisitorOfType<TVisitor>() where TVisitor : class, IVisitor;

		TVisitor GetVisitorOfType<TVisitor>() where TVisitor : class, IVisitor;

	}
}

