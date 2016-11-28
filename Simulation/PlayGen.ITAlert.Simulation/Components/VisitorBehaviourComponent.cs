//using Engine.Components;
//using Engine.Core.Components;
//using Engine.Core.Entities;
//using Engine.Core.Serialization;
//using PlayGen.ITAlert.Simulation.Systems;

//namespace PlayGen.ITAlert.Simulation.VisitorsProperty
//{
//	public abstract class VisitorBehaviourComponent : Component
//	{
//		[SyncState(StateLevel.Differential)]
//		private int _enterCurrentNodeTick;

//		protected VisitorBehaviourComponent(IEntity entity) 
//			: base(entity)
//		{
//		}

//		public virtual void OnEnterNode(INode current)
//		{
//			_enterCurrentNodeTick = 0;
//		}

//		public virtual void OnExitNode(INode current)
//		{
//		}

//		public virtual void OnTick(INode currentNode)
//		{
//		}
//	}
//}
