using GameWork.Core.States.Commands;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.GameStates.GameSubStates;

using UnityEngine;

public class PlayingTickableStateInput : TickStateInput
{
	private GameObject _gamePanel;
	private GameObject _gameContainer;

	protected override void OnInitialize()
	{
		_gamePanel = GameObjectUtilities.FindGameObject("Game/GameCanvas/GameContainer");
		_gameContainer = GameObjectUtilities.FindGameObject("Game/Graph");
	}

	private void OnPauselick()
	{
		CommandQueue.AddCommand(new ChangeStateCommand(PausedState.StateName));
	}

	protected override void OnEnter()
	{
		_gamePanel.SetActive(true);
		_gameContainer.SetActive(true);
	}

	protected override void OnExit()
	{
		_gamePanel.SetActive(false);
		_gameContainer.SetActive(false);
	}

	protected override void OnTick(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnPauselick();
		}
	}
}