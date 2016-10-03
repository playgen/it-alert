﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Debugging.Scripts;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Simulation;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Serialization;
using PlayGen.ITAlert.TestData;
using UnityEngine.UI;

// ReSharper disable CheckNamespace

/// <summary>
/// There should only ever be one instance of this
/// </summary>
public class Director : MonoBehaviour
{
	/// <summary>
	/// Entity to GameObject map
	/// </summary>
	private static readonly Dictionary<int, UIEntity> Entities = new Dictionary<int, UIEntity>();

	/// <summary>
	/// Simulation
	/// </summary>
	public static Simulation Simulation;

	/// <summary>
	/// Current State
	/// </summary>
	private static GameState _state;

	//TODO: load this dynamically
	/// <summary>
	/// How fast the simulator is running
	/// </summary>
	public const float SimulationTick = 0.25f;

	public static float SimulationAnimationRatio;

	/// <summary>
	/// has the simulation been initialized
	/// </summary>
	public static bool Initialized { get; private set; }

	/// <summary>
	/// the active player
	/// </summary>
	private static PlayerBehaviour _player;

	public static PlayerBehaviour Player
	{
		get { return _player; }
	}

	public static Client Client { get; set; }

	public static System.Random Random = new System.Random((int) DateTime.UtcNow.Ticks);

    private static GameObject _gameOverWon;
    private static GameObject _gameOverLost;

	public static PlayerBehaviour[] Players { get; private set; }

	public static CommandResolver LocaResolver { get; private set; }

	public static SimulationRules Rules { get { return Simulation != null ? Simulation.Rules : new SimulationRules(); } }

	/// <summary>
	/// Get entity wrapper by id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public static UIEntity GetEntity(int id)
	{
		UIEntity entity;
		if (Entities.TryGetValue(id, out entity))
		{
			return entity;
		}
		throw new Exception(string.Format("Entity id:{0} not found", id));
	}

	#region Initialization

	public static void DebugInitialize()
	{
		Initialize(InitializeTestSimulation(), 1);
		//GameObject.Find("Canvas/Score").GetComponent<Image>().color = _player.PlayerColor;
		//GameObject.Find("Canvas/Score/Icon").GetComponent<Image>().color = _player.PlayerColor;
		_player.EnableDecorator();

        // todo fixup for refactor
        //PlayerCommands.Client =	new DebugClientProxy();
    }
    private static Simulation InitializeTestSimulation()
	{
		var width = 6;
		var height = 3;
		return ConfigHelper.GenerateSimulation(width, height, 2, width * height, 4);
	}

	public static void Initialize(Simulation simulation, int playerServerId)
	{
		UpdateSimulation(simulation);
		
		SimulationAnimationRatio = Time.deltaTime/SimulationTick;

		// center graph 
		UIConstants.NetworkOffset -= new Vector2((float) Simulation.GraphSize.X/2*UIConstants.SubsystemSpacing.x, (float) Simulation.GraphSize.Y/2*UIConstants.SubsystemSpacing.y);

		SetState();
		CreateInitialEntities();
		// todo uncomment SelectPlayer();

		SetPlayer(playerServerId);

		Initialized = true;

        _gameOverWon = GameObjectUtilities.FindGameObject("Canvas/GameOverWon");
	    _gameOverWon.SetActive(false);

        _gameOverLost = GameObjectUtilities.FindGameObject("Canvas/GameOverLost");
		_gameOverLost.SetActive(false);
	}

	private void Awake()
	{

	}


	private static void SetPlayer(int playerServerId)
	{
		var players = Entities.Values.Where(e => e.Type == EntityType.Player).Select(e => e.EntityBehaviour as PlayerBehaviour).ToArray();
		
		var playerState = Simulation.ExternalPlayers[playerServerId];
		_player = players.Single(p => p.Id == playerState.Id);
		_player.SetActive();
	}

	/// <summary>
	/// Create the entities from the 
	/// </summary>
	private static void CreateInitialEntities()
	{
		foreach (var stateEntity in _state.Entities)
		{
			CreateEntity(stateEntity.Key, stateEntity.Value);
		}
		// initialize after the entities have been created as some will need to reference each other
		foreach (var stateEntity in _state.Entities)
		{
			GetEntity(stateEntity.Key).EntityBehaviour.Initialize(stateEntity.Value);
		}
	}

	#endregion

	#region State Update

	public static void UpdateSimulation(Simulation simulation)
	{
		Simulation = simulation;
		LocaResolver = new CommandResolver(simulation);
	}

	private static void SetState()
	{
		_state = Simulation.GetState();
	}


	private static void CreateEntity(int id, EntityState state)
	{
		var entity = new UIEntity(state);
		Entities.Add(id, entity);
	}

	private static void UpdateEntityStates()
	{
		foreach (var newEntity in _state.Entities.Where(ekvp => Entities.ContainsKey(ekvp.Key) == false))
		{
			CreateEntity(newEntity.Key, newEntity.Value);
			GetEntity(newEntity.Key).EntityBehaviour.Initialize(newEntity.Value);
		}

		foreach (var stateEntity in _state.Entities)
		{
			GetEntity(stateEntity.Key).UpdateEntityState(stateEntity.Value);
		}
		// remove dead entities
		// TODO: make sure nothing is still referencing these
		foreach (var entityToRemove in Entities.Keys.Except(_state.Entities.Keys).ToArray())
		{
			Destroy(GetEntity(entityToRemove).GameObject);
			Entities.Remove(entityToRemove);
		}
	}

    private static void OnGameOver(bool didWin)
    {
        if (didWin)
        {
            _gameOverWon.SetActive(true);
        }
        else
        {
            _gameOverLost.SetActive(true);
        }
    }
    
	public static void Finalise(Simulation simulation)
	{
		Simulation = simulation;

	    var didWin = !Simulation.HasViruses && !Simulation.IsGameFailure;
        OnGameOver(didWin);
	}

	#endregion

	/// <summary>
	/// manually advance the simulation tick and propogate changes
	/// </summary>
	public static void Tick(bool serialize)
	{
		//TODO: replace super lazy way of stopping the game
		if (Initialized && _state.IsGameFailure == false)
		{
			Simulation.Tick();

			if (serialize)
			{
				var state = SimulationSerializer.SerializeSimulation(Simulation);
				Simulation.Dispose();

				UpdateSimulation(SimulationSerializer.DeserializeSimulation(state));
			}

			Refresh();
		}
	}

	public static void Refresh()
	{
		SetState();
		UpdateEntityStates();
	}

	#region UI accessors

	private static void DoInitialized(Action action)
	{
		if (Initialized)
		{
			action();
		}
	}

	private static T GetInitialized<T>(Func<T> func)
	{
		if (Initialized)
		{
			return func();
		}
		return default(T);
	}

	public static string GetScore()
	{
		return GetInitialized(() => _state.Score.ToString());
	}

	public static string GetTimer()
	{
		//TODO: returning the tick is only temporary
		
		return GetInitialized(() => _state.CurrentTick.ToString());
	}

	#endregion

	#region commands (temporary) 
	
	//TODO: better implementation

	public static void RequestMovePlayer(int destinationId)
	{
		Simulation.RequestMovePlayer(_player.Id, destinationId);
	}

	public static void RequestActivateItem(int itemId)
	{
		Simulation.RequestActivateItem(_player.Id, itemId);
	}

	public static void RequestDropItem(int itemId)
	{
		Simulation.RequestDropItem(_player.Id, itemId);
	}

	public static void RequestPickupItem(int itemId, int subsystemId)
	{
		Simulation.RequestPickupItem(_player.Id, itemId, subsystemId);
	}

	public static void SpawnVirus()
	{
		var subsystems = Entities.Values.Where(e => e.Type == EntityType.Subsystem).ToArray();
		Simulation.SpawnVirus((subsystems[Random.Next(0, subsystems.Length)].EntityBehaviour as SubsystemBehaviour).LogicalId);
	}

	#endregion
}