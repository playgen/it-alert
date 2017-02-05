﻿using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;

namespace PlayGen.ITAlert.Unity.States.Game.Menu.ScenarioList
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
				selectCommand?.Execute(_scenarioController);
			}
		}
	}
}