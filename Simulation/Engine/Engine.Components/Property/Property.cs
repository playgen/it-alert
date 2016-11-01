using System;
using Engine.Core.Components;

namespace Engine.Components.Property
{
	public class ReadOnlyProperty<TValue> : ComponentBase, IReadOnlyPropertyComponent<TValue>
	{
		public string PropertyName { get; private set; }

		public bool IncludeInState { get; private set; }

		// ReSharper disable once InconsistentNaming
		protected TValue _value;

		public TValue Value => GetValue();

		private readonly Func<TValue> _valueGetter;

		protected ReadOnlyProperty(IComponentContainer container, string propertyName, bool includeInState)
			: base(container)
		{
			PropertyName = propertyName;
			IncludeInState = includeInState;
			_value = default(TValue);
			_valueGetter = () => _value;
		}

		protected ReadOnlyProperty(IComponentContainer container, string propertyName, bool includeInState, TValue value)
			: base(container)
		{
			PropertyName = propertyName;
			IncludeInState = includeInState;
			_value = value;
			_valueGetter = () => _value;
		}

		public ReadOnlyProperty(IComponentContainer container, string propertyName, bool includeInState, Func<TValue> valueGetter)
			: base(container)
		{
			PropertyName = propertyName;
			IncludeInState = includeInState;
			_valueGetter = valueGetter;
		}

		protected virtual TValue GetValue()
		{
			return _valueGetter();
		}
	}

	public abstract class Property<TValue> : ReadOnlyProperty<TValue>, IPropertyComponent<TValue>
	{
		protected Property(IComponentContainer container, string propertyName, bool includeInState) 
			: base(container, propertyName, includeInState)
		{
		}

		protected Property(IComponentContainer container, string propertyName, bool includeInState, TValue value)
			: base(container, propertyName, includeInState, value)
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
