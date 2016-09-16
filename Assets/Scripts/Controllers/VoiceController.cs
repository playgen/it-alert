using UnityEngine;
using System.Collections;
using PlayGen.ITAlert.Network;

public class VoiceController
{
    private ITAlertClient _client;

    public VoiceController(ITAlertClient client)
    {
        _client = client;
    }

    public void HandleVoiceInput()
    {
        if (Input.GetKey(KeyCode.Tab) && !_client.VoiceClient.IsTransmitting)
        {
            _client.VoiceClient.StartTransmission();
        }
        else if (!Input.GetKey(KeyCode.Tab) && _client.VoiceClient.IsTransmitting)
        {
            _client.VoiceClient.StopTransmission();
        }
    }
}
