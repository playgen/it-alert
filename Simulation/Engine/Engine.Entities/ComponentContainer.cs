using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Core.Components;

namespace Engine.Entities
{
	public class ComponentContainer : ComponentContainer<IComponent>, IComponentContainer
	{
		public static ComponentContainer Default => new ComponentContainer();
	}

	public class ComponentContainer<TComponent> : IComponentContainer<TComponent>
		where TComponent : IComponent
	{
		// ReSharper disable FieldCanBeMadeReadOnly.Local
		private HashSet<TComponent> _components;

		private Dictionary<Type, IEnumerable<TComponent>> _componentsByImplementation = new Dictionary<Type, IEnumerable<TComponent>>();
		// ReSharper restore FieldCanBeMadeReadOnly.Local

		private bool _disposed;

		protected IList<TComponent> Components => _components.ToList();

		#region constructors

		public ComponentContainer()
		{
			_components = new HashSet<TComponent>();
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

		public void AddComponent(TComponent component)
		{
			_components.Add(component);
		}
		public bool HasComponent<TComponentInterface>()
			where TComponentInterface : class, TComponent
		{
			return GetComponentsInternal<TComponentInterface>().Any();
		}

		public TComponentInterface GetComponent<TComponentInterface>()
			where TComponentInterface : class, TComponent
		{
			return GetComponentsInternal<TComponentInterface>().Single();
		}

		public IEnumerable<TComponentInterface> GetComponents<TComponentInterface>()
			where TComponentInterface : class, TComponent
		{
			return GetComponentsInternal<TComponentInterface>();
		}

		private IEnumerable<TComponentInterface> GetComponentsInternal<TComponentInterface>()
			where TComponentInterface : class, TComponent
		{
			IEnumerable<TComponent> components;
			if (_componentsByImplementation.TryGetValue(typeof(TComponentInterface), out components) == false)
			{
				components = GenerateComponentImplementorCache<TComponentInterface>();
				_componentsByImplementation.Add(typeof(TComponentInterface), components);
			}
			return components.Cast<TComponentInterface>();
		}

		public bool HasComponentsImplmenting<TComponentInterface>()
			where TComponentInterface : class, TComponent
		{
			return GetComponents<TComponentInterface>().Any();
		}

		private IEnumerable<TComponent> GenerateComponentImplementorCache<TComponentInterface>()
		{
			return _components.Where(t => t is TComponentInterface);
		}

		#endregion
	}

}

