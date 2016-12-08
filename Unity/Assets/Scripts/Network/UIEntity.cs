
using System;
using Engine.Components;
using PlayGen.ITAlert.Simulation;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Properties;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public class UIEntity
{
	private readonly GameObject _gameObject;
	public GameObject GameObject {  get { return _gameObject; } }

	public int Id { get {  return _entityBehaviour.Id; } }

	public EntityType Type { get { return _entityBehaviour.EntityType; } }

	private readonly IEntityBehaviour _entityBehaviour;

	private static GameObject Graph = GameObject.Find("Graph");

	public IEntityBehaviour EntityBehaviour { get { return _entityBehaviour; } }

	public UIEntity(StateBucket state)
	{
		var entityType = state.Get<EntityTypeProperty>().Value;
		_gameObject = InstantiateEntity(entityType.ToString());
		_gameObject.transform.SetParent(Graph.transform);

		switch (entityType)
		{
			case EntityType.System:
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
		//_entityBehaviour.Initialize(state);
	}

	private GameObject InstantiateEntity(string resourceString)
	{
		return Object.Instantiate(Resources.Load(resourceString)) as GameObject;
	}

	public void UpdateEntityState(StateBucket state)
	{
		_entityBehaviour.UpdateState(state);
	}


}
