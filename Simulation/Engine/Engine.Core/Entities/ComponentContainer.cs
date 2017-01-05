using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;

namespace Engine.Entities
{
	public class ComponentContainer : IComponentContainer
	{
		protected ComponentRegistry ComponentRegistry { get; }

		public Dictionary<Type, IComponent> Components { get; }

		private bool _disposed;
		
		#region constructors

		public ComponentContainer()
		{
			Components = new Dictionary<Type, IComponent>();
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
				foreach (var component in Components.Values)
				{
					component.Dispose();
				}
			}
		}

		#endregion

		#region components

		//TODO: there has to be  better way of storing what type of component this entity has 

		public void AddComponent(IComponent component)
		{
			Components.Add(component.GetType(), component);
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

		//public IComponent GetComponent(Type componentType)
		//{
		//	return GetComponentsInternal<com>().Single();
		//}

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

		//private IEnumerable<IComponent> GetComponentsInternal(Type componentType)
		//{
		//	//TODO: this looks like a m * n problem
		//	HashSet<Type> componentTypes;
		//	if (ComponentRegistry.ComponentTypeImplementations.TryGetValue(componentType, out componentTypes))
		//	{
		//		return componentTypes.Where(ct => Components.ContainsKey(ct))
		//			.Select(ct => Components[ct]);
		//	}
		//	return new IComponent[0];
		//}

		private IEnumerable<TComponentInterface> GetComponentsInternal<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			//TODO: this looks like a m * n problem
			HashSet<Type> componentTypes;
			if (ComponentRegistry.ComponentTypeImplementations.TryGetValue(typeof(TComponentInterface), out componentTypes))
			{ 
				return componentTypes.Where(ct => Components.ContainsKey(ct))
					.Select(ct => Components[ct])
					.Cast<TComponentInterface>();
			}
			return new TComponentInterface[0];
		}

		public bool HasComponentsImplmenting<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			return GetComponents<TComponentInterface>().Any();
		}

		#endregion
	}

}

