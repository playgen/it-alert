using System;
using System.Linq;
using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;

using PlayGen.ITAlert.GameStates.GameSubStates;

using UnityEngine;

public class PlayingStateInterface : TickableStateInterface
{
	private GameObject _gamePanel;
	private GameObject _gameContainer;

	public override void Initialize()
	{
		_gamePanel = GameObjectUtilities.FindGameObject("Game/GameCanvas/GameContainer");
		_gameContainer = GameObjectUtilities.FindGameObject("Game/Graph");
	}

	private void OnPauselick()
	{
		EnqueueCommand(new ChangeStateCommand(PausedState.StateName));
	}

	public override void Enter()
	{
		_gamePanel.SetActive(true);
		_gameContainer.SetActive(true);
	}

	public override void Exit()
	{
		_gamePanel.SetActive(false);
		_gameContainer.SetActive(false);
	}

	public override void Tick(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnPauselick();
		}
	}
}