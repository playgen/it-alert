using PlayGen.ITAlert.Network;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonVoiceRecorder))]
[RequireComponent(typeof(PhotonVoiceSpeaker))]
public class VoicePlayer : MonoBehaviour
{
    private PhotonView _photonView;
    private PhotonVoiceRecorder _photonVoiceRecorder;
    private PhotonVoiceSpeaker _photonVoiceSpeaker;
    
    public bool IsRecording { get; private set; }

    public bool IsOutputting { get; private set; }

    public int OwnerId
    {
        get { return _photonView.ownerId; }
    }

    private void OnEnable()
    {
        _photonView = GetComponent<PhotonView>();
        _photonVoiceRecorder = GetComponent<PhotonVoiceRecorder>();
        _photonVoiceSpeaker = GetComponent<PhotonVoiceSpeaker>();

        VoiceClient.ReisterVoicePlayer(_photonView.ownerId, this);
    }

    void Update()
    {
        IsRecording = _photonVoiceRecorder != null && _photonVoiceRecorder.IsTransmitting
            && PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined;

        IsOutputting = _photonVoiceSpeaker != null && _photonVoiceSpeaker.IsPlaying
            && PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined;
    }
}
