using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Components
{
	public abstract class ComponentBase : IComponent
	{
		public Type[] ComponentDependencies { get; }

		protected ComponentBase(Type[] componentDependencies)
		{
			ComponentDependencies = componentDependencies ?? new Type[0];
		}
	}
}
