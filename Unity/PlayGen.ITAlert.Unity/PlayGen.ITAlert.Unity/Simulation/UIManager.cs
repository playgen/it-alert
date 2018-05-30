using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Lifecycle;
using Engine.Systems.Timing;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scoring.Team;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Components;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation
{
	// ReSharper disable once InconsistentNaming
	public class UIManager : MonoBehaviour
	{
		#region editor properties

		/// <summary>
		/// timer text component
		/// </summary>
		[SerializeField]
		private Text _timerText;

		[SerializeField]
		private GameObject _itemPanel;

		[SerializeField]
		private GameObject _endGameSuccessOverlay;

		[SerializeField]
		private GameObject _endGameNeutralOverlay;

		[SerializeField]
		private GameObject _endGameFailureOverlay;

		[SerializeField]
		private GameObject _endGameOverlay;

		[SerializeField]
		private Director _director;

		[SerializeField]
		private GameObject _teamSCoreOverlay;

		[SerializeField]
		private GameObject _teamScoresParent;

		[SerializeField]
		private Text _teamScoreText;

		#endregion

		private string _resourcesScoreObject = "ScoreObject";

		private TimerSystem _timerSystem;

		private void Start()
		{
			Canvas.ForceUpdateCanvases();
		}

		private void Awake()
		{
			_director.Reset += Reset;
			_director.PlayersGameEnded += Director_GameEnded;
		}

		private void Director_GameEnded(EndGameState endGameState, List<ITAlertPlayer> players)
		{
			switch (endGameState)
			{
				case EndGameState.Success:
					_endGameSuccessOverlay.SetActive(true);
					break;
				case EndGameState.Neutral:
					_endGameNeutralOverlay.SetActive(true);
					break;
				case EndGameState.Failure:
					_endGameFailureOverlay.SetActive(true);
					break;
				default:
					LogProxy.Error("Director ended game with unknown state");
					return;
			}
			_endGameOverlay.SetActive(true);

			if (_director.SimulationRoot.Scenario.Scoring != SimulationScenario.ScoringMode.None)
			{
				_teamSCoreOverlay.SetActive(true);
				if (_director.SimulationRoot.ECS.TryGetSystem<TeamScoringSystem>(out var teamScoringSystem))
				{
					ShowEndGameScore(teamScoringSystem, players);
					//var playerScore = teamScoringSystem.GetPlayerScores();
					//var totalPlayerScore = playerScore.Sum(s => s.PublicScore);
					//var multiplier = teamScoringSystem.SystemHealth.Average();
					//var totalScore = Math.Round(totalPlayerScore * multiplier);

					//_teamScoreText.text = totalPlayerScore + " x " + multiplier.ToString("0.0000") + " = " + totalScore; 
					_teamScoreText.text = teamScoringSystem.CumulativeScore.ToString("d5");
				}
			}
			_timerText.transform.parent.gameObject.SetActive(false);
			_itemPanel.SetActive(false);

			var gameContainer = GameObjectUtilities.FindGameObject("Game/Canvas");

			var canvasGroup = gameContainer.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0.1f;
			canvasGroup.blocksRaycasts = false;
			foreach (var trail in gameContainer.GetComponentsInChildren<TrailRenderer>())
			{
				trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0.25f);
				trail.endColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0.125f);
			}
			foreach (var blink in gameContainer.GetComponentsInChildren<BlinkBehaviour>())
			{
				var image = blink.GetComponent<Image>();
				if (image != null)
				{
					blink.enabled = false;
					image.color = new Color(image.color.r, image.color.g, image.color.b, 0.625f);
				}
			}
		}

		private void ShowEndGameScore(TeamScoringSystem teamScoringSystem, List<ITAlertPlayer> players)
		{
			var parent = _teamScoresParent.transform;
			var playerScores = teamScoringSystem.GetPlayerScores();
			var multiplier = 1 + teamScoringSystem.SystemHealth.Average();
			var finalScore = Math.Round(playerScores.Sum(s => s.PublicScore) * multiplier);

			foreach (var scores in playerScores)
			{
				var playerbehaviours = GameObjectUtilities.FindGameObject("Game").GetComponentsInChildren<PlayerBehaviour>().ToDictionary(p => p.Id, p => p.PhotonId);
				playerbehaviours.TryGetValue(scores.PlayerEntityId, out var playerPhotonId);

				var player = players.FirstOrDefault(p => p.PhotonId == playerPhotonId);

				if (player == null)
				{
					player = new ITAlertPlayer(){Colour = "#FFFFFF", Name = "Temp Player"};
				}
				CreateScoreObject(parent, player.Name, scores.PublicScore.ToString(), player.Colour);
			}

			// now create the multiplier object
			var multiplierObj = parent.transform.parent.Find("Multiplier").gameObject;
			SetupScoreObject(multiplierObj,Localization.Get("GAME_OVER_MULTIPLIER"), multiplier.ToString("N"));
			// Finally create the final score object

			var finalScoreObj = parent.transform.parent.Find("FinalScore").gameObject;
			SetupScoreObject(finalScoreObj, Localization.Get("GAME_OVER_FINAL_SCORE"), finalScore.ToString());

			parent.gameObject.BestFit();
			multiplierObj.BestFit();
			finalScoreObj.BestFit();
		}

		private void CreateScoreObject(Transform parent, string name, string value, string color = "#FFFFFF")
		{
			var scoreObj = Resources.Load<GameObject>(_resourcesScoreObject);

			var playerObj = Instantiate(scoreObj, parent, false);
			SetupScoreObject(playerObj, name, value, color);
		}

		private void SetupScoreObject(GameObject scoreObj, string name, string value, string color = "#FFFFFF")
		{
			var nameText = scoreObj.transform.Find("Text").GetComponent<Text>();
			var scoreText = scoreObj.transform.Find("Score").GetComponent<Text>();

			nameText.text = name.Cutoff(TextCutoff.GlobalCutoffAfter, TextCutoff.GlobalMaxLength);
			scoreText.text = value;

			if (ColorUtility.TryParseHtmlString(color, out var colour))
			{
				//rowText.color = colour;
				nameText.color = colour;
				scoreText.color = colour;
			}
		}

		private void FixedUpdate()
		{
			SetTimer();
		}

		#region read game params

		private void SetTimer()
		{
			if (_timerSystem == null)
			{
				if (_director.SimulationRoot.ECS.TryGetSystem(out _timerSystem) == false)
				{
					throw new SimulationIntegrationException("Could not locate timing system");
				}
			}
			_timerText.text = _timerSystem != null && _timerSystem.Enabled ? $"{_timerSystem.Current.Minutes:00}:{_timerSystem.Current.Seconds:00}" : _director.Tick.ToString("d5");
		}

		#endregion

		public void Reset()
		{
			_endGameSuccessOverlay.SetActive(false);
			_endGameNeutralOverlay.SetActive(false);
			_endGameFailureOverlay.SetActive(false);
			_endGameOverlay.SetActive(false);
			_timerText.transform.parent.gameObject.SetActive(true);
			_itemPanel.SetActive(true);
			_timerText.text = 0.ToString("d5");
			_teamSCoreOverlay.SetActive(false);
			_teamScoreText.text = 0.ToString("d5");
			_timerSystem = null;
		}
	}
}