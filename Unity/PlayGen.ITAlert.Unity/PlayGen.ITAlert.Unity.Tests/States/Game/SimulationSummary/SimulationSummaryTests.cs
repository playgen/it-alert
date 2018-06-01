using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GameWork.Core.States;
using GameWork.Core.States.Tick;
using NUnit.Framework;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.States.Game;
using PlayGen.ITAlert.Unity.States.Game.Menu;
using PlayGen.ITAlert.Unity.States.Game.Room;
using PlayGen.ITAlert.Unity.States.Game.Room.Lobby;
using PlayGen.ITAlert.Unity.States.Game.SimulationSummary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;
using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.Tests.States.Game.SimulationSummary
{
    public class SimulationSummaryTests
    {
        private static string InstanceEventsDumpPath
        {
            get
            {
                var thisAssemblyLocation = Assembly.GetAssembly(typeof(SimulationSummaryTests)).Location;
                var parentDir = Directory.GetParent(thisAssemblyLocation).FullName;
                var assemblyName = typeof(SimulationSummaryTests).Assembly.GetName().Name;
                var @namespace = typeof(SimulationSummaryTests).Namespace.Replace(assemblyName, "");
                var path = parentDir + @namespace.Replace(".", "/");
                return $"{path}/InstanceEvents_dump.txt";
            }
        }

        private readonly List<Unity.States.Game.SimulationSummary.SimulationSummary.PlayerData> _playersData =
            new List<Unity.States.Game.SimulationSummary.SimulationSummary.PlayerData>
            {
                new Unity.States.Game.SimulationSummary.SimulationSummary.PlayerData
                {
                    Id = 0,
                    Name = "Player 1",
                    Colour = "#E5297B"
                },
                new Unity.States.Game.SimulationSummary.SimulationSummary.PlayerData
                {
                    Id = 1,
                    Name = "Player 2",
                    Colour = "#EFF140"
                },
                new Unity.States.Game.SimulationSummary.SimulationSummary.PlayerData
                {
                    Id = 2,
                    Name = "Player 3",
                    Colour = "#64A7EB"
                },
                new Unity.States.Game.SimulationSummary.SimulationSummary.PlayerData
                {
                    Id = 3,
                    Name = "Player 4",
                    Colour = "#ED962F"
                }
            };

        [Test]
        public void CanParseDump()
        {
            ParseDump();
        }

        public List<StopMessage.SimulationEvent> ParseDump()
        {
            var events = new List<StopMessage.SimulationEvent>();

            foreach (var line in File.ReadAllLines(InstanceEventsDumpPath))
            {
                var columns = line.Split('\t');
                for (var i = 0; i < columns.Length; i++)
                {
                    columns[i] = columns[i].TrimStart('\"').TrimEnd('\"').Replace("\"\"", "\"");
                }

                int? playerId = null;
                if (int.TryParse(columns[2], out var parsed))
                {
                    playerId = parsed;
                }

                events.Add(new StopMessage.SimulationEvent
                {
                    PlayerId = playerId,

                    Data = columns[3],

                    EventCode = columns[4],

                    Tick = int.Parse(columns[5])
                });
            }

            Assert.AreNotEqual(0, events.Count);

            return events;
        }

        [UnityTest]
        [Timeout(int.MaxValue)]
        public IEnumerator LongPlayerNames()
        {
            _playersData.ForEach(pd => pd.Name = $"{pd.Name} - and some text to make this name really long");
            return DoesDisplay(_playersData);
        }

        [UnityTest]
        [Timeout(int.MaxValue)]
        public IEnumerator EmailPlayerNames()
        {
            _playersData.ForEach(pd => pd.Name = $"{pd.Name}@gmail.com");
            return DoesDisplay(_playersData);
        }

        [UnityTest]
        [Timeout(int.MaxValue)]
        public IEnumerator DoesDisplay_4()
        {
            return DoesDisplay(_playersData.Take(4).ToList());
        }

        [UnityTest]
        [Timeout(int.MaxValue)]
        public IEnumerator DoesDisplay_3()
        {
            return DoesDisplay(_playersData.Take(3).ToList());
        }

        [UnityTest]
        [Timeout(int.MaxValue)]
        public IEnumerator DoesDisplay_2()
        {
            return DoesDisplay(_playersData.Take(2).ToList());
        }

        [UnityTest]
        [Timeout(int.MaxValue)]
        public IEnumerator DoesDisplay_1()
        {
            return DoesDisplay(_playersData.Take(1).ToList());
        }

        public IEnumerator DoesDisplay(List<Unity.States.Game.SimulationSummary.SimulationSummary.PlayerData> playersData)
        {
            // Load the scene
            SceneManager.LoadScene("IT Alert");
            yield return null; // Scene only loaded on next frame

            // Get the root state controller
            var gameBehaviour = Object.FindObjectOfType<GameBehaviour>();
            Assert.IsNotNull(gameBehaviour);

            var rootStateController = GetStateController<TickStateController>(gameBehaviour);

            // Wait until loading is done and we're in the game state
            while (rootStateController.ActiveStateName != GameState.StateName)
            {
                yield return null;
            }

            // Get the game state controller from the game state
            var rootStateControllerStates = GetStates(rootStateController);

            var gameState = (GameState)rootStateControllerStates[GameState.StateName];
			

            var gameStateController = GetStateController<TickStateController>(gameState);


            // Wait until the game state controller has completed loading
            while (gameStateController.ActiveStateName != MenuState.StateName)
            {
                yield return null;
            }


	        var roomState = GetStates(gameStateController)[RoomState.StateName];
			var roomStateController = GetStateController<TickStateController>(roomState);

			var simulationSummaryState = GetStates(roomStateController)[SimulationSummaryState.StateName];
            var simulationSummary = (Unity.States.Game.SimulationSummary.SimulationSummary)simulationSummaryState
				.GetType()
                .GetField("_simulationSummary", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(simulationSummaryState);

			// Add the events to the simulation summary
			var events = ParseDump();
            simulationSummary.SetData(events, playersData);

			gameStateController.ExitState(roomStateController.ActiveStateName);
            gameStateController.EnterState(SimulationSummaryState.StateName);

            while (!Input.GetKey(KeyCode.Escape))
            {
                yield return null;
            }
        }

        public static T GetStateController<T>(object container)
            where T : StateControllerBase
        {
            Assert.IsNotNull(container);
            var stateControllerField = container.GetType()
                .GetField("_stateController", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(stateControllerField);
            var stateController = (T)stateControllerField.GetValue(container);
            Assert.IsNotNull(stateController);

			return stateController;
        }

        public static Dictionary<string, TState> GetStates<TState>(StateController<TState> stateController)
            where TState : State
        {
            Assert.IsNotNull(stateController);

            var stateControllerStatesField = stateController.GetType()
                .GetField("States", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(stateControllerStatesField);

            var stateControllerStates = (Dictionary<string, TState>)stateControllerStatesField.GetValue(stateController);
            Assert.IsNotNull(stateControllerStates);

            return stateControllerStates;
        }
    }
}