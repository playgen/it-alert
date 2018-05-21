using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.States.Game;
using PlayGen.ITAlert.Unity.States.Game.Menu;
using PlayGen.ITAlert.Unity.States.Game.SimulationSummary;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.Tests.States.Game.SimulationSummary
{
    public class SimulationSummaryTests
    {
        [UnityTest]
        public IEnumerator DoesDisplay()
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