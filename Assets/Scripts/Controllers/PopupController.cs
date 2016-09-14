using UnityEngine;
using System.Collections;

public class PopupController
{
    private GameObject _popupPanel;
    private PopupBehaviour _popupBehaviour;

    public PopupController()
    {
        _popupPanel = GameObject.Find("PopupContainer").transform.GetChild(0).gameObject;
        _popupBehaviour = _popupPanel.GetComponent<PopupBehaviour>();
    }

    public void ShowPopup(string msg)
    {
        // TODO show error on popup
        var errorPanel = Object.Instantiate(Resources.Load("Prefabs/ErrorContentPanel")) as GameObject;
        var errorMsg = errorPanel.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
        errorMsg.text = msg;

        _popupBehaviour.ClearContent();
        _popupBehaviour.SetPopup("Error", new[] { new PopupBehaviour.Output("OK", PopupClosed) });
        _popupBehaviour.SetContent(errorPanel.GetComponent<RectTransform>());

        _popupPanel.gameObject.SetActive(true);
    }

    private void PopupClosed()
    {
        _popupPanel.gameObject.SetActive(false);
    }
}
