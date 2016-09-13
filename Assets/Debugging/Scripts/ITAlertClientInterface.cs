using System;
using UnityEngine;
using System.Linq;
using PlayGen.ITAlert.DataTransferObjects.Simulation;
using PlayGen.ITAlert.Network;

public class ITAlertClientInterface : MonoBehaviour
{
    private const string GamePlugin = "RoomControllerPlugin";

    [SerializeField]
    private string _gameVersion = "1";
    private ITAlertClient _client;

    private StateResponse _lastState;

    #region UI Variables

    private string _createRoomName = "ITAlertRoom";
    private int _createRoomNumPlayers = 4;
    #endregion

    private void Awake()
    {
        var clientBase = this.gameObject.AddComponent<Client>();
        clientBase.Initialize(_gameVersion, GamePlugin);

        _client = new ITAlertClient(clientBase);
    }

    private void OnGUI()
    {
        GUILayout.Label("Current State: " + _client.State);

        switch (_client.State)
        {
            case ClientStates.Roomless:
                ShowJoinCreateRoomOptions();
                break;

            case ClientStates.Lobby:
                ShowLobbyOptions();
                break;

            case ClientStates.Game:

                ShowGameState();

                switch (_client.GameState)
                {
                    case GameStates.Initializing:
                        ShowGameInitializingOptions();
                        break;

                    case GameStates.Playing:
                        ShowGamePlayingOptions();
                        break;

                    case GameStates.Finalizing:
                        ShowGameFinalizingOptions();
                        break;
                }

                break;
        }
    }

    private void ShowLobbyOptions()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("----Lobby----");

            if (!_client.IsReady)
            {
                if (GUILayout.Button("Set Ready"))
                {
                    _client.SetReady(true);
                }
            }

            if (_client.PlayerReadyStatus != null)
            {
                var readyCount = _client.PlayerReadyStatus.Values.Count(v => v);
                GUILayout.Label(readyCount + " / " + _client.ListCurrentRoomPlayers.Length);

                if (_client.IsReady)
                {
                    if (GUILayout.Button("Set Not Ready"))
                    {
                        _client.SetReady(false);
                    }
                    else if (GUILayout.Button("Start if all ready"))
                    {
                        _client.StartGame(false);
                    }
                    else if (GUILayout.Button("Force Start"))
                    {
                        _client.StartGame(true);
                    }
                }
            }
        }
        GUILayout.EndVertical();
    }

    private void ShowJoinCreateRoomOptions()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("----Join/Create Room----");

            if (GUILayout.Button("Join Random Room"))
            {
                _client.JoinRandomRoom();
            }

            GUILayout.BeginVertical("box");
            {
                GUILayout.Label("Rooms:");

                _client.ListRooms(ListRoomsFilters.Open | ListRoomsFilters.Visible).ToList().ForEach(r =>
                {
                    if (GUILayout.Button(r.name))
                    {
                        _client.JoinRoom(r.name);
                    }
                });
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Room Name: ");
                    _createRoomName = GUILayout.TextField(_createRoomName);
                }
                GUILayout.EndHorizontal();


                GUILayout.Label("Max Players: ");
                _createRoomNumPlayers = int.Parse(GUILayout.TextField(_createRoomNumPlayers.ToString()));


                if (GUILayout.Button("Create"))
                {
                    _client.CreateRoom(_createRoomName, _createRoomNumPlayers);
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }

    private void ShowGameState()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("----Game State----");

            GUILayout.Label("Pending Game State: " + _client.HasSimulationState);

            if (_client.HasSimulationState)
            {
                _lastState = _client.TakeSimulationState();
            }

            if (_lastState != null)
            {
                GUILayout.Label("Pending State Type: " + _lastState.Type);
                GUILayout.Label("Pending State Tick: " + _lastState.Tick);
                //GUILayout.TextArea(_lastState.State.MockData.Substring(0, 100));
            }
        }
        GUILayout.EndVertical();
    }

    private void ShowGameInitializingOptions()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("----Initializing----");

            if (GUILayout.Button("Set Initialized."))
            {
                _client.SetGameInitialized();
            }
        }
        GUILayout.EndVertical();
    }

    private void ShowGamePlayingOptions()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("----Playing----");

            if (GUILayout.Button("Send Dummy Command"))
            {
                //_client.SendGameCommand(new DummyCommand());
            }
        }
        GUILayout.EndVertical();
    }

    private void ShowGameFinalizingOptions()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("----Finalizing----");

            if (GUILayout.Button("Set Finalized."))
            {
                _client.SetGameFinalized();
            }
        }
        GUILayout.EndVertical();
    }
}