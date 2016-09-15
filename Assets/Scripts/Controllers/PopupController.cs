using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PopupController
{
    private GameObject _popupPanel;
    private PopupBehaviour _popupBehaviour;

    public PopupController()
    {
        _popupPanel = GameObject.Find("PopupContainer").transform.GetChild(0).gameObject;
        _popupBehaviour = _popupPanel.GetComponent<PopupBehaviour>();
    }

    public void ShowErrorPopup(string msg)
    {
        // Show error on popup
        var errorPanel = Object.Instantiate(Resources.Load("Prefabs/ErrorContentPanel")) as GameObject;
        var errorMsg = errorPanel.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
        errorMsg.text = msg;

        _popupBehaviour.ClearContent();
        _popupBehaviour.SetPopup("Error", new[] { new PopupBehaviour.Output("OK", null) }, PopupClosed);
        _popupBehaviour.SetContent(errorPanel.GetComponent<RectTransform>());

        _popupPanel.gameObject.SetActive(true);
    }

    public void ShowLoadingPopup(/*UnityAction cancelAction = null*/)
    {
        // Show the loading popup along with a button to cancel
        var loadingPanel = Object.Instantiate(Resources.Load("Prefabs/LoadingContentPanel")) as GameObject;

        _popupBehaviour.ClearContent();
        _popupBehaviour.SetPopup("", null/*new[] { new PopupBehaviour.Output("Cancel", null) }*/, PopupClosed);
        _popupBehaviour.SetContent(loadingPanel.GetComponent<RectTransform>());

        _popupPanel.gameObject.SetActive(true);
    }
    public void HideLoadingPopup()
    {
        PopupClosed();
    }
    private void PopupClosed()
    {
        _popupPanel.gameObject.SetActive(false);
    }
}
