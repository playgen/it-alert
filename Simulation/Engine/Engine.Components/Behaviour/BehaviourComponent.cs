using Engine.Core.Components;

namespace Engine.Components.Behaviour
{
	public abstract class BehaviourComponent : ComponentBase, IBehaviourComponent
	{
		protected BehaviourComponent(IComponentContainer container) 
			: base(container)
		{
		}
	}
}
