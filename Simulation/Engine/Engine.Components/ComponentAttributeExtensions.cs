using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Entities;

namespace PlayGen.Engine.Components
{
	public static class ComponentAttributeExtensions
	{
		#region attribute caches

		private static readonly Dictionary<Type, Type[]> ComponentDependencyTypesCache;

		private static readonly Dictionary<Type, Type[]> ComponentUsageTypeCache;

		private static readonly Dictionary<Type, Dictionary<Type, bool>> ComponentUsageCheckCache;


		static ComponentAttributeExtensions()
		{
			ComponentDependencyTypesCache = new Dictionary<Type, Type[]>();
			ComponentUsageTypeCache = new Dictionary<Type, Type[]>();
			ComponentUsageCheckCache = new Dictionary<Type, Dictionary<Type, bool>>();
		}

		#endregion

		#region dependencies

		public static IEnumerable<Type> GetComponentDependencyTypes(this IComponent component)
		{
			return component.GetType().GetComponentDependencyTypes();
		}

		public static IEnumerable<Type> GetComponentDependencyTypes(this Type componentType)
		{
			Type[] dependencyTypes;
			if (ComponentDependencyTypesCache.TryGetValue(componentType, out dependencyTypes) == false)
			{
				dependencyTypes = GenerateComponentDependencyTypes(componentType);
				ComponentDependencyTypesCache.Add(componentType, dependencyTypes);
			}
			return dependencyTypes;
		}

		private static Type[] GenerateComponentDependencyTypes(this Type componentType)
		{
			var dependencyAttributes = componentType.GetCustomAttributes(typeof(ComponentDependencyAttribute), true);
			return dependencyAttributes.OfType<ComponentDependencyAttribute>().Select(da => da.RequiredType).ToArray();
		}

		#endregion

		#region usage

		public static IEnumerable<Type> GetComponentUsageTypes(this IComponent component)
		{
			return component.GetType().GetComponentUsageTypes();
		}

		public static IEnumerable<Type> GetComponentUsageTypes(this Type componentType)
		{
			Type[] usageTypes;
			if (ComponentUsageTypeCache.TryGetValue(componentType, out usageTypes) == false)
			{
				usageTypes = GenerateComponentUsageTypes(componentType);
				ComponentUsageTypeCache.Add(componentType, usageTypes);
			}
			return usageTypes;
		}

		private static Type[] GenerateComponentUsageTypes(this Type componentType)
		{
			var usageAttributes = componentType.GetCustomAttributes(typeof(ComponentUsageAttribute), true);
			return usageAttributes.OfType<ComponentUsageAttribute>().Select(da => da.RequiredType).ToArray();
		}

		public static bool IsComponentUsageValid(this IComponent component, IEntity entity)
		{
			return component.IsComponentUsageValid(entity.GetType());
		}

		public static bool IsComponentUsageValid(this IComponent component, Type entityType)
		{
			return component.GetType().IsComponentUsageValid(entityType);
		}

		public static bool IsComponentUsageValid(this Type componentType, Type entityType)
		{
			Dictionary<Type, bool> usageChecks;
			if (ComponentUsageCheckCache.TryGetValue(componentType, out usageChecks) == false)
			{
				usageChecks = new Dictionary<Type, bool>();
				ComponentUsageCheckCache.Add(componentType, usageChecks);
			}

			bool checkResult;
			if (usageChecks.TryGetValue(entityType, out checkResult) == false)
			{
				checkResult = componentType.GetComponentUsageTypes().Any(type => type.IsAssignableFrom(entityType));
				usageChecks.Add(entityType, checkResult);
			}

			return checkResult;
		}

		#endregion
	}
}
