using System;
using UnityEngine;
using UnityEngine.UI;

public class CreateGamePopupBehaviour : MonoBehaviour
{
    public InputField GameNameInputField;
    public InputField PlayerNumberInputField;

    private int _maxPlayers = 6;

    public GameDetails GetGameDetails()
    {
        return new GameDetails(GameNameInputField.text, PlayerNumberInputField.text);
    }


    public void ResetFields()
    {
        GameNameInputField.text = "";
        PlayerNumberInputField.text = "";
    }

    public void CheckPlayerNumberInput()
    {
        var playerInput = Convert.ToInt32(PlayerNumberInputField.text);
        // clamp input
        playerInput = Mathf.Clamp(playerInput, 1, _maxPlayers);
        // Set Text to clamped value
        PlayerNumberInputField.text = playerInput.ToString();
    }

    public struct GameDetails
    {
        public string GameName;
        public int MaxPlayers;

        public GameDetails(string gameName, string maxPlayers)
        {
            GameName = gameName;
            MaxPlayers = int.Parse(maxPlayers);
        }
    }
}

