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
	public abstract class ECSInstaller<TInstaller> : Installer<TInstaller>
		where TInstaller : ECSInstaller<TInstaller>
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
			Container.Bind(systemConfiguration.Type).AsSingle();
			Container.BindAllInterfaces(systemConfiguration.Type).To(systemConfiguration.Type)
				//.FromSubContainerResolve().ByMethod(container => InstallSystem(container, systemConfiguration))
				//// TODO: perhaps decide if it should be a singleton in configuration
				.AsSingle();
			InstallSystem(Container, systemConfiguration);
		}

		private static void InstallSystem(DiContainer container, SystemConfiguration systemConfiguration)
		{
			//container.BindAllInterfaces(systemConfiguration.Type).To(systemConfiguration.Type).AsSingle();
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
				// TODO: keep this for later reference - we are using [InjectOptional] instead
				//var extensionListType = typeof(List<>).MakeGenericType(extensionConfiguration.Type);
				//if (container.HasBinding(new InjectContext(container, extensionListType, null)) == false)
				//{
				//	container.Bind(extensionListType).AsSingle();
				//}
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
	public abstract class ECSInstaller<TECS, TInstaller> : ECSInstaller<TInstaller>
		where TECS : ECS
		where TInstaller : ECSInstaller<TInstaller>
	// ReSharper restore InconsistentNaming
	{
		protected ECSInstaller(ECSConfiguration configuration)
			: base(configuration)
		{
		}

		public static TECS InstantiateECS<TConfiguration>(TConfiguration configuration)
			where TConfiguration : ECSConfiguration
		{
			var container = new DiContainer();
			container.BindInstance(configuration);
			Install(container);

			return container.Instantiate<TECS>();
		}
	}
}
