using System;
using UnityEngine;
using System.Linq;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Network.Client.States;
using PlayGen.ITAlert.Network.Client.Voice;
using PlayGen.ITAlert.Simulation;

public class ITAlertClientInterface : MonoBehaviour
{
    private const string GamePlugin = "RoomControllerPlugin";

    [SerializeField]
    private string _gameVersion = "1";
    private Client _client;

    private Simulation _lastSimulation;

    #region UI Variables

    private string _createRoomName = "ITAlertRoom";
    private int _createRoomNumPlayers = 4;
    #endregion

    private void Awake()
    {
        var clientBase = this.gameObject.AddComponent<PhotonClient>();
        clientBase.Initialize(_gameVersion, GamePlugin);

        _client = new Client(clientBase);
    }

    private void OnGUI()
    {
        GUILayout.Label("Current State: " + _client.State);

        switch (_client.State)
        {
            case States.Connected:
                ShowJoinCreateRoomOptions();
                break;

            case States.Lobby:
                ShowVoiceOptions();
                ShowLobbyOptions();
                break;

            case States.Game:
                ShowGameState();
                ShowVoiceOptions();

                switch (_client.CurrentRoom.CurrentGame.State)
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

    private void ShowVoiceOptions()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("----Voice----");

            GUILayout.BeginVertical("box");
            {
                GUILayout.Label("Talking:");

                foreach (var kvp in VoiceClient.TransmittingStatuses)
                {
                    GUILayout.Label(kvp.Key + " is " + (kvp.Value ? "Talking" : "Not Talking"));
                }
            }
            GUILayout.EndVertical();


            if (GUILayout.Button("Start Transmitting"))
            {
                _client.CurrentRoom.VoiceClient.StartTransmission();
            }
            else if (GUILayout.Button("Stop Transmitting"))
            {
                _client.CurrentRoom.VoiceClient.StopTransmission();
            }
        }
        GUILayout.EndVertical();
    }

    private void ShowLobbyOptions()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("----Lobby----");

            if (GUILayout.Button("Leave Room"))
            {
                _client.CurrentRoom.Leave();
            }

            if (!_client.CurrentRoom.IsReady)
            {
                if (GUILayout.Button("Set Ready"))
                {
                    _client.CurrentRoom.SetReady(true);
                }
            }

            if (_client.CurrentRoom.PlayerReadyStatus != null)
            {
                var readyCount = _client.CurrentRoom.PlayerReadyStatus.Values.Count(v => v);
                GUILayout.Label(readyCount + " / " + _client.CurrentRoom.ListCurrentRoomPlayers.Length);

                if (_client.CurrentRoom.IsReady)
                {
                    if (GUILayout.Button("Set Not Ready"))
                    {
                        _client.CurrentRoom.SetReady(false);
                    }
                    else if (GUILayout.Button("Start if all ready"))
                    {
                        _client.CurrentRoom.StartGame(false);
                    }
                    else if (GUILayout.Button("Force Start"))
                    {
                        _client.CurrentRoom.StartGame(true);
                    }
                }
            }
        }
        GUILayout.EndVertical();
    }

  
    private void ShowGameState()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("----Game State----");

            GUILayout.Label("Pending Game State: " + _client.CurrentRoom.CurrentGame.HasSimulationState);

            if (_client.CurrentRoom.CurrentGame.HasSimulationState)
            {
                _lastSimulation = _client.CurrentRoom.CurrentGame.TakeSimulationState();
            }

            if (_lastSimulation != null)
            {
                GUILayout.Label("Pending Simulation: " + _lastSimulation.CurrentTick);
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
                _client.CurrentRoom.CurrentGame.SetGameInitialized();
            }
        }
        GUILayout.EndVertical();
    }

    private void ShowGamePlayingOptions()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("----Playing----");
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
                _client.CurrentRoom.CurrentGame.SetGameFinalized();
            }
        }
        GUILayout.EndVertical();
    }
}