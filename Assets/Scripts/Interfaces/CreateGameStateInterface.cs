﻿using GameWork.Commands.States;
using GameWork.Interfacing;
using UnityEngine;

public class CreateGameStateInterface : StateInterface
{
    private GameObject _createGamePanel;

    public override void Initialize()
    {
        // Create Game Popup
        _createGamePanel = GameObjectUtilities.FindGameObject("CreateGameContainer/CreatePanelContainer");
        var popUpButtons = new ButtonList("CreateGameContainer/CreatePanelContainer/ButtonPanel");

        var createGameCloseButton = popUpButtons.GetButton("BackButtonContainer");
        createGameCloseButton.onClick.AddListener(OnBackClick);

        var createGamePopupButton = popUpButtons.GetButton("CreateButtonContainer");
        createGamePopupButton.onClick.AddListener(OnCreateClick);
        // Create Game Listener Goes Here
    }

    private void OnCreateClick()
    {
        var details = _createGamePanel.GetComponent<CreateGamePopupBehaviour>().GetGameDetails();
        EnqueueCommand(new CreateGameCommand(details.GameName, details.MaxPlayers));
    }

    private void OnBackClick()
    {
        EnqueueCommand(new PreviousStateCommand());
    }

    public override void Enter()
    {
        _createGamePanel.SetActive(true);
    }

    public override void Exit()
    {
        _createGamePanel.SetActive(false);
    }

    public void OnCreateGameSuccess()
    {
        EnqueueCommand(new NextStateCommand());
    }
}