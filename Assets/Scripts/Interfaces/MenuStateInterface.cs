using System.Linq;
using GameWork.Commands;
using GameWork.Interfacing;
using UnityEngine;
using UnityEngine.UI;

public class MenuStateInterface : StateInterface
{
    private GameObject _mainMenuPanel;

    public override void Initialize()
    {
        _mainMenuPanel = GameObject.Find("MainMenuContainer").transform.GetChild(0).gameObject;
        var menu = new ButtonList("MainMenuContainer/MenuPanelContainer/MenuContainer");

        var logoutButton = menu.GetButton("LogoutButtonContainer");
        logoutButton.onClick.AddListener(OnLogoutClick);

        var quickMatchButton = menu.GetButton("QuickMatchButtonContainer");
        quickMatchButton.onClick.AddListener(OnQuickMatchClick);
    }

    private void OnQuickMatchClick()
    {
        EnqueueCommand(new NextStateCommand());
    }

    private void OnLogoutClick()
    {
        //EnqueueCommand(new LogoutCommand());
        EnqueueCommand(new PreviousStateCommand());
    }


    public override void Enter()
    {
        _mainMenuPanel.SetActive(true);
    }

    public override void Exit()
    {
        _mainMenuPanel.SetActive(false);
    }
}
