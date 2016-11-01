namespace PlayGen.ITAlert.Simulation.Visitors
{
	public abstract class VisitorBehaviourComponent : BehaviourComponent, IVisitorComponent
	{
		[SyncState(StateLevel.Differential)]
		private int _enterCurrentNodeTick;

		protected VisitorBehaviourComponent(IComponentContainer container) 
			: base(container)
		{
		}

		public virtual void OnEnterNode(INode current)
		{
			_enterCurrentNodeTick = 0;
		}

		public virtual void OnExitNode(INode current)
		{
		}

		public virtual void OnTick(INode currentNode)
		{
		}
	}
}
