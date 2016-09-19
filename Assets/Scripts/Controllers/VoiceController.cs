﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Network;

public class VoiceController
{
    private readonly VoiceClient _voiceClient;

    public VoiceController(ITAlertClient client)
    {
        _voiceClient = client.VoiceClient;
    }

    public void HandleVoiceInput()
    {
        if (Input.GetKey(KeyCode.Tab) && !_voiceClient.IsTransmitting)
        {
            _voiceClient.StartTransmission();
        }
        else if (!Input.GetKey(KeyCode.Tab) && _voiceClient.IsTransmitting)
        {
            _voiceClient.StopTransmission();
        }
    }
}