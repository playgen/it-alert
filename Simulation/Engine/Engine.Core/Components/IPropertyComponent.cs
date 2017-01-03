namespace Engine.Components.Property
{
	public interface IPropertyComponent : IComponent
	{
	}


	public interface IReadOnlyPropertyComponent<out TPropertyType> : IPropertyComponent
	{
		TPropertyType Value { get; }
	}

	public interface IPropertyComponent<TPropertyType> : IReadOnlyPropertyComponent<TPropertyType>
	{
		void SetValue(TPropertyType value);
	}
}
