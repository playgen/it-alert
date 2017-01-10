using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using Zenject;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public abstract class ECSInstaller : Installer<ECSInstaller>
	{
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

			OnInstallBindings();
		}

		protected abstract void OnInstallBindings();
	}

	// ReSharper disable InconsistentNaming
	public abstract class ECSInstaller<TECS> : ECSInstaller
		where TECS : ECS
	// ReSharper restore InconsistentNaming
	{
		public TECS Instantiate()
		{
			return Container.Instantiate<TECS>();
		}
	}
}
