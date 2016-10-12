using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Components
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class ComponentUsageAttribute : Attribute
	{
		public Type RequiredType { get; private set; }

		public ComponentUsageAttribute(Type requiredType)
		{
			RequiredType = requiredType;
		}
	}
}
