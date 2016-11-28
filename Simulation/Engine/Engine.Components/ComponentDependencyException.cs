using System;
using Engine.Core.Exceptions;

namespace Engine.Components
{
	public class ComponentDependencyException : EngineException
	{
		public string Archetype { get; }

		public Type ComponentType { get; }

		public Type DependencyType { get; }

		public ComponentDependencyException(string archetype, Type componentType, Type dependencyType)
			// ReSharper disable once UseStringInterpolation
			: base (string.Format("Component dependency not satisfied. Archetype: {0}, ComponentType: {1}, DependencyType: {2}", archetype, componentType.AssemblyQualifiedName, dependencyType.AssemblyQualifiedName))
		{
			Archetype = archetype;
			ComponentType = componentType;
			DependencyType = dependencyType;
		}
	}
}
