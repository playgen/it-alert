using UnityEngine;
using GameWork.Commands.States;
using GameWork.Interfacing;
using UnityEngine.UI;

public class LobbyStateInterface : StateInterface
{
    private GameObject _lobbyPanel;
    private Button _readyButton;

    private bool _ready;

    public override void Initialize()
    {
        _lobbyPanel = GameObject.Find("LobbyContainer").transform.GetChild(0).gameObject;
        var buttons = new ButtonList("LobbyContainer/LobbyPanelContainer/ButtonPanel");

        var backButton = buttons.GetButton("BackButtonContainer");
        backButton.onClick.AddListener(OnBackButtonClick);

        _readyButton = buttons.GetButton("ReadyButtonContainer");
        _readyButton.onClick.AddListener(OnReadyButtonClick);
    }

    private void OnReadyButtonClick()
    {
        EnqueueCommand(new ReadyPlayerCommand(!_ready));
    }

    private void OnBackButtonClick()
    {
        EnqueueCommand(new PreviousStateCommand());
    }

    public override void Enter()
    {
        _lobbyPanel.SetActive(true);
    }

    public override void Exit()
    {
        _lobbyPanel.SetActive(false);
    }

    public void OnReadySucceeded()
    {
        var text = "";
        if (_ready)
        {
            text = "READY";
            _ready = false;
        }
        else
        {
            text = "WAITING";
            _ready = true;
        }
        _readyButton.gameObject.GetComponent<Text>().text = text;
    }
}
