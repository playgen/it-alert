using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;

using PlayGen.ITAlert.Photon.Players;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackStateInterface : StateInterface
{
    private GameObject _feedbackPanel;
    private GameObject _columnPrefab;
    private GameObject _entryPrefab;
    private GameObject _slotPrefab;
    private ButtonList _buttons;

    public override void Initialize()
    {
        _feedbackPanel = GameObjectUtilities.FindGameObject("FeedbackContainer/FeedbackPanelContainer/FeedbackPanel");
        _columnPrefab = Resources.Load("FeedbackColumn") as GameObject;
        _entryPrefab = Resources.Load("FeedbackEntry") as GameObject;
        _slotPrefab = Resources.Load("FeedbackSlot") as GameObject;
        _buttons = new ButtonList("FeedbackContainer/FeedbackPanelContainer/FeedbackButtons");

        var sendButton = _buttons.GetButton("SendButtonContainer");
        sendButton.onClick.AddListener(OnSendClick);
    }

    private void OnSendClick()
    {
        //Todo: Send selected feedback to SUGAR
        EnqueueCommand(new NextStateCommand());
    }

    public override void Enter()
    {
        _feedbackPanel.transform.parent.gameObject.SetActive(true);
        _buttons.BestFit();
    }

    public override void Exit()
    {
        _feedbackPanel.transform.parent.gameObject.SetActive(false);
    }

    public void PopulateFeedback(Player[] players)
    {
        foreach (Transform child in _feedbackPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //To-Do: Get the list of evaluation criteria from somewhere

        string[] evaluationSections = new[] { "Cooperation", "Leadership", "Communication" };

        var playerList = Object.Instantiate(_columnPrefab);
        playerList.transform.SetParent(_feedbackPanel.transform, false);
        var emptySlot = Object.Instantiate(_slotPrefab);
        emptySlot.transform.SetParent(playerList.transform, false);
        emptySlot.GetComponent<Image>().enabled = false;

        foreach (var player in players)
        {
            var color = new Color();
            ColorUtility.TryParseHtmlString("#" + player.Color, out color);

            var playerSlot = Object.Instantiate(_slotPrefab);
            playerSlot.transform.SetParent(playerList.transform, false);
            playerSlot.GetComponent<Image>().enabled = false;
            var playerObj = Object.Instantiate(_entryPrefab);
            playerObj.transform.SetParent(playerSlot.transform, false);
            playerObj.GetComponent<Text>().text = player.Name;
            playerObj.GetComponent<Text>().color = color;
        }

        foreach (var section in evaluationSections)
        {
            var sectionList = Object.Instantiate(_columnPrefab);
            sectionList.transform.SetParent(_feedbackPanel.transform, false);

            var headerObj = Object.Instantiate(_entryPrefab);
            headerObj.transform.SetParent(sectionList.transform, false);
            headerObj.GetComponent<Text>().text = section;

            foreach (var player in players)
            {
                var color = new Color();
                ColorUtility.TryParseHtmlString("#" + player.Color, out color);

                var playerSlot = Object.Instantiate(_slotPrefab);
                playerSlot.transform.SetParent(sectionList.transform, false);
            }
        }
        RebuildLayout();
    }

    private void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_feedbackPanel.transform);

        var textObjs = _feedbackPanel.GetComponentsInChildren<Text>();
        int smallestFontSize = 0;
        foreach (var text in textObjs)
        {
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 1;
            text.resizeTextMaxSize = 100;
            text.cachedTextGenerator.Invalidate();
            text.cachedTextGenerator.Populate(text.text, text.GetGenerationSettings(text.rectTransform.rect.size));
            text.resizeTextForBestFit = false;
            var newSize = text.cachedTextGenerator.fontSizeUsedForBestFit;
            var newSizeRescale = text.rectTransform.rect.size.x / text.cachedTextGenerator.rectExtents.size.x;
            if (text.rectTransform.rect.size.y / text.cachedTextGenerator.rectExtents.size.y < newSizeRescale)
            {
                newSizeRescale = text.rectTransform.rect.size.y / text.cachedTextGenerator.rectExtents.size.y;
            }
            newSize = Mathf.FloorToInt(newSize * newSizeRescale);
            if (newSize < smallestFontSize || smallestFontSize == 0)
            {
                smallestFontSize = newSize;
            }
        }
        foreach (var text in textObjs)
        {
            text.fontSize = smallestFontSize;
        }
    }
}