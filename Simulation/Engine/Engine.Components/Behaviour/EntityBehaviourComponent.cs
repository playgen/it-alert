using Engine.Core.Components;
using Engine.Core.Entities;

namespace Engine.Components.Behaviour
{
	public abstract class EntityBehaviourComponent : EntityBehaviourComponent<IEntity>, IBehaviourComponent
	{
		protected EntityBehaviourComponent(IEntity container) 
			: base(container)
		{
		}
	}

	public abstract class EntityBehaviourComponent<TEntity> : BehaviourComponent, IBehaviourComponent
		where TEntity : IEntity
	{
		protected TEntity Entity { get; private set; }

		protected EntityBehaviourComponent(IEntity container)
			: base(container)
		{
			// TODO: I dont like this pattern, we need to deal with the component constuctor on the base interface though
			Entity = (TEntity)container;
		}
	}

}
