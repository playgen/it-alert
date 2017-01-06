using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	//public class ComponentEntityTuple : IEquatable<ComponentEntityTuple>
	//{
	//	public Entity Entity { get; }


	//	public ComponentEntityTuple(Entity entity, Dictionary<Type, IComponent> components)
	//	{
	//		Entity = entity;
	//		Components = components;
	//	}

	//	public bool Equals(ComponentEntityTuple other)
	//	{
	//		return other?.Entity != null
	//				&& Entity.Id == other.Entity.Id;
	//	}
	//}

	//public class ComponentEntityTuple<TComponent1> : IEquatable<ComponentEntityTuple<TComponent1>>
	//	where TComponent1 : IComponent
	//{
	//	public Entity Entity { get; }

	//	public TComponent1 Component1 { get; }

	//	public ComponentEntityTuple(Entity entity, TComponent1 component1)
	//	{
	//		Entity = entity;
	//		Component1 = component1;
	//	}

	//	public bool Equals(ComponentEntityTuple<TComponent1> other)
	//	{
	//		return other?.Entity != null
	//				&& Entity.Id == other.Entity.Id;
	//	}
	//}

	//public class ComponentEntityTuple<TComponent1, TComponent2> : ComponentEntityTuple<TComponent1>,
	//		IEquatable<ComponentEntityTuple<TComponent1, TComponent2>>
	//		where TComponent1 : IComponent
	//		where TComponent2 : IComponent
	//{
	//	public TComponent2 Component2 { get; }

	//	public ComponentEntityTuple(Entity entity, TComponent1 component1, TComponent2 component2)
	//		: base(entity, component1)
	//	{
	//		Component2 = component2;
	//	}

	//	public bool Equals(ComponentEntityTuple<TComponent1, TComponent2> other)
	//	{
	//		return base.Equals(other);
	//	}
	//}

	//public class ComponentEntityTuple<TComponent1, TComponent2, TComponent3> : ComponentEntityTuple<TComponent1, TComponent2>,
	//	IEquatable<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>>
	//	where TComponent1 : IComponent
	//	where TComponent2 : IComponent
	//	where TComponent3 : IComponent
	//{
	//	public TComponent3 Component3 { get; }

	//	public ComponentEntityTuple(Entity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3)
	//		: base(entity, component1, component2)
	//	{
	//		Component3 = component3;
	//	}

	//	public bool Equals(ComponentEntityTuple<TComponent1, TComponent2, TComponent3> other)
	//	{
	//		return base.Equals(other);
	//	}
	//}
}

