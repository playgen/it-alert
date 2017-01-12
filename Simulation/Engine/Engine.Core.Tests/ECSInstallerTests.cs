using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Systems;
using NUnit.Framework;
using Zenject;

namespace Engine.Core.Tests
{
	[TestFixture]
	// ReSharper disable once InconsistentNaming
	public class ECSInstallerTests
	{
		#region test classes

		#region systems

		#region A

		public interface ISystemA : ISystem
		{
			ISystemB SystemB { get; }
		}

		public class SystemA : Systems.System, ISystemA
		{
			public ISystemB SystemB { get; }

			public IList<ISystemAExtension> Extensions { get; }

			public SystemA(IComponentRegistry componentRegistry, IEntityRegistry entityRegistry, ISystemB systemB, List<ISystemAExtension> extensions)
				: base(componentRegistry, entityRegistry)
			{
				SystemB = systemB;
				SystemB.Value = "TEST";
				Extensions = extensions;
			}

		}

		public interface ISystemAExtension : ISystemExtension
		{
			
		}

		public class ConcreteAExtensionA : ISystemAExtension
		{
			
		}

		public class ConcreteAExtensionB : ISystemAExtension
		{

		}

		#endregion

		#region B

		public interface ISystemB : ISystem
		{
			string Value { get; set; }
		}

		public class SystemB : Systems.System, ISystemB
		{
			public IList<ISystemBExtension> Extensions { get; }

			public SystemB(IComponentRegistry componentRegistry, 
				IEntityRegistry entityRegistry, 
				[InjectOptional] List<ISystemBExtension> extensions)
				: base(componentRegistry, entityRegistry)
			{
				Extensions = extensions;
			}

			public string Value { get; set; }
		}

		public interface ISystemBExtension : ISystemExtension
		{

		}

		#endregion

		#region C

		public interface ISystemC : ISystemA
		{

		}

		public class SystemC : Systems.System, ISystemC
		{
			public ISystemB SystemB { get; }

			public SystemC(IComponentRegistry componentRegistry, 
				IEntityRegistry entityRegistry, 
				ISystemB systemB)
				: base(componentRegistry, entityRegistry)
			{
				SystemB = systemB;
			}

		}


		#endregion

		#endregion

		#region installer

		// ReSharper disable once InconsistentNaming
		public class TestECS : ECS
		{
			public TestECS(IEntityRegistry entityRegistry, IComponentRegistry componentRegistry, ISystemRegistry systemRegistry)
				: base(entityRegistry, componentRegistry, systemRegistry)
			{
			}
		}

		public class TestInstaller : ECSInstaller<TestECS, TestInstaller>
		{
			public DiContainer PublicContainer => Container;

			public TestInstaller(ECSConfiguration configuration)
				: base(configuration)
			{
			}
		}

		#endregion

		#endregion


		[Test]
		public void TestSystemConfigurationInstaller()
		{
			var systemConfigurations = new List<SystemConfiguration>()
			{
				new SystemConfiguration<SystemA>()
				{
					ExtensionConfiguration = new SystemExtensionConfiguration[]
					{
						new SystemExtensionConfiguration<ISystemAExtension>()
						{
							AllOfType = true,
						},
					}
				},
				new SystemConfiguration<SystemB>()
				{
					ExtensionConfiguration = new SystemExtensionConfiguration[]
					{
						new SystemExtensionConfiguration<ISystemBExtension>()
						{
							AllOfType = true,
						},
					}
				},
				new SystemConfiguration<SystemC>()
				{
					ExtensionConfiguration = new SystemExtensionConfiguration[0]
				},
			};

			var configuration = new ECSConfiguration(null, systemConfigurations);

			var ecs = TestInstaller.InstantiateECS(configuration);

			Assert.That(ecs, Is.Not.Null);
			Assert.That(ecs.SystemRegistry.GetSystems<ISystemA>(), Is.Not.Null);
			Assert.That(ecs.SystemRegistry.GetSystems<ISystemA>().Count, Is.EqualTo(2));
			Assert.That(ecs.SystemRegistry.GetSystem<ISystemB>(), Is.Not.Null);
			Assert.That(ecs.SystemRegistry.GetSystem<ISystemC>(), Is.Not.Null);
			Assert.That(ecs.SystemRegistry.GetSystem<SystemA>().SystemB, Is.Not.Null);
			Assert.That(ecs.SystemRegistry.GetSystem<SystemA>().SystemB, Is.EqualTo(ecs.SystemRegistry.GetSystem<ISystemB>()));
			Assert.That(ecs.SystemRegistry.GetSystem<SystemA>().SystemB, Is.EqualTo(ecs.SystemRegistry.GetSystem<ISystemC>().SystemB));

			Assert.That(ecs.SystemRegistry.GetSystem<SystemA>().Extensions.Count, Is.EqualTo(2));
			Assert.That(ecs.SystemRegistry.GetSystem<SystemB>().Extensions.Count, Is.EqualTo(0));
		}

	}
}
