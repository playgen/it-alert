using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Summary
{
    public static class PlayerMetrics
    {
        public static void GetPlayerBestMetrics(Dictionary<string, Dictionary<int?, int>> sumByPlayerByMetrics)
        {
            //var playerComparisons = GetPlayerComparisons(sumByPlayerByMetrics);
            //   var unique = UniquePlayerBests(playerComparisons);

            //LogMetrics(unique);
        }

        public static List<PlayerMetric> GetPlayerComparisons(Dictionary<string, Dictionary<int?, int>> metrics, List<int> playerIds)
        {
            var playerMetrics = new List<PlayerMetric>();
            foreach (var item in metrics)
            {
                var metric = item.Key;
                var scoresByPlayer = item.Value.ToDictionary(i => i.Key, i => i.Value.ToString());

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

                    var comparison = new PlayerMetric
                    {
                        PlayerId = playerId,
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

        public static void LogMetrics(List<PlayerMetric> metrics)
        {
            var str = "";
            foreach (var metric in metrics)
            {
                str += metric.OutputString();
            }
            UnityEngine.Debug.LogError(str);
        }

        /// <summary>
        /// Get the players best score for all metrics
        /// </summary>
        /// <param name="metrics"></param>
        /// <param name="playerId"></param>
        /// <param name="excludeMetrics">Metrics to exclude, eg. other players have these metrics as their best</param>
        /// <returns></returns>
        public static string GetPlayersBest(List<PlayerMetric> metrics, int? playerId, List<string> excludeMetrics = null)
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

        public struct PlayerMetric
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
    }
}
