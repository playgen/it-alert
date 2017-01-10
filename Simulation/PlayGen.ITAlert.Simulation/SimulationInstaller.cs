using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Systems;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Configuration;
using System = Engine.Systems.System;

namespace PlayGen.ITAlert.Simulation
{
	public class SimulationInstaller : ECSInstaller<Simulation>
	{
		private readonly SimulationConfiguration _simulationConfiguration;

		public SimulationInstaller(SimulationConfiguration simulationConfiguration)
		{
			_simulationConfiguration = simulationConfiguration;
			Container.Bind<SimulationConfiguration>().FromInstance(simulationConfiguration).AsSingle();
		}

		protected override void OnInstallBindings()
		{
			// configuration driven system loading

			foreach (var system in _simulationConfiguration.Systems)
			{
				Container.Bind<ISystem>()
					.To(system.Type)
					.AsSingle();

				foreach (var systemExtensionType in AttributeHelper.SelectValues<SystemExtensionTypeAttribute, Type>(system.Type, attribute => attribute.ExtensionType))
				{
					// for now load all system extensions in app domain
					// TODO: load the enabled extensions from configuration too
					Container.Bind(systemExtensionType)
						.To(ModuleLoader.GetTypesImplementing(systemExtensionType));
				}
			}
		}
		public static Simulation CreateSimulation(SimulationConfiguration simulationConfiguration)
		{
			return new SimulationInstaller(simulationConfiguration).Instantiate();
		}
	}
}
