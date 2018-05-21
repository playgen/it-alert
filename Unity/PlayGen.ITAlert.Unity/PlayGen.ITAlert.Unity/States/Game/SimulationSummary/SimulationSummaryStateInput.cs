using System;
using System.Threading.Tasks;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.States.Game.SimulationSummary
{
    public class SimulationSummaryStateInput : TickStateInput
    {
        private readonly SimulationSummary _simulationSummary;
        private GameObject _panel;
        private Button _continueButton;

        public event Action ContinueClickedEvent;

        public SimulationSummaryStateInput(SimulationSummary simulationSummary)
        {
            _simulationSummary = simulationSummary;
        }

        protected override void OnInitialize()
        {
            _panel = GameObjectUtilities.FindGameObject("Menu/SimulationSummaryContainer/SimulationSummaryPanelContainer");
            _continueButton = _panel.transform.Find("ContinueButtonContainer").GetComponent<Button>();
        }

        protected override void OnEnter()
        {
            _continueButton.onClick.AddListener(OnContinueClicked);
            _panel.SetActive(true);

            /*PopulateFeedback(_director.Players, _director.Player.PhotonId);
            _feedbackPanel.transform.parent.gameObject.SetActive(true);
            _buttons.Buttons.BestFit();
            _sendButton.gameObject.SetActive(true); */

            //Task.Delay(5000).ContinueWith(_ => ContinueClickedEvent?.Invoke());
        }

        protected override void OnExit()
        {
            _continueButton.onClick.RemoveListener(OnContinueClicked);
            _panel.SetActive(false);
        }

        private void OnContinueClicked()
        {
            ContinueClickedEvent.Invoke();
        }
    }
}
