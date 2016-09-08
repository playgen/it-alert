using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ButtonList
{
    private GameObject[] _buttons;

    public ButtonList(string menuPath)
    {
        _buttons = GameObjectUtilities.FindAllChildren(menuPath);
    }

    public Button GetButton(string containerName)
    {
        return _buttons.First(o => o.name.Equals(containerName)).transform.GetChild(0).GetComponent<Button>();
    }
}
