using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Entities;

namespace PlayGen.Engine.Components
{
	public delegate IComponent ComponentFactoryDelegate(IComponentContainer container);

	public class ComponentConfiguration
	{
		private readonly Dictionary<Type, HashSet<Type>> _componentTypes;

		private readonly Dictionary<Type, List<ComponentFactoryDelegate>> _componentFactories;

		#region constructor

		public ComponentConfiguration()
		{
			_componentTypes = new Dictionary<Type, HashSet<Type>>();
			_componentFactories = new Dictionary<Type, List<ComponentFactoryDelegate>>();
		}

		#endregion

		public void AddFactoryMethod(Type entityType, Type componentType, ComponentFactoryDelegate factoryDelegate)
		{
			if (componentType.IsComponentUsageValid(entityType) == false)
			{
				throw new ComponentUsageException(componentType, entityType);
			}

			HashSet<Type> componentTypes;
			if (_componentTypes.TryGetValue(entityType, out componentTypes) == false)
			{
				componentTypes = new HashSet<Type>();
				_componentTypes.Add(entityType, componentTypes);
			}
			componentTypes.Add(componentType);

			List<ComponentFactoryDelegate> factoryDelegates;
			if (_componentFactories.TryGetValue(entityType, out factoryDelegates) == false)
			{
				factoryDelegates = new List<ComponentFactoryDelegate>(1);
				_componentFactories.Add(entityType, factoryDelegates);
			}
			factoryDelegates.Add(factoryDelegate);
		}

		public IComponentContainer GenerateContainerForType(Type entityType)
		{
			if (_componentFactories.ContainsKey(entityType))
			{
				var componentContainer = new ComponentContainer();
				PopulateContainerForType(entityType, componentContainer);
				return componentContainer;
			}
			return ComponentContainer.Default;
		}

		public void PopulateContainerOfType<TEntity>(TEntity entity)
			where TEntity : IEntity, IComponentContainer
		{
			PopulateContainerForType(typeof(TEntity), entity);
		}

		public void PopulateContainerForType(Type entityType, IComponentContainer componentContainer)
		{
			List<ComponentFactoryDelegate> factoryDelegates;
			if (_componentFactories.TryGetValue(entityType, out factoryDelegates))
			{
				foreach (var factoryDelegate in factoryDelegates)
				{
					componentContainer.AddComponent(factoryDelegate(componentContainer));
				}
			}
		}

		public void ValidateComponentDependencies()
		{
			foreach (var entityType in _componentTypes.Keys)
			{
				ValidateDependenciesForType(entityType);
			}
		}

		private void ValidateDependenciesForType(Type entityType)
		{
			foreach (var componentType in _componentTypes[entityType])
			{
				foreach (var componentDependency in componentType.GetComponentDependencyTypes())
				{
					if (_componentTypes[entityType].Contains(componentDependency) == false && _componentTypes[entityType].Any(type => componentDependency.IsAssignableFrom(type)))
					{
						throw new ComponentDependencyException(entityType, componentType, componentDependency);
					}
				}
			}
		}
	}
}
