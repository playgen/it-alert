using UnityEngine;

public class OnClickInstantiate : MonoBehaviour
{
    public GameObject Prefab;
    public int InstantiateType;
    private string[] InstantiateTypeNames = {"Mine", "Scene"};

    public bool showGui;

	private void OnClick()
    {
        if (!PhotonNetwork.inRoom)
        {
            // only use PhotonNetwork.Instantiate while in a room.
            return;
        }

        switch (InstantiateType)
        {
            case 0:
                PhotonNetwork.Instantiate(Prefab.name, InputToEvent.inputHitPos + new Vector3(0, 5f, 0), Quaternion.identity, 0);
                break;
            case 1:
                PhotonNetwork.InstantiateSceneObject(Prefab.name, InputToEvent.inputHitPos + new Vector3(0, 5f, 0), Quaternion.identity, 0, null);
                break;
        }
    }

	private void OnGUI()
    {
        if (showGui)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 180, 0, 180, 50));
            InstantiateType = GUILayout.Toolbar(InstantiateType, InstantiateTypeNames);
            GUILayout.EndArea();
        }
    }


}
