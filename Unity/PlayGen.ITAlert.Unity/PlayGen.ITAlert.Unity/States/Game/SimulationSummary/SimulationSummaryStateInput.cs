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

	    private Transform _playerMetricsContainer;
	    private GameObject _playerMetricsResource;


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
	        _playerMetricsContainer = _panel.transform.Find("PlayerMetricsPanel");
			_continueButton = _panel.transform.Find("ButtonPanel/ContinueButtonContainer").GetComponent<Button>();
            _columnResource = Resources.Load<GameObject>("SimulationSummaryColumn");
            _rowItemResource = Resources.Load<GameObject>("SimulationSummaryRowItem");
	        _playerMetricsResource = Resources.Load<GameObject>("PlayerMetricContainer");
        }

        protected override void OnEnter()
        {
	        _panel.SetActive(true);
			
			var bestFitGroups = PopulateMetrics();
            _continueButton.onClick.AddListener(OnContinueClicked);
            bestFitGroups.ForEach(bfg => bfg.BestFit());
        }

        protected override void OnExit()
        {
            _continueButton.onClick.RemoveListener(OnContinueClicked);
            _panel.SetActive(false);

            // Remove all columns
            foreach (Transform column in _columnContainer)
            {
                Object.Destroy(column.gameObject);
            }
        }

        private void OnContinueClicked()
        {
            ContinueClickedEvent.Invoke();
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
	        playerNameBestFitGroup.ForEach(playerText => playerText.gameObject.AddComponent<TextCutoff>());

			AddPlayers(column.GetComponentInChildren<LayoutGroup>().gameObject, _simulationSummary.PlayersData, playerNameBestFitGroup);

            foreach (var metricConfig in metricConfigs)
            {
                sumByPlayerByMetrics.TryGetValue(metricConfig.KeyMetric, out var valueByPlayer);

                column = CreateColumn();
                SetLocalizedTitle(column, metricConfig.KeyMetric, metricTitleBestFitGroup);
                SetIcon(column, metricConfig.IconPath);
                AddItems(
                    column.GetComponentInChildren<LayoutGroup>().gameObject,
                    _simulationSummary.PlayersData, 
                    valueByPlayer, 
                    0, 
                    metricConfig.HighlightHighest,
                    metricValueBestFitGroup);
            }
	        var playerMetrics = PlayerMetrics.GetPlayerBestMetrics(sumByPlayerByMetrics, _simulationSummary);
	        AddMetricItems(playerMetrics);
            return new[] {playerNameBestFitGroup, metricTitleBestFitGroup, metricValueBestFitGroup};
        }

		private void AddPlayers(GameObject column, IReadOnlyList<SimulationSummary.PlayerData> playersData, List<Text> bestFitGroup)
        {
            foreach (var playerData in playersData)
            {
                var (rowItem, rowText, background) = AddRowItem(column, bestFitGroup);

				if (ColorUtility.TryParseHtmlString(playerData.Colour, out var colour))
                {
					//rowText.color = colour;
	                background.color = colour;
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
            List<Text> rowBestFitGroup)
        {
            List<Image> extremeValueRowHighlights = null;
            int? extremeValue = null;

            foreach (var playerData in playersData)
            {
                var (rowItem, rowText, background) = AddRowItem(column, rowBestFitGroup);
                var rowHighlight = rowItem.transform.Find("Highlight").GetComponent<Image>();

                if (ColorUtility.TryParseHtmlString(playerData.Colour, out var colour))
                {
                    //rowText.color = colour;
                    rowHighlight.color = colour;
	                background.color = colour;
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

	    private void AddMetricItems(List<PlayerMetrics.PlayerMetric> playerMetrics)
	    {
			var bestFitGroupName = new List<Text>();
			var bestFitGroupTitle = new List<Text>();
			var bestFitGroupDescription = new List<Text>();
			foreach (var playerMetric in playerMetrics)
		    {
			    var metricConfig = SummaryMetricConfigs.GetAll().First(c => c.KeyMetric == playerMetric.Metric);
				var metricItem = Object.Instantiate(_playerMetricsResource, _playerMetricsContainer.transform);

			    var nameText = metricItem.transform.Find("PlayerName").GetComponent<Text>();
			    var titleText = metricItem.transform.Find("PlayerTitle").GetComponent<Text>();
			    var descriptionText = metricItem.transform.Find("PlayerMetric").GetComponent<Text>();

				nameText.text = playerMetric.PlayerData.Name;
				titleText.text = Localization.Get(metricConfig.KeyTitle);
				descriptionText.text = Localization.GetAndFormat(metricConfig.KeyFormattedMetric, false, playerMetric.Score.ToString());

				if (ColorUtility.TryParseHtmlString(playerMetric.PlayerData.Colour, out var colour))
			    {
				    nameText.color = colour;
				    //titleText.color = colour;
				    descriptionText.color = colour;
			    }

				bestFitGroupName.Add(nameText);
				bestFitGroupTitle.Add(titleText);
				bestFitGroupDescription.Add(descriptionText);
			}

			bestFitGroupName.BestFit();
		    bestFitGroupTitle.BestFit();
		    bestFitGroupDescription.BestFit();
	    }

        private (GameObject, Text, Image) AddRowItem(GameObject column, List<Text> bestFitGroup)
        {
            var rowItem = Object.Instantiate(_rowItemResource, column.transform);
            var rowText = rowItem.transform.Find("Text").GetComponent<Text>();
			var background = rowItem.transform.Find("Background").GetComponent<Image>();
			bestFitGroup.Add(rowText);

            return (rowItem, rowText, background);
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
	}
}
