using System.Collections.Generic;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using Engine.Systems.Timing.Actions;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.SPL.Archetypes;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Actions;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Scenario.Actions;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters;
using PlayGen.ITAlert.Simulation.Scenario.Localization;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.SPL
{
	// ReSharper disable once InconsistentNaming
	internal class SPL3 : ScenarioFactory
	{
		public SPL3(int timeLimitSeconds = 480)
			: base(key: "SPL3",
				nameToken: "SPL Scenario 3",
				descriptionToken: "SPL Scenario 3",
				minPlayers: 1,
				maxPlayers: 4,
                timeLimitSeconds: timeLimitSeconds)
		{

		}

		// TODO: this should be parameterized further and read from config
		public override SimulationScenario GenerateScenario()
		{
			#region configuration

			var text = new Dictionary<string, Dictionary<string, string>>();

			#region graph

			var node00 = new NodeConfig(0, 0, SubsystemNode.Archetype);
			var node10 = new NodeConfig(1, 0, SubsystemNode.Archetype);
			var node20 = new NodeConfig(2, 0, SubsystemNode.Archetype);
			var node30 = new NodeConfig(3, 0, SubsystemNode.Archetype);

			var node01 = new NodeConfig(0, 1, TransferWorkstation.Archetype);
			var node11 = new NodeConfig(1, 1, AntivirusWorkstation.Archetype);
			var node21 = new NodeConfig(2, 1, SubsystemNode.Archetype);
			var node31 = new NodeConfig(3, 1, TransferWorkstation.Archetype);

			var node02 = new NodeConfig(0, 2, SubsystemNode.Archetype);
			var node12 = new NodeConfig(1, 2, SubsystemNode.Archetype);
			var node22 = new NodeConfig(2, 2, SubsystemNode.Archetype);
			var node32 = new NodeConfig(3, 2, SubsystemNode.Archetype);

			var nodeConfigs = new NodeConfig[]
			{
				node00,
				node10,
				node20,
				node30,

				node01,
				node11,
				node21,
				node31,

				node02,
				node12,
				node22,
				node32,
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			// top row

			// horizontal
			var connection0010 = new EdgeConfig(node00, EdgeDirection.East, node10, ConnectionNode.Archetype);
			var connection1000 = connection0010.Reciprocate();

			var connection1020 = new EdgeConfig(node10, EdgeDirection.East, node20, ConnectionNode.Archetype);
			var connection2010 = connection1020.Reciprocate();

			var connection2030 = new EdgeConfig(node20, EdgeDirection.East, node30, ConnectionNode.Archetype);
			var connection3020 = connection2030.Reciprocate();

			//vertical
			var connection0001 = new EdgeConfig(node00, EdgeDirection.South, node01, ConnectionNode.Archetype);
			var connection0100 = connection0001.Reciprocate();

			var connection1011 = new EdgeConfig(node10, EdgeDirection.South, node11, ConnectionNode.Archetype);
			var connection1110 = connection1011.Reciprocate();

			var connection3031 = new EdgeConfig(node30, EdgeDirection.South, node31, ConnectionNode.Archetype);
			var connection3130 = connection3031.Reciprocate();

			// middle row
			
			//horizontal
			var connection0111 = new EdgeConfig(node01, EdgeDirection.East, node11, ConnectionNode.Archetype);
			var connection1101 = connection0111.Reciprocate();

			var connection2131 = new EdgeConfig(node21, EdgeDirection.East, node31, ConnectionNode.Archetype);
			var connection3121 = connection2131.Reciprocate();

			// vertical
			var connection0102 = new EdgeConfig(node01, EdgeDirection.South, node02, ConnectionNode.Archetype);
			var connection0201 = connection0102.Reciprocate();

			var connection2122 = new EdgeConfig(node21, EdgeDirection.South, node22, ConnectionNode.Archetype);
			var connection2221 = connection2122.Reciprocate();

			var connection3132 = new EdgeConfig(node31, EdgeDirection.South, node32, ConnectionNode.Archetype);
			var connection3231 = connection3132.Reciprocate();

			// bottom row

			//horizontal
			var connection0212 = new EdgeConfig(node02, EdgeDirection.East, node12, ConnectionNode.Archetype);
			var connection1202 = connection0212.Reciprocate();

			var connection1222 = new EdgeConfig(node12, EdgeDirection.East, node22, ConnectionNode.Archetype);
			var connection2212 = connection1222.Reciprocate();

			var connection2232 = new EdgeConfig(node22, EdgeDirection.East, node32, ConnectionNode.Archetype);
			var connection3222 = connection2232.Reciprocate();

			var edgeConfigs = new List<EdgeConfig>()
			{
				connection0010,
				connection1000,
				connection1020,
				connection2010,
				connection2030,
				connection3020,
				connection0001,
				connection0100,
				connection1011,
				connection1110,
				connection3031,
				connection3130,
				connection0111,
				connection1101,
				connection2131,
				connection3121,
				connection0102,
				connection0201,
				connection2122,
				connection2221,
				connection3132,
				connection3231,
				connection0212,
				connection1202,
				connection1222,
				connection2212,
				connection2232,
				connection3222,
			};

			#endregion

			var archetypes = new List<Archetype>
			{
				SubsystemNode.Archetype,
				ConnectionNode.Archetype,
				Player.Archetype,

				AntivirusWorkstation.Archetype,
				AnalyserActivator.Archetype,
				CaptureTool.Archetype,
				AntivirusTool.Archetype,

				TransferWorkstation.Archetype,
				TransferActivator.Archetype,

				ScannerTool.Archetype,
				RedVirus.Archetype,
				GreenVirus.Archetype
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);
			configuration.RNGSeed = 889753438;

			#endregion

			var scenario = new SimulationScenario(ScenarioInfo)
			{
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new[] { node00.Id, node30.Id, node02.Id, node32.Id }),
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),

				Scoring = SimulationScenario.ScoringMode.Full,
			};

			var redSpawnSequence = new NodeSequence(new[]
			{
				// offset by one vs config
				node10,
				node22,
				node02,
				node30,
				node21,
				node01,
				node31,
				node11,
				node12,
				node00,
				node32,
				node10,
			});

			var greenSpawnSequence = new NodeSequence(new[]
			{
				// offset by one vs config
				node12,
				node00,
				node32,
				node20,
				node31,
				node22,
				node02,
				node30,
				node21,
				node01,
				node10,
				node11,
				node12,
			});

			#region frames

			// 1
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetTimer<Simulation, SimulationConfiguration>(scenario.TimeLimitSeconds.Value),
						new CreateItem(ScannerTool.Archetype, node00),
						new CreateItem(ScannerTool.Archetype, node32),
						new CreateItem(CaptureTool.Archetype, node10),
						new CreateItem(CaptureTool.Archetype, node22),
					},
					ExitCondition = new WaitForTicks(1),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(RedVirus.Archetype, node10),
						new CreateMalware(GreenVirus.Archetype, node21),
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
						new ConditionalAction<Simulation, SimulationConfiguration>(new NodeSequenceAction(redSpawnSequence, ec => new CreateMalware(RedVirus.Archetype, ec)),
							new OnEvent<AntivirusActivationEvent>(aae => (aae.ActivationResult == AntivirusActivationEvent.AntivirusActivationResult.SoloExtermination
																		|| aae.ActivationResult == AntivirusActivationEvent.AntivirusActivationResult.CoopExtermination)
																		&& aae.GenomeEradicated == SimulationConstants.MalwareGeneRed)),
						new ConditionalAction<Simulation, SimulationConfiguration>(new NodeSequenceAction(greenSpawnSequence, ec => new CreateMalware(GreenVirus.Archetype, ec)),
							new OnEvent<AntivirusActivationEvent>(aae => (aae.ActivationResult == AntivirusActivationEvent.AntivirusActivationResult.SoloExtermination
																		|| aae.ActivationResult == AntivirusActivationEvent.AntivirusActivationResult.CoopExtermination)
																		&& aae.GenomeEradicated == SimulationConstants.MalwareGeneGreen)),

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
