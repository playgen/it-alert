using System;
using System.Collections.Generic;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation
{
	// ReSharper disable once InconsistentNaming
	public class UIEntity
	{
		private GameObject _gameObject;

		public GameObject GameObject => _gameObject;

		public int Id => _entityBehaviour.Id;

		private IEntityBehaviour _entityBehaviour;
		
		public IEntityBehaviour EntityBehaviour => _entityBehaviour;

		private static readonly Dictionary<string, Func<GameObject, IEntityBehaviour>> BehaviourMappers = new Dictionary<string, Func<GameObject, IEntityBehaviour>>()
		{
			{ nameof(Subsystem), go => go.GetComponent<SubsystemBehaviour>() },
			//{ typeof(Subsystem).Name, go => go.GetComponent<EnhancementBehaviour>() },
			{ nameof(Connection), go => go.GetComponent<ConnectionBehaviour>() },
			{ nameof(Player), go => go.GetComponent<PlayerBehaviour>() },
			{ nameof(Npc), go => go.GetComponent<NpcBehaviour>() },
			{ nameof(Item), go => go.GetComponent<ItemBehaviour>() },
			{ nameof(ScenarioText), go => go.GetComponent<ScenarioTextBehaviour>() },
		};

		public UIEntity(string entityTypeName)
		{
			if (TryInistantiateEntity(entityTypeName) == false)
			{
				Debug.LogWarning($"Unknown entity type '{entityTypeName}'");
			}
		}

		public UIEntity(Entity entity)
		{
			IEntityType entityTypeFlag;
			if (entity.TryGetComponent(out entityTypeFlag))
			{
				var entityTypeName = entityTypeFlag.GetType().Name;
				if (TryInistantiateEntity(entityTypeName) == false)
				{
					Debug.LogWarning($"Unknown entity type '{entityTypeName}' on entity {entity.Id}");
				}
			}
			else
			{
				throw new Exception($"Entity type flag missing for entity {entity.Id}");
			}
		}

		private bool TryInistantiateEntity(string entityTypeName)
		{
			Func<GameObject, IEntityBehaviour> behaviourMapper;

			if (BehaviourMappers.TryGetValue(entityTypeName, out behaviourMapper))
			{
				_gameObject = Director.InstantiateEntity(entityTypeName);
				_entityBehaviour = behaviourMapper(GameObject);
				return true;
			}
			return false;
		}

		public void UpdateEntityState()
		{
			_entityBehaviour.UpdateState();
		}


	}
}