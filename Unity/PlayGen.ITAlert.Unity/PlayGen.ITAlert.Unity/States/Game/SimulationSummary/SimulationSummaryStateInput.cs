using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Simulation.Summary;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Unity.Utilities.Localization;
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

        private readonly List<SummaryMetricConfig> _multiplayerMetrics = new List<SummaryMetricConfig>
        {
            SummaryMetricConfigs.Spoke,
            SummaryMetricConfigs.Moved,
            SummaryMetricConfigs.TransfersSent,
            SummaryMetricConfigs.TransfersRecieved,
            SummaryMetricConfigs.AntivirusesUsed,
            SummaryMetricConfigs.ScannersUsed,
            SummaryMetricConfigs.VirusesKilled,
            SummaryMetricConfigs.AntivirusesWasted,
            SummaryMetricConfigs.VirusesFound,
            SummaryMetricConfigs.ScansWithNoVirusesFound,
            SummaryMetricConfigs.CapturesUsed,
            SummaryMetricConfigs.VirusesCaptured,
            SummaryMetricConfigs.CaptureWithNoVirusCaught,
            SummaryMetricConfigs.AnalysersUsed,
            SummaryMetricConfigs.AntivirusesCreated,
            SummaryMetricConfigs.AntivirusCreationFails
        };

        private readonly List<SummaryMetricConfig> _singleplayerMetrics = new List<SummaryMetricConfig>
        {
            SummaryMetricConfigs.Moved,
            SummaryMetricConfigs.AntivirusesUsed,
            SummaryMetricConfigs.ScannersUsed,
            SummaryMetricConfigs.VirusesKilled,
            SummaryMetricConfigs.AntivirusesWasted,
            SummaryMetricConfigs.VirusesFound,
            SummaryMetricConfigs.ScansWithNoVirusesFound,
            SummaryMetricConfigs.CapturesUsed,
            SummaryMetricConfigs.VirusesCaptured,
            SummaryMetricConfigs.CaptureWithNoVirusCaught,
            SummaryMetricConfigs.AnalysersUsed,
            SummaryMetricConfigs.AntivirusesCreated,
            SummaryMetricConfigs.AntivirusCreationFails
        };

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
            PopulateMetrics();
            _continueButton.onClick.AddListener(OnContinueClicked);
            _panel.SetActive(true);
        }

        private void PopulateMetrics()
        {
            var metricConfigs = _simulationSummary.PlayersData.Count > 1
                ? _multiplayerMetrics
                : _singleplayerMetrics;

            var sumByPlayerByMetrics = EventProcessor.GetSumByPlayerByMetric(_simulationSummary.Events);

            AddColumn(null, _simulationSummary.PlayersData.ToDictionary(pd => pd.Id, pd => pd.Name), _simulationSummary.PlayersData);

            foreach (var metricConfig in metricConfigs)
            {
                sumByPlayerByMetrics.TryGetValue(metricConfig.Key, out var valueByPlayer);
                AddColumn(metricConfig.Key, valueByPlayer?.ToDictionary(i => i.Key, i => i.Value.ToString()), _simulationSummary.PlayersData, "0");
            }
        }

        private void AddColumn(
            string titleKey,
            Dictionary<int?, string> valueByPlayer,
            IReadOnlyList<SimulationSummary.PlayerData> playersData,
            string placeholderValue = null)
        {
            var column = Object.Instantiate(_columnResource, _columnContainer);

            // Title
            if (!string.IsNullOrWhiteSpace(titleKey))
            {
                column.transform.Find("Title").GetComponent<Text>().text = Localization.Get(titleKey);
            }

            // Items
            foreach (var playerData in playersData)
            {
                var rowItem = Object.Instantiate(_rowItemResource, column.transform);
                if (valueByPlayer == null || !valueByPlayer.TryGetValue(playerData.Id, out var value))
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
