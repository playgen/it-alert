using System;
using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Systems;

namespace PlayGen.ITAlert.Simulation.Visitors
{
	public abstract class Visitor<TState> : ITAlertEntity<TState>, IVisitor
		where TState : ITAlertEntityState
	{
		[SyncState(StateLevel.Differential)]
		public INode CurrentNode { get; protected set; }

		#region constructors

		protected Visitor(ISimulation simulation, EntityType entityType)
			: base(simulation, entityType)
		{

		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected Visitor()
		{
			
		}

		protected override void OnTick()
		{
			ForEachComponentImplementing<IVisitorComponent>(component => component.OnTick(CurrentNode));
		}

		#endregion

		public virtual void OnEnterNode(INode current)
		{
			CurrentNode = current;
			ForEachComponentImplementing<IVisitorComponent>(component => component.OnEnterNode(current));
		}

		public virtual void OnExitNode(INode current)
		{
			CurrentNode = null;
			// do nothing, log maybe
			ForEachComponentImplementing<IVisitorComponent>(component => component.OnExitNode(current));
		}
	}
}
