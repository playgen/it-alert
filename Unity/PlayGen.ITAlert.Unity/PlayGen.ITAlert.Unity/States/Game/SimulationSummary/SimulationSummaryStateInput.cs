using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
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
	        //var playerComparisons = GetPlayerComparisons(sumByPlayerByMetrics);
	        //LogPlayers(playerComparisons);

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
				Logger.LogError(metric + ": " + secondBest);

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
						Ratio = (float) score / secondBest
					};
					playerMetrics.Add(comparison);
				}
			}
		    return playerMetrics;
	    }

	    private void LogPlayers(List<PlayerMetrics> metrics)
	    {
		    var str = "";
		    foreach (var playerMetricse in metrics)
		    {
			    str += "Player: " + playerMetricse.PlayerId + " - " + playerMetricse.Metric + " - " + playerMetricse.Ratio + " (" + playerMetricse.Score + ")\n";
		    }
			Logger.LogError(str);
	    }

	    private void LogPlayersBest(List<PlayerMetrics> metrics, int playerId)
	    {
		    var best = metrics.Where(m => m.PlayerId == playerId)
			    .OrderBy(v => v.Ratio)
			    .Last().Metric;
	    }


		private struct PlayerMetrics
	    {
		    public int? PlayerId;
		    public int Score;
		    public string Metric;
			// How players compare to others in the game
		    public float Ratio;
	    }
    }
}
