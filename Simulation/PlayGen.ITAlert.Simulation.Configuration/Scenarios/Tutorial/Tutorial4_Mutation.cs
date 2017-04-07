using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Evaluators;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Archetypes;
using PlayGen.ITAlert.Simulation.Scenario.Actions;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial
{
	public static class Tutorial4_Mutation
	{
		private static SimulationScenario _scenario;
		public static SimulationScenario Scenario => _scenario ?? (_scenario = GenerateScenario());

		private static readonly Archetype VisibleRedTutorialVirus = new Archetype(nameof(VisibleRedTutorialVirus))
			.Extends(RedTutorialVirus.Archetype)
			.HasComponent(new ComponentBinding<MalwareVisibility>()
			{
				ComponentTemplate = new MalwareVisibility()
				{
					VisibleTo = MalwareVisibility.All,
				}
			});

		private static readonly Archetype VisibleGreenTutorialVirus = new Archetype(nameof(VisibleGreenTutorialVirus))
			.Extends(GreenTutorialVirus.Archetype)
			.HasComponent(new ComponentBinding<MalwareVisibility>()
			{
				ComponentTemplate = new MalwareVisibility()
				{
					VisibleTo = MalwareVisibility.All,
				}
			});

		// TODO: this should be parameterized further and read from config
		private static SimulationScenario GenerateScenario()
		{
			#region configuration

			const int playerCount = 1;

			var text = new Dictionary<string, Dictionary<string, string>>();

			#region graph

			var node00 = new NodeConfig()
			{
				Name = "00",
				X = 0,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};
			var node10 = new NodeConfig()
			{
				Name = "10",
				X = 1,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};
			var node20 = new NodeConfig()
			{
				Name = "20",
				X = 2,
				Y = 0,
				Archetype = SubsystemNode.Archetype,
			};
			var node01 = new NodeConfig()
			{
				Name = "01",
				X = 0,
				Y = 1,
				Archetype = SubsystemNode.Archetype,
			};
			var node11 = new NodeConfig()
			{
				Name = "11",
				X = 1,
				Y = 1,
				Archetype = SubsystemNode.Archetype,
			};
			var node21 = new NodeConfig()
			{
				Name = "21",
				X = 2,
				Y = 1,
				Archetype = SubsystemNode.Archetype,
			};


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

			var connection0010 = new EdgeConfig(node00.Id, EdgeDirection.East, node10.Id);
			var connection1000 = new EdgeConfig(node10.Id, EdgeDirection.West, node01.Id);

			var connection1020 = new EdgeConfig(node10.Id, EdgeDirection.East, node20.Id);
			var connection2010 = new EdgeConfig(node20.Id, EdgeDirection.West, node10.Id);

			var connection2021 = new EdgeConfig(node20.Id, EdgeDirection.South, node21.Id);
			var connection2120 = new EdgeConfig(node21.Id, EdgeDirection.North, node20.Id);

			var connection0001 = new EdgeConfig(node00.Id, EdgeDirection.South, node01.Id);
			var connection0100 = new EdgeConfig(node01.Id, EdgeDirection.North, node00.Id);

			var connection0111 = new EdgeConfig(node01.Id, EdgeDirection.East, node11.Id);
			var connection1101 = new EdgeConfig(node11.Id, EdgeDirection.West, node01.Id);

			var connection1121 = new EdgeConfig(node11.Id, EdgeDirection.East, node21.Id);
			var connection2111 = new EdgeConfig(node21.Id, EdgeDirection.West, node11.Id);

			var edgeConfigs = new EdgeConfig[]
			{
				connection0010,
				connection1000,
				connection1020,
				connection2010,
				connection2021,
				connection2120,
				connection0001,
				connection0100,
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
				TransferWorkstation.Archetype,
				TransferActivator.Archetype,
				Player.Archetype,
				ScannerTool.Archetype,
				RedTutorialAntivirus.Archetype,
				VisibleRedTutorialVirus,
				GreenTutorialAntivirus.Archetype,
				VisibleGreenTutorialVirus,
			};

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, null, archetypes);

			#endregion

			var scenario = new SimulationScenario()
			{
				Key = "Tutorial4",
				Name = "Tutorial4_Name",
				Description = "Tutorial4_Description",
				MinPlayers = playerCount,
				MaxPlayers = playerCount,
				Configuration = configuration,

				PlayerConfigFactory = new StartingLocationSequencePlayerConfigFactory(Player.Archetype, new[] {node21.Id}),
				Sequence = new List<SequenceFrame<Simulation, SimulationConfiguration>>(),
			};

			#region frames

			// 1
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(VisibleRedTutorialVirus, node00),
						new ShowText(true, $"{scenario.Key}_Frame1")
					},
					Evaluator = new WaitForTutorialContinue(),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 2
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(VisibleRedTutorialVirus, connection0001),
						new ShowText(true, $"{scenario.Key}_Frame2")
					},
					Evaluator = new WaitForTutorialContinue()
						.And(new IsInfected(node10)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 3
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(VisibleRedTutorialVirus, connection0001),
						new ShowText(true, $"{scenario.Key}_Frame3")
					},
					Evaluator = new WaitForTutorialContinue()
						.And(new IsInfected(connection0001)),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);
			// 4
			scenario.Sequence.Add(
				new SimulationFrame()
				{
					OnEnterActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new CreateMalware(VisibleGreenTutorialVirus, node20),
						new CreateMalware(VisibleGreenTutorialVirus, connection2010),
						new ShowText(true, $"{scenario.Key}_Frame4")
					},
					Evaluator = new WaitForTutorialContinue()
						.And(new IsInfected(node10, new AntivirusGenomeFilter(SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneGreen))),
					OnExitActions = new List<ECSAction<Simulation, SimulationConfiguration>>()
					{
						new HideText(),
					},
				}
			);

			#endregion

			return scenario;
		}
	}
}
