using System;
using System.Collections.Generic;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Unity.Network.Behaviours;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation
{
// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
	public class UIEntity
	{
		private readonly GameObject _gameObject;

		public GameObject GameObject => _gameObject;

		public int Id => _entityBehaviour.Id;

		private readonly IEntityBehaviour _entityBehaviour;
		
		public IEntityBehaviour EntityBehaviour => _entityBehaviour;

		private static readonly Dictionary<string, Func<GameObject, IEntityBehaviour>> BehaviourMappers = new Dictionary<string, Func<GameObject, IEntityBehaviour>>()
		{
			{ typeof(Subsystem).Name, go => go.GetComponent<SubsystemBehaviour>() },
			//{ typeof(Subsystem).Name, go => go.GetComponent<EnhancementBehaviour>() },
			{ typeof(Connection).Name, go => go.GetComponent<ConnectionBehaviour>() },
			{ typeof(Player).Name, go => go.GetComponent<PlayerBehaviour>() },
			{ typeof(Npc).Name, go => go.GetComponent<NpcBehaviour>() },
			{ typeof(Item).Name, go => go.GetComponent<ItemBehaviour>() },
			{ typeof(ScenarioText).Name, go => go.GetComponent<ScenarioTextBehaviour>() },
		};

		public UIEntity(Entity entity)
		{
			IEntityType entityTypeFlag;
			if (entity.TryGetComponent(out entityTypeFlag))
			{
				var entityTypeName = entityTypeFlag.GetType().Name;
				Func<GameObject, IEntityBehaviour> behaviourMapper;

				if (BehaviourMappers.TryGetValue(entityTypeName, out behaviourMapper))
				{
					_gameObject = Director.InstantiateEntity(entityTypeName);

					_entityBehaviour = behaviourMapper(GameObject);
				}
				else
				{
					Debug.LogWarning($"Unknown entity type '{entityTypeName}' on entity {entity.Id}");
				}
			}
			else
			{
				throw new Exception($"Entity type flag missing for entity {entity.Id}");
			}
		}

		public void UpdateEntityState()
		{
			_entityBehaviour.UpdateState();
		}


	}
}