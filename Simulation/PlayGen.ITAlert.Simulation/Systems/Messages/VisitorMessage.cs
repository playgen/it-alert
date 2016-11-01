using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Messaging;
using PlayGen.ITAlert.Simulation.Visitors;

namespace PlayGen.ITAlert.Simulation.Systems.Messages
{
	public class VisitorEnteringNodeMessage : IMessage
	{
		public IVisitor Visitor { get; private set; }

		public INode Source { get; private set; }

		public int MovementOverflow { get; private set; }

		public VisitorEnteringNodeMessage(IVisitor visitor, INode source, int movementOverflow)
		{
			Visitor = visitor;
			Source = source;
			MovementOverflow = movementOverflow;
		}
	}

	public class VisitorEnteredNodeMessage : IMessage
	{
		public IVisitor Visitor { get; private set; }

		public INode Node { get; private set; }

		public VisitorEnteredNodeMessage(IVisitor visitor, INode node)
		{
			Visitor = visitor;
			Node = node;
		}
	}

	public class VisitorLeavingNodeMessage : NodeVisitorMessage
	{


		public VisitorLeavingNodeMessage(IVisitor visitor, INode source) 
			: base(visitor, source)
		{
		}
	}

	public class VisitorLeftNodeMessage : NodeVisitorMessage
	{
		public VisitorLeftNodeMessage(IVisitor visitor, INode source) 
			: base(visitor, source)
		{
		}
	}

}
