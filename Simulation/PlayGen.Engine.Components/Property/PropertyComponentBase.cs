namespace PlayGen.Engine.Components.Property
{
	public abstract class PropertyComponentBase<TValue> : ComponentBase, IPropertyComponent<TValue>
		where TValue : struct
	{
		public string PropertyName { get; private set; }

		private TValue _value;

		public TValue Value => GetValue();

		protected PropertyComponentBase(IComponentContainer container, string propertyName, TValue value) 
			: base( container)
		{
			PropertyName = propertyName;
			_value = value;
		}

		protected virtual TValue GetValue()
		{
			return _value;
		}

		public virtual void Set(TValue value)
		{
			_value = value;
		}

		public virtual void ApplyDelta(TValue delta)
		{
			
		}
	}
}
