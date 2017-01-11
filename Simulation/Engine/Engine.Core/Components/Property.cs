﻿using System;

namespace Engine.Components
{
	public class ReadOnlyProperty<TValue> : IComponent
	{
		// ReSharper disable once InconsistentNaming
		protected TValue _value;

		public TValue Value => GetValue();

		protected Func<TValue> ValueGetter;

		protected ReadOnlyProperty()
			: base()
		{
			_value = default(TValue);
			ValueGetter = () => _value;
		}

		protected ReadOnlyProperty(TValue value)
			: base()
		{
			_value = value;
			ValueGetter = () => _value;
		}

		public ReadOnlyProperty(Func<TValue> valueGetter)
			: base()
		{
			ValueGetter = valueGetter;
		}

		protected virtual TValue GetValue()
		{
			return ValueGetter();
		}
	}

	public abstract class Property<TValue> : ReadOnlyProperty<TValue>
	{
		protected Property() 
			: base()
		{
		}

		protected Property(TValue value)
			: base(value)
		{
		}

		public virtual void SetValue(TValue value)
		{
			_value = value;
		}

		//public virtual void ApplyDelta(TValue delta)
		//{
			
		//}
	}
}
