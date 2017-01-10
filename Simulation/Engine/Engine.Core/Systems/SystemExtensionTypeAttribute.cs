using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Systems
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SystemExtensionTypeAttribute : Attribute
	{
		public Type ExtensionType { get; }

		public SystemExtensionTypeAttribute(Type extensionType)
		{
			ExtensionType = extensionType;
		}
	}
}
