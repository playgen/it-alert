using UnityEngine;
using System.Collections;
using System;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;

// ReSharper disable CheckNamespace
public class ItemBehaviour : EntityBehaviour<ItemState>
{
	public ItemType Type
	{
		get { return EntityState.ItemType;  }
	}

	#region public state

	public bool IsActive { get { return EntityState != null && EntityState.Active; } }

	public int? Owner { get { return EntityState == null ? (int?) null : EntityState.Owner; } }

	#endregion

	#region Initialization

	public void Start()
	{
		
	}

	public void Awake()
	{
		
	}

	protected override void OnInitialize()
	{
		SetSprite();
		//_activeDuration = EntityState.ActiveDuration;
	}
	
	private void SetSprite()
	{
		string spriteName;

		switch (Type)
		{
			case ItemType.Repair:
				spriteName = "tool_repair";
				break;

			case ItemType.Scanner:
				spriteName = "tool_scanner";
				break;

			case ItemType.Tracer:
				spriteName = "tool_tracer";
				break;

			case ItemType.Cleaner:
				spriteName = "tool_cleaner";
				break;

			default:
				return;
		}
		//TODO: this can probably be optimised by precaching a dictionary of assets to item type
		GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spriteName);
	}

	#endregion

	#region Unity Update

	protected override void OnFixedUpdate()
	{
	}

	protected override void OnUpdate()
	{
	}

	#endregion

	#region State Update

	protected override void OnUpdatedState()
	{
		//TODO: if owner has changed
		UpdateItemColor();
	}

	private void UpdateItemColor()
	{
		bool isWhite = GetComponent<SpriteRenderer>().color == Color.white ? true : false;
		if (EntityState.Owner.HasValue && isWhite)
		{
			GetComponent<SpriteRenderer>().color = Director.GetEntity(EntityState.Owner.Value).GameObject.GetComponent<SpriteRenderer>().color;
			TriggerHint();
		}
		else if (!EntityState.Owner.HasValue && !isWhite)
		{
			GetComponent<SpriteRenderer>().color = Color.white;
		}
	}

	#endregion


	private void TriggerHint()
	{
		//TODO: reimplement
		//UIManager.instance.DisplayHint(GetComponent<SVGRenderer>().vectorGraphics, _hintText);
	}

	#region player interaction

	public bool CanActivate()
	{
		return IsActivated() == false && (EntityState.Owner.HasValue  == false || EntityState.Owner.Value == Director.Player.Id);
	}

	public bool IsActivated()
	{
		return EntityState.Active;
	}

	public void Deactivate()
	{
		throw new NotImplementedException();
	}

	public void Activate()
	{
		// TODO: Start Item Animation
		throw new NotImplementedException();
	}

	#endregion

}


