using System;
using System.Collections.Generic;
using System.Linq;

using GameWork.Core.States.Tick.Input;
using ModestTree;
using PlayGen.ITAlert.Photon.Common;
using PlayGen.ITAlert.Unity.Components;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation.Summary;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Extensions;
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

	    private ITAlertPhotonClient _photonClient;
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

        public SimulationSummaryStateInput(SimulationSummary simulationSummary, ITAlertPhotonClient photonClient)
        {
            _simulationSummary = simulationSummary;
	        _photonClient = photonClient;
        }

        protected override void OnInitialize()
        {
            _panel = GameObjectUtilities.FindGameObject("Menu/SimulationSummaryContainer/SimulationSummaryPanelContainer");
            _columnContainer = _panel.transform.Find("ColumnContainer");
	        _playerMetricsContainer = _panel.transform.Find("PlayerMetricsPanel");
			_continueButton = _panel.transform.FindButton("ButtonPanel/ContinueButtonContainer");
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
			foreach (Transform player in _playerMetricsContainer)
			{
				Object.Destroy(player.gameObject);
			}
		}

        private void OnContinueClicked()
        {
			if (_photonClient.CurrentRoom.RoomInfo.customProperties[CustomRoomSettingKeys.GameScenario].ToString() == "SPL3")
	        {
		        // The following string contains the key for the google form is used for the cognitive load questionnaire
		        var formsKey = "1FAIpQLSctM-kR-1hlmF6Nk-pQNIWYnFGxRAVvyP6o3ZV0kr8K7JD5dQ";

		        // Google form ID
		        var googleFormsURL = "https://docs.google.com/forms/d/e/"
		                             + formsKey
		                             + "/viewform?entry.1596836094="
		                             + SUGARManager.CurrentUser.Name;
		        // Open the default browser and show the form
		        Application.OpenURL(googleFormsURL);
		        Application.Quit();
	        }
			ContinueClickedEvent?.Invoke();
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
			// Set players box to be wider than the rest
			var layoutElement = column.GetComponent<LayoutElement>();
	        layoutElement.preferredWidth = layoutElement.preferredWidth * 3;
	        layoutElement.minWidth = layoutElement.minWidth * 3;

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
				rowText.text = playerData.Name.Cutoff(TextCutoff.GlobalCutoffAfter, TextCutoff.GlobalMaxLength);
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
                var rowHighlight = rowItem.transform.FindImage("Highlight");

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
                if(highlightHighest != null)
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

			    var nameText = metricItem.transform.FindText("PlayerName");
			    var titleText = metricItem.transform.FindText("PlayerTitle");
			    var descriptionText = metricItem.transform.FindText("PlayerMetric");

				nameText.text = playerMetric.PlayerData.Name.Cutoff(TextCutoff.GlobalCutoffAfter, TextCutoff.GlobalMaxLength);
				titleText.text = Localization.Get(metricConfig.KeyTitle);
			    //descriptionText.text = Localization.Get(metricConfig.KeyDescription);
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
            var rowText = rowItem.transform.FindText("Text");
			var background = rowItem.transform.FindImage("Background");
			bestFitGroup.Add(rowText);

            return (rowItem, rowText, background);
        }
        private void SetIcon(GameObject column, string iconPath)
        {
            var image = column.transform.FindImage("IconContainer/Icon");
            var sprite = Resources.Load<Sprite>(iconPath);
            image.sprite = sprite;
            image.gameObject.SetActive(true);
        }

        private void SetLocalizedTitle(GameObject column, string localizationKey, List<Text> bestFitGroup)
        {
            var titleText = column.transform.FindText("Title");
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
