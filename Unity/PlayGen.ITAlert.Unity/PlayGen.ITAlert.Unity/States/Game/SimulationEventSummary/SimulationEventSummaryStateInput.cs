using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameWork.Core.States.Input;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.States.Game.SimulationEventSummary
{
    public class SimulationEventSummaryStateInput : StateInput
    {
        private readonly SimulationSummary _simulationSummary;
        private GameObject _panel;

        public event Action ContinueClickedEvent;

        public SimulationEventSummaryStateInput(SimulationSummary simulationSummary)
        {
            _simulationSummary = simulationSummary;
        }

        protected override void OnInitialize()
        {
            _panel = GameObjectUtilities.FindGameObject("SimulationEventSummaryContainer/SimulationEventSummaryPanelContainer/SimulationEventSummaryPanel");
        }

        protected override void OnEnter()
        {
            /*_sendButton.onClick.AddListener(OnSendClick);

            PopulateFeedback(_director.Players, _director.Player.PhotonId);
            _feedbackPanel.transform.parent.gameObject.SetActive(true);
            _buttons.Buttons.BestFit();
            _sendButton.gameObject.SetActive(true);*/

            Task.Delay(5000).ContinueWith(_ => ContinueClickedEvent?.Invoke());
        }

        protected override void OnExit()
        {
            /*_sendButton.onClick.RemoveListener(OnSendClick);
            _feedbackPanel.transform.parent.gameObject.SetActive(false);*/
        }
    }
}
