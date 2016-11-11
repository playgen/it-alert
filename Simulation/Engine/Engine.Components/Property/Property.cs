using System;
using Engine.Core.Components;
using Engine.Core.Entities;

namespace Engine.Components.Property
{
	public class ReadOnlyProperty<TValue> : Component, IReadOnlyPropertyComponent<TValue>
	{
		// ReSharper disable once InconsistentNaming
		protected TValue _value;

		public TValue Value => GetValue();

		protected Func<TValue> ValueGetter;

		protected ReadOnlyProperty(IEntity entity)
			: base(entity)
		{
			_value = default(TValue);
			ValueGetter = () => _value;
		}

		protected ReadOnlyProperty(IEntity entity, TValue value)
			: base(entity)
		{
			_value = value;
			ValueGetter = () => _value;
		}

		public ReadOnlyProperty(IEntity entity, Func<TValue> valueGetter)
			: base(entity)
		{
			ValueGetter = valueGetter;
		}

		protected virtual TValue GetValue()
		{
			return ValueGetter();
		}
	}

	public abstract class Property<TValue> : ReadOnlyProperty<TValue>, IPropertyComponent<TValue>
	{
		protected Property(IEntity entity) 
			: base(entity)
		{
		}

		protected Property(IEntity entity, TValue value)
			: base(entity, value)
		{
		}

		public virtual void Set(TValue value)
		{
			_value = value;
		}

		//public virtual void ApplyDelta(TValue delta)
		//{
			
		//}
	}
}
