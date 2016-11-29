using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace Engine.Archetypes
{
	public class Archetype
	{
		public string Name { get; }

		public List<ComponentFactoryDelegate> Components { get; }

		public Archetype(string name)
		{
			Name = name;
			Components = new List<ComponentFactoryDelegate>();
		}
	}

	public static class ArchetypeExtensions
	{
		public static Archetype HasComponent(this Archetype archetype, ComponentFactoryDelegate componentFactoryDelegate)
		{
			archetype.Components.Add(componentFactoryDelegate);
			return archetype;
		}

		public static Archetype HasComponents(this Archetype archetype, IEnumerable<ComponentFactoryDelegate> componentFactoryDelegates)
		{
			archetype.Components.AddRange(componentFactoryDelegates);
			return archetype;
		}

		public static Archetype Extends(this Archetype archetype, Archetype otherArchetype)
		{
			return archetype.HasComponents(otherArchetype.Components);
		}
	}
}
