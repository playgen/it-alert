using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Components
{
	public abstract class ComponentContainer : ComponentContainer<IComponent>, IComponentContainer
	{

		#region constructors

		protected ComponentContainer()
		{
		}

		protected ComponentContainer(IEnumerable<IComponent> components)
			: base (components)
		{
		}

		#endregion

	}

	public abstract class ComponentContainer<TComponent> : IComponentContainer<TComponent>
		where TComponent : IComponent
	{
		private Dictionary<Type, TComponent> _components;

		private Dictionary<Type, IEnumerable<TComponent>> _componentsByImplementation = new Dictionary<Type, IEnumerable<TComponent>>();

		#region constructors

		protected ComponentContainer()
		{
			_components = new Dictionary<Type, TComponent>();
		}

		protected ComponentContainer(IEnumerable<TComponent> components)
		{
			_components = components.ToDictionary(k => k.GetType(), v => v);
		}

		#endregion

		#region components

		public TConcreteComponent GetComponent<TConcreteComponent>()
			where TConcreteComponent : class, TComponent
		{
			TComponent component;
			if (_components.TryGetValue(typeof(TConcreteComponent), out component))
			{
				return component as TConcreteComponent;
			}
			return null;
		}

		public bool TryGetComponent<TConcreteComponent>(out TConcreteComponent tComponent)
			where TConcreteComponent : class, TComponent
		{
			TComponent component;
			if (_components.TryGetValue(typeof(TConcreteComponent), out component))
			{
				tComponent = component as TConcreteComponent;
				return tComponent != null;
			}
			tComponent = null;
			return false;
		}
		
		public IEnumerable<TComponentInterface> GetComponentsImplmenting<TComponentInterface>()
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

		private IEnumerable<TComponent> GenerateComponentImplementorCache<TComponentInterface>()
		{
			var implementors = _components.Keys.Where(t => typeof(TComponentInterface).IsAssignableFrom(t));
			return implementors.Select(i => _components[i]);
		}

		#endregion
	}

}

