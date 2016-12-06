using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using Engine.Messaging;

namespace PlayGen.ITAlert.Simulation.Components.Messages
{
	public class VisitorEnteringNodeMessage : Message
	{
		public Entity Visitor { get; private set; }

		public Entity Source { get; private set; }

		public int MovementOverflow { get; private set; }

		public VisitorEnteringNodeMessage(MessageScope scope, Entity visitor, Entity source, int movementOverflow) 
			: base(scope)
		{
			Visitor = visitor;
			Source = source;
			MovementOverflow = movementOverflow;
		}
	}

	public class VisitorEnteredNodeMessage : Message
	{
		public Entity Visitor { get; private set; }

		public Entity Node { get; private set; }

		public VisitorEnteredNodeMessage(MessageScope scope, Entity visitor, Entity node) 
			: base(scope)
		{
			Visitor = visitor;
			Node = node;
		}
	}

	public class VisitorLeavingNodeMessage : Message
	{
		public Entity Visitor { get; private set; }

		public Entity Node { get; private set; }

		public VisitorLeavingNodeMessage(MessageScope scope, Entity visitor, Entity node) 
			: base(scope)
		{
			Visitor = visitor;
			Node = node;
		}
	}

	public class VisitorLeftNodeMessage : Message
	{
		public Entity Visitor { get; private set; }

		public Entity Node { get; private set; }

		public VisitorLeftNodeMessage(MessageScope scope, Entity visitor, Entity node) 
			: base(scope)
		{
			Visitor = visitor;
			Node = node;
		}
	}

}
