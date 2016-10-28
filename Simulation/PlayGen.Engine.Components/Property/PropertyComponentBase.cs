namespace PlayGen.Engine.Components.Property
{
	public abstract class ReadOnlyPropertyComponentBase<TValue> : ComponentBase, IReadOnlyPropertyComponent<TValue>
	{
		public string PropertyName { get; private set; }

		public bool IncludeInState { get; private set; }

		// ReSharper disable once InconsistentNaming
		protected TValue _value;

		public TValue Value => GetValue();

		protected ReadOnlyPropertyComponentBase(IComponentContainer container, string propertyName, bool includeInState, TValue value)
			: base(container)
		{
			PropertyName = propertyName;
			IncludeInState = includeInState;
			_value = value;
		}

		protected virtual TValue GetValue()
		{
			return _value;
		}
	}

	public abstract class PropertyComponentBase<TValue> : ReadOnlyPropertyComponentBase<TValue>, IPropertyComponent<TValue>
	{
		protected PropertyComponentBase(IComponentContainer container, string propertyName, bool includeInState, TValue value) 
			: base( container, propertyName, includeInState, value)
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
