using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Entities;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Tutorial;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Evaluators;
using PlayGen.ITAlert.Simulation.Sequencing;
using PlayGen.ITAlert.Simulation.Systems.Tutorial;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class ScenarioHelpers
	{
		#region text

		public static SimulationAction HideTextAction => new SimulationAction((ecs, config) =>
		{
			ecs.HandleCommand(new HideTextCommand());
		}, "Hide Text");

		public static SimulationAction GenerateTextAction(bool @continue, params string[] text)
		{
			return new SimulationAction((ecs, config) =>
			{
				var textCommand = new DisplayTextCommand()
				{
					Text = text.Aggregate(new StringBuilder(), (sb, t) => sb.AppendLine(t), sb => sb.ToString()),
					Continue = @continue,
				};
				ecs.HandleCommand(textCommand);
			}, "Welcome Message");
		}

		#endregion

		#region commands

		public static SimulationAction SetCommandEnabled<TCommand>(bool enabled)
			where TCommand : ICommand
		{
			return new SimulationAction((ecs, config) =>
			{
				ICommandSystem commandSystem;
				ICommandHandler commandHandler;
				if (ecs.TryGetSystem(out commandSystem)
					&& commandSystem.TryGetHandler<TCommand>(out commandHandler))
				{
					commandHandler.SetEnabled(enabled);
				}
			}, $"Set command enabled ({nameof(TCommand)}): {enabled}");
		}

		#endregion

		#region evaluator helpers

		public static SimulationEvaluator WaitForTutorialContinue => new SimulationEvaluator((ecs, config) =>
		{
			ITutorialSystem tutorialSystem;
			if (ecs.TryGetSystem(out tutorialSystem))
			{
				return tutorialSystem.Continue;
			}
			return false;
		});

		public static TimeEvaluator<Simulation, SimulationConfiguration> WaitForSeconds(int seconds)
		{
			return new TimeEvaluator<Simulation, SimulationConfiguration>()
			{
				Threshold = 1000 * seconds,
			};
		}

		public static TickEvaluator<Simulation, SimulationConfiguration> WaitForTicks(int ticks)
		{
			return new TickEvaluator<Simulation, SimulationConfiguration>()
			{
				Threshold = ticks,
			};
		}

		public static SimulationEvaluator OnlyPlayerIsAtLocation(NodeConfig node)
		{
			return new SimulationEvaluator((ecs, config) =>
			{
				var playerId = config.PlayerConfiguration.Single().EntityId;
				Entity playerEntity;
				CurrentLocation location;
				return ecs.Entities.TryGetValue(playerId, out playerEntity)
						&& playerEntity.TryGetComponent(out location)
						&& location.Value == node.EntityId;
			});
		}

		#endregion

		#region entity creation

		public static SimulationAction CreateItemCommand(string itemType, int systemLogicalId)
		{
			return new SimulationAction((ecs, config) =>
			{
				var createItemCommand = new CreateItemCommand()
				{
					Archetype = itemType,
					IdentifierType = IdentifierType.Logical,
					SystemId = systemLogicalId
				};
				ecs.HandleCommand(createItemCommand);
			}, "Create item");
		}

		public static SimulationAction CreateNpcCommand(string archetype, int systemLogicalId)
		{
			return new SimulationAction((ecs, config) =>
			{
				var createNpcCommand = new CreateNpcCommand()
				{
					Archetype = archetype,
					IdentifierType = IdentifierType.Logical,
					SystemId = systemLogicalId
				};
				ecs.HandleCommand(createNpcCommand);
			}, "Create NPC");
		}

		#endregion
	}
}
