using System;
using System.Collections.Generic;
using Engine.Core.Entities;

namespace Engine.Serialization
{
	internal static class TypeExtensions
	{
		public static bool IsEntity(this Type type)
		{
			return typeof(IEntity).IsAssignableFrom(type);
		}

		public static bool IsEntityRegistry(this Type type)
		{
			return typeof(IEntityRegistry).IsAssignableFrom(type);
		}

		public static bool IsEntityCollection(this Type type)
		{
			return false;
			return typeof(IEnumerable<IEntity>).IsAssignableFrom(type);
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
