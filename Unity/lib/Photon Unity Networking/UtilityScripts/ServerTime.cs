using System;
using UnityEngine;

public class ServerTime : MonoBehaviour
{
    // Update is called once per frame
	private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width/2-100, 0, 200,30));
        GUILayout.Label(string.Format("Time Offset: {0}", PhotonNetwork.ServerTimestamp - Environment.TickCount));
        if (GUILayout.Button("fetch"))
        {
            PhotonNetwork.FetchServerTimestamp();
        }
        GUILayout.EndArea();
    }
}
