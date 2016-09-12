using UnityEngine;
using UnityEngine.UI;

public class CreateGamePopupBehaviour : MonoBehaviour
{
    public InputField GameNameInputField;
    public InputField PlayerNumberInputField;

    public GameDetails GetGameDetails()
    {
        return new GameDetails(GameNameInputField.text, PlayerNumberInputField.text);
    }


    public void ResetFields()
    {
        GameNameInputField.text = "";
        PlayerNumberInputField.text = "";
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

