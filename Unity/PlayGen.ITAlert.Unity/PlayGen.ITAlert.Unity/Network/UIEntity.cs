
using System;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Unity.Network.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Network
{
// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
	public class UIEntity
	{
		private readonly GameObject _gameObject;

		public GameObject GameObject => _gameObject;

		public int Id => _entityBehaviour.Id;

		public EntityType Type => _entityBehaviour.EntityType;

		private readonly IEntityBehaviour _entityBehaviour;


		public IEntityBehaviour EntityBehaviour => _entityBehaviour;

		public UIEntity(Entity entity)
		{
			EntityTypeProperty entityType;
			if (entity.TryGetComponent(out entityType))
			{
				_gameObject = Director.InstantiateEntity(entityType.Value.ToString());
				_gameObject.transform.SetParent(Director.Graph.transform, false);

				switch (entityType.Value)
				{
					case EntityType.Subsystem:
						_entityBehaviour = GameObject.GetComponent<SubsystemBehaviour>();
						break;
					case EntityType.Enhancement:
						_entityBehaviour = GameObject.GetComponent<EnhancementBehaviour>();
						break;
					case EntityType.Connection:
						_entityBehaviour = GameObject.GetComponent<ConnectionBehaviour>();
						break;
					case EntityType.Player:
						_entityBehaviour = GameObject.GetComponent<PlayerBehaviour>();
						break;
					case EntityType.Npc:
						_entityBehaviour = GameObject.GetComponent<NpcBehaviour>();
						break;
					case EntityType.Item:
						_entityBehaviour = GameObject.GetComponent<ItemBehaviour>();
						break;
					default:
						// bad practice to throw an exception from a constructor, but this is serious!
						throw new Exception("Unknow entity type");
				}

				// initialize will be called after all the entities have been created
				//_entityBehaviour.Initialize(entity);
			}
		}

		public void UpdateEntityState()
		{
			_entityBehaviour.UpdateState();
		}


	}
}