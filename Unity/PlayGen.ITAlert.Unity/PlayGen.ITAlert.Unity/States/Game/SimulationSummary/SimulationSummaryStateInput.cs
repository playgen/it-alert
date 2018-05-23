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
using Logger = PlayGen.Photon.Unity.Logger;
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

            var column = CreateColumn();
            AddPlayers(column, _simulationSummary.PlayersData);

            foreach (var metricConfig in metricConfigs)
            {
                sumByPlayerByMetrics.TryGetValue(metricConfig.Key, out var valueByPlayer);

                column = CreateColumn();
                SetLocalizedTitle(column, metricConfig.Key);
                SetIcon(column, metricConfig.IconPath);
                AddItems(
                    column,
                    _simulationSummary.PlayersData, 
                    valueByPlayer, 
                    0, 
                    metricConfig.HighlightHighest);
            }

	        GetPlayerBestMetrics(sumByPlayerByMetrics);
        }

		private void AddPlayers(GameObject column, IReadOnlyList<SimulationSummary.PlayerData> playersData)
        {
            foreach (var playerData in playersData)
            {
                var (rowItem, rowText) = AddRowItem(column);

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
            bool? highlightHighest)
        {
            List<Image> extremeValueRowHighlights = null;
            int? extremeValue = null;

            foreach (var playerData in playersData)
            {
                var (rowItem, rowText) = AddRowItem(column);
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

        private (GameObject, Text) AddRowItem(GameObject column)
        {
            var rowItem = Object.Instantiate(_rowItemResource, column.transform);
            var rowText = rowItem.transform.Find("Text").GetComponent<Text>();

            return (rowItem, rowText);
        }
        private void SetIcon(GameObject column, string iconPath)
        {
            var image = column.transform.Find("IconContainer/Icon").GetComponent<Image>();
            var sprite = Resources.Load<Sprite>(iconPath);
            image.sprite = sprite;
            image.gameObject.SetActive(true);
        }

        private void SetLocalizedTitle(GameObject column, string localizationKey)
        {
            column.transform.Find("Title").GetComponent<Text>().text = Localization.Get(localizationKey);
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


	    #region PlayerMetrics

	    private void GetPlayerBestMetrics(Dictionary<string, Dictionary<int?, int>> sumByPlayerByMetrics)
	    {
			//var playerComparisons = GetPlayerComparisons(sumByPlayerByMetrics);
		 //   var unique = UniquePlayerBests(playerComparisons);
			
			//LogMetrics(unique);
		}

	    private List<PlayerMetrics> GetPlayerComparisons(Dictionary<string, Dictionary<int?, int>> metrics)
	    {
		    var playerMetrics = new List<PlayerMetrics>();
		    foreach (var item in metrics)
		    {
			    var metric = item.Key;
			    var scoresByPlayer = item.Value.ToDictionary(i => i.Key, i => i.Value.ToString());
			    var players = _simulationSummary.PlayersData;

			    // using the following formula, comparison = score / 2nd best score
			    var orderedList = scoresByPlayer.OrderBy(s => Convert.ToInt16(s.Value));

			    var secondBestValue = orderedList.Count() == 1 ? orderedList.Last().Value : orderedList.ElementAt(orderedList.Count() - 2).Value;
			    var secondBest = Convert.ToInt16(secondBestValue);

			    foreach (var playerData in players)
			    {
				    int score = 0;
				    if (scoresByPlayer.TryGetValue(playerData.Id, out var value))
				    {
					    score = Convert.ToInt16(value);
				    }

				    var comparison = new PlayerMetrics
				    {
					    PlayerId = playerData.Id,
					    Metric = metric,
					    Score = score,
					    Ratio = (float)score / secondBest
				    };
				    playerMetrics.Add(comparison);
			    }
		    }
		    return playerMetrics;
	    }

		/// <summary>
		/// Get a list of each players best metric that they have unique to other players
		/// </summary>
		/// <param name="metrics"></param>
		/// <returns></returns>
	    private List<PlayerMetrics> UniquePlayerBests(List<PlayerMetrics> metrics)
	    {
		    var uniqueList = new List<PlayerMetrics>();
		    metrics = metrics.OrderByDescending(m => m.Ratio).ToList();
			// it is safe to assume the first element is the best and unique
			uniqueList.Add(metrics[0]);
			
			// now iterate through the list and get unique bests
		    for (var i = 1; i < metrics.Count; i++)
		    {
			    var playerId = metrics[i].PlayerId;
			    if (uniqueList.Any(m => m.PlayerId == playerId || m.Metric == metrics[i].Metric))
			    {
					// we already have this players or metric best, we can skip this
				    continue;
			    }
				// now we have a unique player and metric, so add it to the list
				uniqueList.Add(metrics[i]);
		    }
		    return uniqueList;
	    }

		private void LogMetrics(List<PlayerMetrics> metrics)
	    {
		    var str = "";
		    foreach (var metric in metrics)
		    {
			    str += metric.OutputString();
			}
			Logger.LogError(str);
	    }

		/// <summary>
		/// Get the players best score for all metrics
		/// </summary>
		/// <param name="metrics"></param>
		/// <param name="playerId"></param>
		/// <param name="excludeMetrics">Metrics to exclude, eg. other players have these metrics as their best</param>
		/// <returns></returns>
	    private string GetPlayersBest(List<PlayerMetrics> metrics, int? playerId, List<string> excludeMetrics = null)
	    {
			if (excludeMetrics != null)
		    {
				return metrics.Where(m => m.PlayerId == playerId && !excludeMetrics.Contains(m.Metric))
					.OrderBy(r => r.Ratio)
					.Last().Metric;
			}
			return metrics.Where(m => m.PlayerId == playerId)
				.OrderBy(r => r.Ratio)
				.Last().Metric;
	    }

	    private struct PlayerMetrics
	    {
		    public int? PlayerId;
		    public int Score;
		    public string Metric;
		    // How players compare to others in the game
		    public float Ratio;

		    public string OutputString()
		    {
			    return "Player: " + PlayerId + " - " + Metric + " - " + Ratio + " (" + Score + ")\n";
			}
	    }

	    #endregion

	}
}
