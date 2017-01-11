using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Systems;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation
{
	public class SimulationInstaller : ECSInstaller<Simulation>
	{
		private readonly SimulationConfiguration _simulationConfiguration;

		public SimulationInstaller(SimulationConfiguration simulationConfiguration)
			: base (simulationConfiguration)
		{
			_simulationConfiguration = simulationConfiguration;
			Container.Bind<SimulationConfiguration>().FromInstance(simulationConfiguration).AsSingle();
		}

		protected override void OnInstallBindings()
		{

		}
		public static Simulation CreateSimulation(SimulationConfiguration simulationConfiguration)
		{
			return new SimulationInstaller(simulationConfiguration).Instantiate();
		}
	}
}
