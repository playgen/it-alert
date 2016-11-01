using System;
using Engine.Core.Exceptions;

namespace Engine.Components
{
	public class ComponentUsageException : EngineException
	{
		public Type ComponentType { get; }
		public Type EntityType { get; }

		public ComponentUsageException(Type componentType, Type entityType)
			// ReSharper disable once UseStringInterpolation
			: base (string.Format("Component type {0} cannot be used on Entity type: {1}", componentType.AssemblyQualifiedName, entityType.AssemblyQualifiedName))
		{
			ComponentType = componentType;
			EntityType = entityType;
		}
	}
}
