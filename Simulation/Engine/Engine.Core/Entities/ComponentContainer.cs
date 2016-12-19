using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;

namespace Engine.Entities
{
	public class ComponentContainer : IComponentContainer
	{
		public static ComponentContainer Default => new ComponentContainer();

		private readonly HashSet<IComponent> _components;

		public IList<IComponent> Components => _components.ToList();

		private Dictionary<Type, IEnumerable<IComponent>> _componentsByImplementation = new Dictionary<Type, IEnumerable<IComponent>>();
		// ReSharper restore FieldCanBeMadeReadOnly.Local

		private bool _disposed;
		
		#region constructors

		public ComponentContainer()
		{
			_components = new HashSet<IComponent>();
		}

		#endregion

		#region dispose

		~ComponentContainer()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			if (_disposed == false)
			{
				_disposed = true;
				foreach (var component in _components)
				{
					component.Dispose();
				}
			}
		}

		#endregion

		#region components

		public void AddComponent(IComponent component)
		{
			_components.Add(component);
		}
		public bool HasComponent<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			return GetComponentsInternal<TComponentInterface>().Any();
		}

		public TComponentInterface GetComponent<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			return GetComponentsInternal<TComponentInterface>().Single();
		}

		public bool TryGetComponent<TComponentInterface>(out TComponentInterface component)
			where TComponentInterface : class, IComponent
		{
			component = GetComponentsInternal<TComponentInterface>().SingleOrDefault();
			return component != null;
		}

		public IEnumerable<TComponentInterface> GetComponents<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			return GetComponentsInternal<TComponentInterface>();
		}

		private IEnumerable<TComponentInterface> GetComponentsInternal<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			IEnumerable<IComponent> components;
			if (_componentsByImplementation.TryGetValue(typeof(TComponentInterface), out components) == false)
			{
				components = GenerateComponentImplementorCache<TComponentInterface>();
				_componentsByImplementation.Add(typeof(TComponentInterface), components);
			}
			return components.Cast<TComponentInterface>();
		}

		public bool HasComponentsImplmenting<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			return GetComponents<TComponentInterface>().Any();
		}

		private IEnumerable<IComponent> GenerateComponentImplementorCache<TComponentInterface>()
		{
			return _components.Where(t => t is TComponentInterface);
		}

		#endregion
	}

}

