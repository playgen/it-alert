using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;

namespace PlayGen.ITAlert.Unity.GameStates.Menu.ScenarioList
{
	public class ScenarioListState : InputTickState
	{
		private readonly ScenarioController _scenarioController;

		public const string StateName = "ScenarioListState";
		public override string Name => StateName;

		#region constructor

		public ScenarioListState(ScenarioListStateInput input, ScenarioController scenarioController) : base(input)
		{
			_scenarioController = scenarioController;
		}

		#endregion

		protected override void OnTick(float deltaTime)
		{
			ICommand command;
			if (CommandQueue.TryTakeFirstCommand(out command))
			{
				var selectCommand = command as SelectScenarioCommand;
				if (selectCommand != null)
				{
					selectCommand.Execute(_scenarioController);
				}
			}
		}
	}
}