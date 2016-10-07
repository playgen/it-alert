using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Serialization
{
	internal static class TypeExtensions
	{
		public static bool IsEntity(this Type type)
		{
			return typeof(IEntity).IsAssignableFrom(type);
		}

		/// <summary>
		/// Borrowed from newtonsoft
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsValueType(this Type type)
		{
#if !(DOTNET || PORTABLE)
			return type.IsValueType;
#else
            return type.GetTypeInfo().IsValueType;
#endif
		}

		public static bool IsInterface(this Type type)
		{
#if !(DOTNET || PORTABLE)
			return type.IsInterface;
#else
            return type.GetTypeInfo().IsInterface;
#endif
		}
	}
}
