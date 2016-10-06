using System;
using PlayGen.Engine;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Visitors
{
	public abstract class Visitor<TState> : ITAlertEntity<TState>, IVisitor
		where TState : ITAlertEntityState
	{
		[SyncState(StateLevel.Minimal)]
		public INode CurrentNode { get; protected set; }

		#region constructors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="entityType"></param>
		protected Visitor(ISimulation simulation, EntityType entityType)
			: base(simulation, entityType)
		{

		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected Visitor()
		{
			
		}

		#endregion

		public virtual void OnEnterNode(INode current)
		{
			CurrentNode = current;
		}

		public virtual void OnExitNode(INode current)
		{
			CurrentNode = null;
			// do nothing, log maybe
		}
	}
}
