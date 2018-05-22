using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.States.Game.SimulationSummary
{
    public class SimulationSummaryStateInput : TickStateInput
    {
        private readonly SimulationSummary _simulationSummary;
        private GameObject _panel;
        private Button _continueButton;
        private Transform _columnContainer;
        private GameObject _columnResource;
        private GameObject _rowItemResource;

        public event Action ContinueClickedEvent;

        public SimulationSummaryStateInput(SimulationSummary simulationSummary)
        {
            _simulationSummary = simulationSummary;
        }

        protected override void OnInitialize()
        {
            _panel = GameObjectUtilities.FindGameObject("Menu/SimulationSummaryContainer/SimulationSummaryPanelContainer");
            _columnContainer = _panel.transform.Find("ColumnContainer");
            _continueButton = _panel.transform.Find("ButtonPanel/ContinueButtonContainer").GetComponent<Button>();
            _columnResource = Resources.Load<GameObject>("SimulationSummaryColumn");
            _rowItemResource = Resources.Load<GameObject>("SimulationSummaryRowItem");
        }

        protected override void OnEnter()
        {
            _continueButton.onClick.AddListener(OnContinueClicked);
            _panel.SetActive(true);

            var sumByPlayerByMetrics = EventProcessor.GetSumByPlayerByMetric(_simulationSummary.Events);
            
            AddColumn(null, _simulationSummary.PlayersData.ToDictionary(pd => pd.Id, pd => pd.Name), _simulationSummary.PlayersData);

            foreach (var item in sumByPlayerByMetrics)
            {
                AddColumn(item.Key, item.Value.ToDictionary(i => i.Key, i => i.Value.ToString()), _simulationSummary.PlayersData, "0");
            }
        }

        private void AddColumn(
            string title, 
            Dictionary<int?, string> valueByPlayer, 
            IReadOnlyList<SimulationSummary.PlayerData> playersData, 
            string placeholderValue = null)
        {
            var column = Object.Instantiate(_columnResource, _columnContainer);

            // Title
            if (!string.IsNullOrWhiteSpace(title))
            {
                // todo localization
                column.transform.Find("Title").GetComponent<Text>().text = title;
            }

            // Items
            foreach (var playerData in playersData)
            {
                var rowItem = Object.Instantiate(_rowItemResource, column.transform);
                if (!valueByPlayer.TryGetValue(playerData.Id, out var value))
                {
                    value = placeholderValue;
                }

                var rowText = rowItem.GetComponent<Text>();
                rowText.text = value;

                if (ColorUtility.TryParseHtmlString(playerData.Colour, out var colour))
                {
                    rowText.color = colour;
                }
            }
        }

        protected override void OnExit()
        {
            _continueButton.onClick.RemoveListener(OnContinueClicked);
            _panel.SetActive(false);

            // Remove all columns
            foreach (Object column in _columnContainer)
            {
                Object.Destroy(column);
            }
        }
        
        private void OnContinueClicked()
        {
            ContinueClickedEvent.Invoke();
        }

        private void AddColumn()
        {

        }
    }
}
