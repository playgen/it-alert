using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace Engine.Archetypes
{
	public abstract class ComponentBinding
	{
		public abstract Type ComponentType { get; }
	}

	public class ComponentBinding<TComponent> : ComponentBinding
		where TComponent : IComponent
	{
		public override Type ComponentType => typeof(TComponent);

		public string ComponentTemplateSerialized { get; }

		#region constructors

		public ComponentBinding()
		{
		}

		public ComponentBinding(string componentTemplateSerialized)
		{
			ComponentTemplateSerialized = componentTemplateSerialized;
		}

		#endregion
	}
}
