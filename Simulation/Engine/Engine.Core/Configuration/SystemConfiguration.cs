using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Systems;

namespace Engine.Configuration
{
	public abstract class SystemConfiguration
	{
		public abstract Type Type { get; }

		public SystemExtensionConfiguration[] ExtensionConfiguration { get; set; }
	}

	public sealed class SystemConfiguration<TSystem> : SystemConfiguration
		where TSystem : ISystem
	{
		public override Type Type => typeof(TSystem);
	}

	public abstract class SystemExtensionConfiguration
	{
		public abstract Type Type { get; }

		public bool AllOfType { get; set; }

		public Type[] Implementations { get; set; }
	}

	public sealed class SystemExtensionConfiguration<TExtension> : SystemExtensionConfiguration
		where TExtension : ISystemExtension
	{
		public override Type Type => typeof(TExtension);
	}
}
