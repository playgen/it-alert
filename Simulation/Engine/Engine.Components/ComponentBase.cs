using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Util;

namespace PlayGen.Engine.Components
{
	public abstract class ComponentBase : IComponent
	{
		protected IComponentContainer EntityComponents { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="container"></param>
		protected ComponentBase(IComponentContainer container)
		{
			NotNullHelper.ArgumentNotNull(container, nameof(container));
			EntityComponents = container;
		}
	}
}
