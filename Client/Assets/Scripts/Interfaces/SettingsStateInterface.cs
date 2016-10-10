using UnityEngine;
using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using UnityEngine.UI;

public class SettingsStateInterface : StateInterface
{
    private GameObject _settingsPanel;

    public override void Initialize()
    {
        // Join Game Popup
        _settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer");
        var panelButtons = new ButtonList("SettingsContainer/SettingsPanelContainer/ButtonPanel");

        var backButton = panelButtons.GetButton("BackButtonContainer");
        backButton.onClick.AddListener(OnBackClick);
    }

    private void OnBackClick()
    {
        EnqueueCommand(new PreviousStateCommand());
    }

    public override void Enter()
    {
        _settingsPanel.SetActive(true);
    }
    public override void Exit()
    {
        _settingsPanel.SetActive(false);
    }
}
