using Engine.Core.Components;

namespace Engine.Components.Property
{
	public interface IPropertyComponent : IComponent
	{
		string PropertyName { get; }

		bool IncludeInState { get; }
	}


	public interface IReadOnlyPropertyComponent<out TPropertyType> : IPropertyComponent
	{
		TPropertyType Value { get; }
	}

	public interface IPropertyComponent<TPropertyType> : IReadOnlyPropertyComponent<TPropertyType>
	{
		void Set(TPropertyType value);
	}
}
