using System.Collections.Generic;
using PlayGen.ITAlert.Simulation.Entities.Visitors;

namespace PlayGen.ITAlert.Simulation.Entities.World
{
	public interface INode : IITAlertEntity
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

