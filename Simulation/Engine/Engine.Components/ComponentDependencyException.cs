using System;
using Engine.Core.Exceptions;

namespace Engine.Components
{
	public class ComponentDependencyException : EngineException
	{
		public Type EntityType { get; }

		public Type ComponentType { get; }

		public Type DependencyType { get; }

		public ComponentDependencyException(Type entityType, Type componentType, Type dependencyType)
			// ReSharper disable once UseStringInterpolation
			: base (string.Format("Component dependency not satisfied. EntityType: {0}, ComponentType: {1}, DependencyType: {2}", entityType.AssemblyQualifiedName, componentType.AssemblyQualifiedName, dependencyType.AssemblyQualifiedName))
		{
			EntityType = entityType;
			ComponentType = componentType;
			DependencyType = dependencyType;
		}
	}
}
