using System;
using UnityEngine;
using UnityEngine.EventSystems;
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
        GameNameInputField.text = Guid.NewGuid().ToString().Substring(0,8);
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift) && EventSystem.current.currentSelectedGameObject != null)
        {
            var next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();

            if (next == null)
                return;

            var inputfield = next.GetComponent<InputField>();
            if (inputfield != null)
            {
                inputfield.OnPointerClick(new PointerEventData(EventSystem.current));
            }
            EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && EventSystem.current.currentSelectedGameObject != null)
        {
            var next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next == null)
                return;

            var inputfield = next.GetComponent<InputField>();
            if (inputfield != null)
            {
                inputfield.OnPointerClick(new PointerEventData(EventSystem.current));
            }
            EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
        }
    }
}

