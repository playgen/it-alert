using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	public interface IComponentFactory
	{
		void AddFactoryMethod(string archetype, ComponentFactoryDelegate factoryDelegate);
		void AddFactoryMethods(string archetype, IEnumerable<ComponentFactoryDelegate> factoryDelegates);
		void PopulateContainerForArchetype(string archetype, Entity componentContainer);
	}
}
