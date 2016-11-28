using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Core.Entities;
using Engine.Core.Messaging;

namespace PlayGen.ITAlert.Simulation.Components.Messages
{
	public class VisitorEnteringNodeMessage : Message
	{
		public IEntity Visitor { get; private set; }

		public IEntity Source { get; private set; }

		public int MovementOverflow { get; private set; }

		public VisitorEnteringNodeMessage(MessageScope scope, IEntity visitor, IEntity source, int movementOverflow) 
			: base(scope)
		{
			Visitor = visitor;
			Source = source;
			MovementOverflow = movementOverflow;
		}
	}

	public class VisitorEnteredNodeMessage : Message
	{
		public IEntity Visitor { get; private set; }

		public IEntity Node { get; private set; }

		public VisitorEnteredNodeMessage(MessageScope scope, IEntity visitor, IEntity node) 
			: base(scope)
		{
			Visitor = visitor;
			Node = node;
		}
	}

	public class VisitorLeavingNodeMessage : Message
	{
		public IEntity Visitor { get; private set; }

		public IEntity Node { get; private set; }

		public VisitorLeavingNodeMessage(MessageScope scope, IEntity visitor, IEntity node) 
			: base(scope)
		{
			Visitor = visitor;
			Node = node;
		}
	}

	public class VisitorLeftNodeMessage : Message
	{
		public IEntity Visitor { get; private set; }

		public IEntity Node { get; private set; }

		public VisitorLeftNodeMessage(MessageScope scope, IEntity visitor, IEntity node) 
			: base(scope)
		{
			Visitor = visitor;
			Node = node;
		}
	}

}
