using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Messaging;
using PlayGen.ITAlert.Simulation.Visitors;

namespace PlayGen.ITAlert.Simulation.Systems.Messages
{
	public class VisitorEnteringNodeMessage : Message
	{
		public IVisitor Visitor { get; private set; }

		public INode Source { get; private set; }

		public int MovementOverflow { get; private set; }

		public VisitorEnteringNodeMessage(MessageScope scope, IVisitor visitor, INode source, int movementOverflow) 
			: base(scope)
		{
			Visitor = visitor;
			Source = source;
			MovementOverflow = movementOverflow;
		}
	}

	public class VisitorEnteredNodeMessage : Message
	{
		public IVisitor Visitor { get; private set; }

		public INode Node { get; private set; }

		public VisitorEnteredNodeMessage(MessageScope scope, IVisitor visitor, INode node) 
			: base(scope)
		{
			Visitor = visitor;
			Node = node;
		}
	}

	public class VisitorLeavingNodeMessage : Message
	{
		public IVisitor Visitor { get; private set; }

		public INode Node { get; private set; }

		public VisitorLeavingNodeMessage(MessageScope scope, IVisitor visitor, INode node) 
			: base(scope)
		{
			Visitor = visitor;
			Node = node;
		}
	}

	public class VisitorLeftNodeMessage : Message
	{
		public IVisitor Visitor { get; private set; }

		public INode Node { get; private set; }

		public VisitorLeftNodeMessage(MessageScope scope, IVisitor visitor, INode node) 
			: base(scope)
		{
			Visitor = visitor;
			Node = node;
		}
	}

}
