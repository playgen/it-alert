using System.Collections.Generic;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Actions;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Scenario.Actions;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators;
using PlayGen.ITAlert.Simulation.Scenario.Localization;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.SPL
{
	// ReSharper disable once InconsistentNaming
	internal static class SPL3
	{
		private static SimulationScenario _scenario;
		public static SimulationScenario Scenario => _scenario ?? (_scenario = GenerateScenario());

		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateScenario()
		{
			#region configuration

			var text = new Dictionary<string, Dictionary<string, string>>();

			#region graph

			var nodeLeft = new NodeConfig()
			{
				Name = "Left",
				X = 0,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};

			var nodeRight = new NodeConfig()
			{
				Name = "Right",
				X = 1,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};

			var nodeConfigs = new NodeConfig[]
			{
				nodeLeft,
				nodeRight
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);
			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedConfiguration(nodeConfigs, 1);


			#endregion

			var archetypes = new List<Archetype>
			{
				SubsystemNode.Archetype,
				ConnectionNode.Archetype,
				Player.Archetype,
				ScannerTool.Archetype,
				RedTutorialAntivirus.Archetype,
				RedTutorialVirus.Archetype,
				TutorialText.Archetype,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);

			#endregion

			var scenario = new SimulationScenario()
			{
				Key = "SPL3",
				Name = "SPL Scenario 3",
				Description = "Scenario 3",
				MinPlayers = 1,
				MaxPlayers = 4,
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new[] { nodeRight.Id }),
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};

			scenario.LocalizationDictionary = LocalizationHelper.GetLocalizationFromEmbeddedResource(scenario.Key);

			#region frames

			// 1
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetCommandEnabled<SetActorDestinationCommand>(false),
						new ShowText(true, $"{scenario.Key}_Frame1")
					},
					ExitCondition = new WaitForTutorialContinue(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
						new SetCommandEnabled<SetActorDestinationCommand>(true),
					},
				}
			);
			#endregion

			return scenario;
		}
	}
}
