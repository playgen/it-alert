using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Systems;
using Zenject;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public abstract class ECSInstaller : Installer<ECSInstaller>
	{
		protected ECSConfiguration Configuration;

		protected ECSInstaller(ECSConfiguration configuration)
		{
			Configuration = configuration;
		}

		public override void InstallBindings()
		{
			// TODO: perhaps these shouldnt be here

			Container.BindFactory<Entity, Entity.Factory>();

			Container.Bind<IEntityRegistry>()
				.To<EntityRegistry>()
				.AsSingle();
			
			Container.Bind<IComponentRegistry>()
				.To<ComponentRegistry>()
				.AsSingle();

			Container.Bind<ISystemRegistry>()
				.To<SystemRegistry>()
				.AsSingle();

			// configuration driven system loading
			foreach (var systemConfiguration in Configuration.Systems)
			{
				InstallSystemBinding(systemConfiguration);
			}

			//foreach (var archetypeConfiguration in Configuration.Archetypes)
			//{
			//	InstallArchetype(archetypeConfiguration);
			//}

			OnInstallBindings();
		}

		protected virtual void OnInstallBindings()
		{
		}

		public void InstallSystemBinding(SystemConfiguration systemConfiguration)
		{
			Container.Bind<ISystem>()
				.FromSubContainerResolve().ByMethod(container => InstallSystem(container, systemConfiguration))
				// TODO: perhaps decide if it should be a singleton in configuration
				.AsSingle();
		}

		private static void InstallSystem(DiContainer container, SystemConfiguration systemConfiguration)
		{
			container.BindAllInterfaces(systemConfiguration.Type).To(systemConfiguration.Type).AsSingle();

			foreach (var extensionConfiguration in systemConfiguration.ExtensionConfiguration)
			{
				if (extensionConfiguration.AllOfType)
				{
					container.Bind(extensionConfiguration.Type).To(t => t.AllNonAbstractClasses().DerivingFrom(extensionConfiguration.Type));
				}
				else
				{
					foreach (var extensionImplementation in extensionConfiguration.Implementations)
					{
						container.Bind(extensionConfiguration.Type).To(extensionImplementation);
					}
				}
			}
		}

		//private void InstallArchetype(Archetype archetypeConfiguration)
		//{
		//	Container.BindIFactory<string, Entity, EntityFactory>()
		//		.WithId(archetypeConfiguration.Name)
		//		.FromSubContainerResolve()
		//		.ByMethod()
		//}

		//private static void InstallArchetypeFactory(DiContainer container, Archetype archetypeConfiguration)
		//{
		//	foreach (var extensionConfiguration in systemConfiguration.ExtensionConfiguration)
		//	{
		//		if (extensionConfiguration.AllOfType)
		//		{
		//			container.Bind(extensionConfiguration.Type).To(t => t.AllNonAbstractClasses().DerivingFrom(extensionConfiguration.Type));
		//		}
		//		else
		//		{
		//			foreach (var extensionImplementation in extensionConfiguration.Implementations)
		//			{
		//				container.Bind(extensionConfiguration.Type).To(extensionImplementation);
		//			}
		//		}
		//	}
		//}
	}

	// ReSharper disable InconsistentNaming
	public abstract class ECSInstaller<TECS> : ECSInstaller
		where TECS : ECS
	// ReSharper restore InconsistentNaming
	{
		protected ECSInstaller(ECSConfiguration configuration)
			: base(configuration)
		{
		}

		public TECS Instantiate()
		{
			return Container.Instantiate<TECS>();
		}
	}
}
