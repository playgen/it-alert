using System;
using Engine.Components.Messaging;
using Engine.Core.Components;
using Engine.Core.Messaging;
using Engine.Core.Util;

namespace Engine.Components
{
	public abstract class ComponentBase : DelegatingObserver, IComponent
	{
		protected IComponentContainer Container { get; private set; }

		#region constructor

		protected ComponentBase(IComponentContainer container)
		{
			NotNullHelper.ArgumentNotNull(container, nameof(container));
			Container = container;
		}

		#endregion

		#region disposal

		~ComponentBase()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
		}

		#endregion
	}
}
