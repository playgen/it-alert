using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Core.Components;
using Engine.Core.Entities;

namespace Engine.Components
{
	public delegate IComponent ComponentFactoryDelegate(IEntity container);

	public class ComponentConfiguration
	{
		private readonly Dictionary<string, HashSet<Type>> _componentTypes;

		private readonly Dictionary<string, List<ComponentFactoryDelegate>> _componentFactories;



		#region constructor

		public ComponentConfiguration()
		{
			_componentTypes = new Dictionary<string, HashSet<Type>>();
			_componentFactories = new Dictionary<string, List<ComponentFactoryDelegate>>();
		}

		#endregion

		public void AddFactoryMethod(string archetype, Type componentType, ComponentFactoryDelegate factoryDelegate)
		{
			//if (componentType.IsComponentUsageValid(entityType) == false)
			//{
			//	throw new ComponentUsageException(componentType, entityType);
			//}

			HashSet<Type> componentTypes;
			if (_componentTypes.TryGetValue(archetype, out componentTypes) == false)
			{
				componentTypes = new HashSet<Type>();
				_componentTypes.Add(archetype, componentTypes);
			}
			componentTypes.Add(componentType);

			List<ComponentFactoryDelegate> factoryDelegates;
			if (_componentFactories.TryGetValue(archetype, out factoryDelegates) == false)
			{
				factoryDelegates = new List<ComponentFactoryDelegate>(1);
				_componentFactories.Add(archetype, factoryDelegates);
			}
			factoryDelegates.Add(factoryDelegate);
		}

		//public IComponentContainer GenerateContainerForType(Type entityType)
		//{
		//	if (_componentFactories.ContainsKey(entityType))
		//	{
		//		var componentContainer = new ComponentContainer();
		//		PopulateContainerForType(entityType, componentContainer);
		//		return componentContainer;
		//	}
		//	return ComponentContainer.Default;
		//}

			[Obsolete("This has been replaced with archetype initialization")]
		public void PopulateContainerOfType<TEntity>(TEntity entity)
			where TEntity : IEntity, IComponentContainer
		{
			PopulateContainerForArchetype(typeof(TEntity).Name, entity);
		}

		public void PopulateContainerForArchetype(string archetype, IEntity componentContainer)
		{
			List<ComponentFactoryDelegate> factoryDelegates;
			if (_componentFactories.TryGetValue(archetype, out factoryDelegates))
			{
				foreach (var factoryDelegate in factoryDelegates)
				{
					componentContainer.AddComponent(factoryDelegate(componentContainer));
				}
			}
		}

		public void ValidateComponentDependencies()
		{
			foreach (var archetype in _componentTypes.Keys)
			{
				ValidateDependenciesForArchetype(archetype);
			}
		}

		private void ValidateDependenciesForArchetype(string archetype)
		{
			foreach (var componentType in _componentTypes[archetype])
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
