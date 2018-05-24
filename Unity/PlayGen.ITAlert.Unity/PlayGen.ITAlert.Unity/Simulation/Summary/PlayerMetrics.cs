using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayGen.ITAlert.Unity.States.Game.SimulationSummary;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Summary
{
    public static class PlayerMetrics
    {
	   
		public static List<PlayerMetric> GetPlayerBestMetrics(Dictionary<string, Dictionary<int?, int>> sumByPlayerByMetrics, SimulationSummary simulationSummary)
        {
            var playerComparisons = GetPlayerComparisons(sumByPlayerByMetrics, simulationSummary);
            var unique = UniquePlayerBests(playerComparisons);
	        return unique;
	        //LogMetrics(unique);
        }

        public static List<PlayerMetric> GetPlayerComparisons(Dictionary<string, Dictionary<int?, int>> metrics, SimulationSummary simulationSummary)
        {
            var playerMetrics = new List<PlayerMetric>();
            foreach (var item in metrics)
            {
                var metric = item.Key;
                var scoresByPlayer = item.Value.ToDictionary(i => i.Key, i => i.Value.ToString());

				var playerIds = simulationSummary.PlayersData.Select(p => p.Id).ToList();

				// using the following formula, comparison = score / 2nd best score
				var orderedList = scoresByPlayer.OrderBy(s => Convert.ToInt16(s.Value));

                var secondBestValue = orderedList.Count() == 1 ? orderedList.Last().Value : orderedList.ElementAt(orderedList.Count() - 2).Value;
                var secondBest = Convert.ToInt16(secondBestValue);

                foreach (var playerId in playerIds)
                {
                    int score = 0;
                    if (scoresByPlayer.TryGetValue(playerId, out var value))
                    {
                        score = Convert.ToInt16(value);
                    }

					var playerData = simulationSummary.PlayersData.First(p => p.Id == playerId);

                    var comparison = new PlayerMetric
                    {
                        PlayerData = playerData,
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
        public static List<PlayerMetric> UniquePlayerBests(List<PlayerMetric> metrics)
        {
            var uniqueList = new List<PlayerMetric>();
            metrics = metrics.OrderByDescending(m => m.Ratio).ToList();
            // it is safe to assume the first element is the best and unique
            uniqueList.Add(metrics[0]);

            // now iterate through the list and get unique bests
            for (var i = 1; i < metrics.Count; i++)
            {
                var playerId = metrics[i].PlayerData.Id;
                if (!uniqueList.Any(m => m.PlayerData.Id == playerId || m.Metric == metrics[i].Metric))
                {
					// this is a unique player and metric, so add it to the list
	                uniqueList.Add(metrics[i]);
                }
            }
            return uniqueList;
        }

#if DEBUG
		public static void LogMetrics(List<PlayerMetric> metrics)
        {
            var str = "";
            foreach (var metric in metrics)
            {
                str += metric.OutputString();
            }
            UnityEngine.Debug.LogError(str);
        }
#endif
        public struct PlayerMetric
        {
	        public SimulationSummary.PlayerData PlayerData;
            public int Score;
            public string Metric;
            // How players compare to others in the game
            public float Ratio;

            public string OutputString()
            {
                return "Player: " + PlayerData.Name + " - " + Metric + " - " + Ratio + " (" + Score + ")\n";
            }
        }
    }
}
