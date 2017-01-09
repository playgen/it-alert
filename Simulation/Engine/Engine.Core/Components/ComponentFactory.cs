using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;

namespace Engine.Components
{
	public delegate IComponent ComponentFactoryDelegate();

	public delegate void EntityComponentBinding(Entity entity, IComponent component);
	public delegate void EntityArchetypeCreated(Entity entity);

	public class ComponentFactory : IComponentFactory
	{
		private readonly Dictionary<string, bool> _archetypeValidation;

		private readonly Dictionary<string, List<ComponentFactoryDelegate>> _componentFactories;

		public EntityComponentBinding EntityComponentBound;
		public EntityArchetypeCreated EntityArchetypeCreated;

		#region constructor

		public ComponentFactory()
		{
			_archetypeValidation = new Dictionary<string, bool>();
			_componentFactories = new Dictionary<string, List<ComponentFactoryDelegate>>();
		}

		#endregion

		public void AddFactoryMethods(string archetype, IEnumerable<ComponentFactoryDelegate> factoryDelegates)
		{
			factoryDelegates.ToList().ForEach(fd => AddFactoryMethod(archetype, fd));
		}

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

		public void PopulateContainerForArchetype(string archetype, Entity componentContainer)
		{
			List<ComponentFactoryDelegate> factoryDelegates;
			if (_componentFactories.TryGetValue(archetype, out factoryDelegates) == false)
			{
				throw new ComponentLookupException($"Component specification not found for archetype: {archetype}");
			}
			foreach (var factoryDelegate in factoryDelegates)
			{
				var component = factoryDelegate();
				componentContainer.AddComponent(component);
				EntityComponentBound?.Invoke(componentContainer, component);
			}
			EntityArchetypeCreated(componentContainer);
		}

		public void ValidateComponentDependencies(IComponentContainer componentContainer)
		{
			foreach (var archetype in _archetypeValidation.Keys)
			{
				ValidateDependenciesForArchetype(archetype, componentContainer);
			}
		}

		private void ValidateDependenciesForArchetype(string archetype, IComponentContainer componentContainer)
		{
			foreach (var component in componentContainer.Components)
			{
				foreach (var componentDependency in component.GetType().GetComponentDependencyTypes())
				{
					if (componentContainer.Components.Any(c => componentDependency.IsInstanceOfType(c)) == false)
					{
						throw new ComponentDependencyException(archetype, component.GetType(), componentDependency);
					}
				}
			}
		}
	}
}
