using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Components
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class ComponentDependencyAttribute : Attribute
	{
		public Type RequiredType { get; private set; }

		public ComponentDependencyAttribute(Type requiredType)
		{
			RequiredType = requiredType;
		}
	}
}
