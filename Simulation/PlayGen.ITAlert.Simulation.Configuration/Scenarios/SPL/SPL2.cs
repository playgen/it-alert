using System.Collections.Generic;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Lifecycle;
using Engine.Sequencing;
using Engine.Systems.Timing.Actions;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.SPL.Archetypes;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
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
	internal class SPL2 : ScenarioFactory
	{
		public SPL2()
			: base(key: "SPL2",
				nameToken: "SPL Scenario 2",
				descriptionToken: "SPL Scenario 2",
				minPlayers: 1,
				maxPlayers: 4)
		{

		}


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

			var node02 = new NodeConfig(0, 2, SubsystemNode.Archetype);
			var node12 = new NodeConfig(1, 2, SubsystemNode.Archetype);
			var node22 = new NodeConfig(2, 2, SubsystemNode.Archetype);

			var nodeConfigs = new NodeConfig[]
			{
				node00,
				node10,
				node20,

				node01,
				node11,
				node21,

				node02,
				node12,
				node22,
			};
			ConfigurationHelper.ProcessNodeConfigs(nodeConfigs);

			var connection0001 = new EdgeConfig(node00.Id, EdgeDirection.South, node01.Id, ConnectionNode.Archetype);
			var connection0100 = connection0001.Reciprocate();

			var connection1011 = new EdgeConfig(node10.Id, EdgeDirection.South, node11.Id, ConnectionNode.Archetype);
			var connection1110 = connection1011.Reciprocate();

			var connection2021 = new EdgeConfig(node20.Id, EdgeDirection.South, node21.Id, ConnectionNode.Archetype);
			var connection2120 = connection2021.Reciprocate();

			var connection0102 = new EdgeConfig(node01.Id, EdgeDirection.South, node02.Id, ConnectionNode.Archetype);
			var connection0201 = connection0102.Reciprocate();

			var connection1112 = new EdgeConfig(node11.Id, EdgeDirection.South, node12.Id, ConnectionNode.Archetype);
			var connection1211 = connection1112.Reciprocate();

			var connection2122 = new EdgeConfig(node21.Id, EdgeDirection.South, node22.Id, ConnectionNode.Archetype);
			var connection2221 = connection2122.Reciprocate();

			var connection0111 = new EdgeConfig(node01.Id, EdgeDirection.East, node11.Id, ConnectionNode.Archetype);
			var connection1101 = connection0111.Reciprocate();

			var connection1121 = new EdgeConfig(node11.Id, EdgeDirection.East, node21.Id, ConnectionNode.Archetype);
			var connection2111 = connection1121.Reciprocate();


			var edgeConfigs = new EdgeConfig[]
			{
				connection0001,
				connection0100,
				connection1011,
				connection1110,
				connection2021,
				connection2120,
				connection0102,
				connection0201,
				connection1112,
				connection1211,
				connection2122,
				connection2221,
				connection0111,
				connection1101,
				connection1121,
				connection2111,
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
				RedVirus.Archetype,
				GreenVirus.Archetype,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);
			configuration.RNGSeed = 561122999;

			#endregion

			var scenario = new SimulationScenario(ScenarioInfo)
			{
				TimeLimitSeconds = 600, // 10 minutes
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new[] { node00.Id, node20.Id, node11.Id, node02.Id }),
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};

			var redSpawnSequence = new NodeSequence(new[]
			{
				// offset by one vs config
				node10,
				node21,
				node20,
				node02,
				node02,
				node20,
				node10,
				node21,
				node00,
			});

			var greenSpawnSequence = new NodeSequence(new[]
			{
				// offset by one vs config
				node02,
				node02,
				node20,
				node10,
				node00,
				node01,
				node00,
				node10,
				node01,
				node20,
			});

			#region frames

			// 1
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new SetTimer<Simulation, SimulationConfiguration>(scenario.TimeLimitSeconds.Value),
						new CreateItem(ScannerTool.Archetype, node01),
						new CreateItem(ScannerTool.Archetype, node21),
						new CreateItem(CaptureTool.Archetype, node02),
					},
					ExitCondition = new WaitForTicks(1),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(RedVirus.Archetype, node00),
						new CreateMalware(GreenVirus.Archetype, node20),
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
						new ConditionalAction<Simulation, SimulationConfiguration>(new NodeSequenceAction(redSpawnSequence, ec => new CreateMalware(RedVirus.Archetype, ec)), new EntityDisposed<Malware>(new MalwareGenomeFilter(SimulationConstants.MalwareGeneRed))),
						new ConditionalAction<Simulation, SimulationConfiguration>(new NodeSequenceAction(greenSpawnSequence, ec => new CreateMalware(GreenVirus.Archetype, ec)), new EntityDisposed<Malware>(new MalwareGenomeFilter(SimulationConstants.MalwareGeneGreen))),
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
