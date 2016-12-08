using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Network.Client.Voice;

public class VoiceController
{
    private VoiceClient _voiceClient;
    private readonly Client _client;

    public VoiceController(Client client)
    {
        _client = client;
        _client.JoinedRoomEvent += OnJoinedRoom;
    }

    public void HandleVoiceInput()
    {
        if (Input.GetKey(KeyCode.Tab) && _voiceClient != null && !_voiceClient.IsTransmitting)
        {
            _voiceClient.StartTransmission();
        }
        else if (!Input.GetKey(KeyCode.Tab) && _voiceClient != null && _voiceClient.IsTransmitting)
        {
            _voiceClient.StopTransmission();
        }
    }

    private void OnJoinedRoom(ClientRoom room)
    {
        _voiceClient = room.VoiceClient;
    }
}
