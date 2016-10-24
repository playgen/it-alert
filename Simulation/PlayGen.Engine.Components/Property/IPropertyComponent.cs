namespace PlayGen.Engine.Components.Property
{
	public interface IPropertyComponent : IComponent
	{

	}

	public interface IPropertyComponent<TPropertyType> : IPropertyComponent
		where TPropertyType : struct 
	{
		string PropertyName { get; }

		bool IncludeInState { get; }

		TPropertyType Value { get; }

		void Set(TPropertyType value);

		void ApplyDelta(TPropertyType delta);
	}
}
