using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Util
{
	public static class ModuleLoader
	{
		public static IEnumerable<Type> GetTypesImplementing<TInterface>()
		{
			return AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(assembly => assembly.GetTypes().Where(t => typeof(TInterface).IsAssignableFrom(t)));
		}

		public static IEnumerable<TInterface> InstantiateTypesImplementing<TInterface>(params object[] constructorArgs)
			where TInterface : class
		{
			return GetTypesImplementing<TInterface>().Select(t => Activator.CreateInstance(t, constructorArgs)).Cast<TInterface>();
		}

		public static void LoadModuleAssemblies()
		{
			// TODO: load all module assemblies from a specified path into the app domain, checking if they are already loaded 
			throw new NotImplementedException();
		}
	}
}
