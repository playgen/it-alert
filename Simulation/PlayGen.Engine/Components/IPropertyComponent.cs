using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Components
{
	public interface IPropertyComponent : IComponent
	{

	}

	public interface IPropertyComponent<TPropertyType> : IPropertyComponent
		where TPropertyType : struct 
	{
		string PropertyName { get; }

		TPropertyType Value { get; }

		void Set(TPropertyType value);

		void ApplyDelta(TPropertyType delta);
	}
}
