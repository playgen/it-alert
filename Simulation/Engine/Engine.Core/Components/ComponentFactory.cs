using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;

namespace Engine.Components
{
	public delegate IComponent ComponentFactoryDelegate(IEntity container);

	public class ComponentFactory
	{
		private readonly Dictionary<string, bool> _archetypeValidation;

		private readonly Dictionary<string, List<ComponentFactoryDelegate>> _componentFactories;

		#region constructor

		public ComponentFactory()
		{
			_archetypeValidation = new Dictionary<string, bool>();
			_componentFactories = new Dictionary<string, List<ComponentFactoryDelegate>>();
		}

		#endregion

		public void AddFactoryMethod(string archetype, ComponentFactoryDelegate factoryDelegate)
		{
			List<ComponentFactoryDelegate> factoryDelegates;
			if (_componentFactories.TryGetValue(archetype, out factoryDelegates) == false)
			{
				factoryDelegates = new List<ComponentFactoryDelegate>(1);
				_componentFactories.Add(archetype, factoryDelegates);
			}
			factoryDelegates.Add(factoryDelegate);
		}

		public void PopulateContainerForArchetype(string archetype, IComponentContainer componentContainer)
		{
			List<ComponentFactoryDelegate> factoryDelegates;
			if (_componentFactories.TryGetValue(archetype, out factoryDelegates) == false)
			{
				throw new ComponentLookupException($"Component specification not found for archetype: {archetype}");
			}
			foreach (var factoryDelegate in factoryDelegates)
			{
				componentContainer.AddComponent(factoryDelegate(componentContainer));
			}

		}

		public void ValidateComponentDependencies()
		{
			foreach (var archetype in _archetypeValidation.Keys)
			{
				
				ValidateDependenciesForArchetype(archetype);
			}
		}

		private void ValidateDependenciesForArchetype(string archetype, IComponentContainer componentContainer)
		{
			foreach (var component in componentContainer.)
			{
				foreach (var componentDependency in componentType.GetComponentDependencyTypes())
				{
					if (_componentTypes[archetype].Contains(componentDependency) == false && _componentTypes[archetype].Any(type => componentDependency.IsAssignableFrom(type)))
					{
						throw new ComponentDependencyException(archetype, componentType, componentDependency);
					}
				}
			}
		}
	}
}
