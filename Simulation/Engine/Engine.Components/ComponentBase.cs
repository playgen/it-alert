using Engine.Core.Components;
using Engine.Core.Util;
using Engine.Messaging;
using Engine.Messaging.Extensions;

namespace Engine.Components
{
	public abstract class ComponentBase : IComponent
	{
		protected IComponentContainer Container { get; private set; }

		private DelegatingObserver _observer;

		protected DelegatingObserver Observer => _observer ?? (_observer = new DelegatingObserver().SubscribeTo(Container.MessageHub)); 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="container"></param>
		protected ComponentBase(IComponentContainer container)
		{
			NotNullHelper.ArgumentNotNull(container, nameof(container));
			Container = container;
		}
	}
}
