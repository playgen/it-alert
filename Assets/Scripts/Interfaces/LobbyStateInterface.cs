using UnityEngine;
using System.Collections;
using GameWork.Interfacing;

public class LobbyStateInterface : StateInterface
{

    private GameObject _lobbyPanel;

    public override void Initialize()
    {
        _lobbyPanel = GameObject.Find("LobbyContainer").transform.GetChild(0).gameObject;
    }

    public override void Enter()
    {
        _lobbyPanel.SetActive(true);
    }

    public override void Exit()
    {
        _lobbyPanel.SetActive(false);
    }
}
