using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameWork.Core.States.Tick.Input;
using ModestTree;
using PlayGen.ITAlert.Unity.Components;
using PlayGen.ITAlert.Unity.Simulation.Summary;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Unity.Utilities.BestFit;
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
            //SummaryMetricConfigs.Moved,
            SummaryMetricConfigs.TransfersSent,
            SummaryMetricConfigs.TransfersRecieved,
            //SummaryMetricConfigs.AntivirusesUsed,
            //SummaryMetricConfigs.ScannersUsed,
            SummaryMetricConfigs.VirusesKilled,
            SummaryMetricConfigs.AntivirusesWasted,
            SummaryMetricConfigs.VirusesFound,
            SummaryMetricConfigs.ScansWithNoVirusesFound,
            //SummaryMetricConfigs.CapturesUsed,
            SummaryMetricConfigs.VirusesCaptured,
            SummaryMetricConfigs.CaptureWithNoVirusCaught,
            //SummaryMetricConfigs.AnalysersUsed,
            SummaryMetricConfigs.AntivirusesCreated,
            SummaryMetricConfigs.AntivirusCreationFails
        };

        private readonly List<SummaryMetricConfig> _singleplayerMetrics = new List<SummaryMetricConfig>
        {
            //SummaryMetricConfigs.Spoke,
            SummaryMetricConfigs.Moved,
            //SummaryMetricConfigs.TransfersSent,
            //SummaryMetricConfigs.TransfersRecieved,
            //SummaryMetricConfigs.AntivirusesUsed,
            //SummaryMetricConfigs.ScannersUsed,
            SummaryMetricConfigs.VirusesKilled,
            //SummaryMetricConfigs.AntivirusesWasted,
            SummaryMetricConfigs.VirusesFound,
            SummaryMetricConfigs.ScansWithNoVirusesFound,
            //SummaryMetricConfigs.CapturesUsed,
            SummaryMetricConfigs.VirusesCaptured,
            SummaryMetricConfigs.CaptureWithNoVirusCaught,
            //SummaryMetricConfigs.AnalysersUsed,
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
            var bestFitGroups = PopulateMetrics();
            _continueButton.onClick.AddListener(OnContinueClicked);
            _panel.SetActive(true);
            bestFitGroups.ForEach(bfg => bfg.BestFit());
        }

        private IEnumerable<List<Text>> PopulateMetrics()
        {
            var metricConfigs = _simulationSummary.PlayersData.Count > 1
                ? _multiplayerMetrics
                : _singleplayerMetrics;

            var sumByPlayerByMetrics = MetricProcessor.GetSumByPlayerByMetric(_simulationSummary.Events);

            var playerNameBestFitGroup = new List<Text>();
            var metricTitleBestFitGroup = new List<Text>();
            var metricValueBestFitGroup = new List<Text>();

            var column = CreateColumn();
            AddPlayers(column, _simulationSummary.PlayersData, playerNameBestFitGroup);
            playerNameBestFitGroup.ForEach(playerText => playerText.gameObject.AddComponent<TextCutoff>());

            foreach (var metricConfig in metricConfigs)
            {
                sumByPlayerByMetrics.TryGetValue(metricConfig.Key, out var valueByPlayer);

                column = CreateColumn();
                SetLocalizedTitle(column, metricConfig.Key, metricTitleBestFitGroup);
                SetIcon(column, metricConfig.IconPath);
                AddItems(
                    column,
                    _simulationSummary.PlayersData, 
                    valueByPlayer, 
                    0, 
                    metricConfig.HighlightHighest,
                    metricValueBestFitGroup);
            }

	        PlayerMetrics.GetPlayerBestMetrics(sumByPlayerByMetrics);

            return new[] {playerNameBestFitGroup, metricTitleBestFitGroup, metricValueBestFitGroup};
        }

		private void AddPlayers(GameObject column, IReadOnlyList<SimulationSummary.PlayerData> playersData, List<Text> rowItemTexts)
        {
            foreach (var playerData in playersData)
            {
                var (rowItem, rowText) = AddRowItem(column, rowItemTexts);

                if (ColorUtility.TryParseHtmlString(playerData.Colour, out var colour))
                {
                    rowText.color = colour;
                }

                rowText.text = playerData.Name;
            }
        }

        private void AddItems(
            GameObject column, 
            IReadOnlyList<SimulationSummary.PlayerData> playersData, 
            Dictionary<int?, int> valueByPlayer, 
            int defaultValue, 
            bool? highlightHighest,
            List<Text> rowItemTexts)
        {
            List<Image> extremeValueRowHighlights = null;
            int? extremeValue = null;

            foreach (var playerData in playersData)
            {
                var (rowItem, rowText) = AddRowItem(column, rowItemTexts);
                var rowHighlight = rowItem.transform.Find("Highlight").GetComponent<Image>();

                if (ColorUtility.TryParseHtmlString(playerData.Colour, out var colour))
                {
                    rowText.color = colour;
                    rowHighlight.color = colour;
                }

                if (valueByPlayer == null || !valueByPlayer.TryGetValue(playerData.Id, out var value))
                {
                    value = defaultValue;
                }
                else if(highlightHighest != null)
                {
                    if (extremeValue == null)
                    {
                        extremeValue = value;
                        extremeValueRowHighlights = new List<Image> { rowHighlight };
                    }
                    else if(extremeValue == value)
                    {
                        extremeValueRowHighlights.Add(rowHighlight);
                    }
                    else if(highlightHighest.Value && value > extremeValue)
                    {
                        extremeValue = value;
                        extremeValueRowHighlights = new List<Image> { rowHighlight };
                    }
                    else if(!highlightHighest.Value && value <= extremeValue)
                    {
                        extremeValue = value;
                        extremeValueRowHighlights = new List<Image> { rowHighlight };
                    }
                }

                rowText.text = value.ToString();
            }

            extremeValueRowHighlights?.ForEach(rowHighlight =>
            {
                rowHighlight.gameObject.SetActive(true);
            });
        }

        private (GameObject, Text) AddRowItem(GameObject column, List<Text> bestFitGroup)
        {
            var rowItem = Object.Instantiate(_rowItemResource, column.transform);
            var rowText = rowItem.transform.Find("Text").GetComponent<Text>();
            bestFitGroup.Add(rowText);

            return (rowItem, rowText);
        }
        private void SetIcon(GameObject column, string iconPath)
        {
            var image = column.transform.Find("IconContainer/Icon").GetComponent<Image>();
            var sprite = Resources.Load<Sprite>(iconPath);
            image.sprite = sprite;
            image.gameObject.SetActive(true);
        }

        private void SetLocalizedTitle(GameObject column, string localizationKey, List<Text> bestFitGroup)
        {
            var titleText = column.transform.Find("Title").GetComponent<Text>();
            titleText.text = Localization.Get(localizationKey);
            bestFitGroup.Add(titleText);
        }

        private GameObject CreateColumn()
        {
            var column = Object.Instantiate(_columnResource, _columnContainer);
            return column;
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
	}
}
