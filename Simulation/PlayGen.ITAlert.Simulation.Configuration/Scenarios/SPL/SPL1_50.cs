using System.Collections.Generic;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using Engine.Systems.Timing.Actions;
using Engine.Systems.Timing.Evaluators;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.SPL.Archetypes;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
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
	internal class SPL1_50 : ScenarioFactory
	{
		public SPL1_50()
			: base(key: "SPL1_0.50",
				nameToken: "SPL Scenario 1 [50%]",
				descriptionToken: "SPL Scenario 1 - Virus Propogation Rate 50%",
				minPlayers: 1,
				maxPlayers: 4)
		{
			
		}

		public static readonly Archetype RedVirus50 = new Archetype(nameof(RedVirus50))
			.Extends(RedVirus.Archetype)
			.HasComponent(new ComponentBinding<MalwarePropogation>() {
				ComponentTemplate = new MalwarePropogation() {
					TicksRemaining = 200,
					Interval = 600,
					IntervalVariation = 50,
					RollThreshold = 20,
				}
			});

		// TODO: this should be parameterized further and read from config
		public override SimulationScenario GenerateScenario()
		{
			#region configuration

			#region graph

			var node00 = new NodeConfig(0, 0, AntivirusWorkstation.Archetype, "Antivirus");
			var node10 = new NodeConfig(1, 0, SubsystemNode.Archetype);
			var node20 = new NodeConfig(2, 0, SubsystemNode.Archetype);
			var node01 = new NodeConfig(0, 1, SubsystemNode.Archetype);
			var node11 = new NodeConfig(1, 1, SubsystemNode.Archetype);
			var node21 = new NodeConfig(2, 1, SubsystemNode.Archetype);
			var nodeConfigs = new NodeConfig[]
			{
				node00,
				node10,
				node20,
				node01,
				node11,
				node21,
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			var connection0010 = new EdgeConfig(node00.Id, EdgeDirection.East, node10.Id, ConnectionNode.Archetype);
			var connection1000 = connection0010.Reciprocate();

			var connection1020 = new EdgeConfig(node10.Id, EdgeDirection.East, node20.Id, ConnectionNode.Archetype);
			var connection2010 = connection1020.Reciprocate();

			var connection0001 = new EdgeConfig(node00.Id, EdgeDirection.South, node01.Id, ConnectionNode.Archetype);
			var connection0100 = connection0001.Reciprocate();

			var connection0111 = new EdgeConfig(node01.Id, EdgeDirection.East, node11.Id, ConnectionNode.Archetype);
			var connection1101 = connection0111.Reciprocate();

			var connection1121 = new EdgeConfig(node11.Id, EdgeDirection.East, node21.Id, ConnectionNode.Archetype);
			var connection2111 = connection1121.Reciprocate();

			var connection2021 = new EdgeConfig(node20.Id, EdgeDirection.South, node21.Id, ConnectionNode.Archetype);
			var connection2120 = connection2021.Reciprocate();

			var edgeConfigs = new EdgeConfig[]
			{
				connection0010,
				connection1000,
				connection1020,
				connection2010,
				connection0001,
				connection0100,
				connection0111,
				connection1101,
				connection1121,
				connection2111,
				connection2021,
				connection2120,
			};

			#endregion

			var archetypes = new List<Archetype>
			{
				SubsystemNode.Archetype,
				ConnectionNode.Archetype,
				Player.Archetype,
				ScannerTool.Archetype,
				AntivirusWorkstation.Archetype,
				AnalyserActivator.Archetype,
				CaptureTool.Archetype,
				AntivirusTool.Archetype,
				RedVirus50,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);
			configuration.RNGSeed = 561122999;

			#endregion

			var scenario = new SimulationScenario(ScenarioInfo)
			{
				TimeLimitSeconds = 480, // 8 minutes
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new[] { node00.Id, node20.Id, node01.Id, node21.Id }),
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),

				Scoring = SimulationScenario.ScoringMode.Full,
			};

			var spawnSequence = new NodeSequence(new[]
			{
				// offset by one vs config
				node00,
				node20,
				node10,
				node00,
				node20,
				node01,
				node10,
				node01,
			});

			#region frames

			// 1
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetTimer<Simulation, SimulationConfiguration>(scenario.TimeLimitSeconds.Value),
						new CreateItem(ScannerTool.Archetype, node10),
						new CreateItem(ScannerTool.Archetype, node20),
						new CreateItem(CaptureTool.Archetype, node10),
						new CreateItem(CaptureTool.Archetype, node10),
					},
					ExitCondition = new WaitForTicks(1),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(RedVirus50, node01)
					},
				}
			);

			// 2
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
					},
					OnTickActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new ConditionalAction<Simulation, SimulationConfiguration>(new NodeSequenceAction(spawnSequence, ec => new CreateMalware(RedVirus50, ec)), 
							new OnEvent<AntivirusActivationEvent>(aae => aae.ActivationResult == AntivirusActivationEvent.AntivirusActivationResult.SoloExtermination
								|| aae.ActivationResult == AntivirusActivationEvent.AntivirusActivationResult.CoopExtermination)),
					},
					ExitCondition = new WaitForTimer(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new EndGame(EndGameState.Neutral),
					},
				}
			);

			#endregion

			return scenario;
		}
	}
}
