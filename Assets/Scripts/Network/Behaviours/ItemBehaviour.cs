using UnityEngine;
using System.Collections;
using System;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;
using UnityEngine.UI;

// ReSharper disable CheckNamespace
public class ItemBehaviour : EntityBehaviour<ItemState>
{
	[SerializeField]
	private Image _activationTimerImage;

	[SerializeField]
	private SpriteRenderer _iconRenderer;

	private bool _dragging;

	private int _dragCount;

	public bool Dragging
	{
		get { return _dragging || _dragCount > 0; }
	}


	#region public state

	public bool IsActive { get { return EntityState != null && EntityState.Active; } }

	public int? Owner { get { return EntityState == null ? (int?) null : EntityState.Owner; } }

	public ItemType Type { get { return EntityState.ItemType; } }

	public bool IsOnSubsystem { get { return EntityState.CurrentNode.HasValue; } }

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

			//case ItemType.Tracer:
			//	spriteName = "tool_tracer";
			//	break;

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
		_dragCount = Math.Max(0, --_dragCount);
	}

	#endregion

	#region State Update

	protected override void OnUpdatedState()
	{
		//TODO: if owner has changed
		UpdateItemColor();
		UpdateActivationTimer();
	}

	private void UpdateItemColor()
	{
		bool isWhite = GetComponent<SpriteRenderer>().color == Color.white ? true : false;
		if (EntityState.Owner.HasValue && isWhite)
		{
			var playerColour = Director.GetEntity(EntityState.Owner.Value).GameObject.GetComponent<SpriteRenderer>().color;
			_iconRenderer.color = playerColour;
			_activationTimerImage.color = playerColour;

			//TriggerHint();
		}
		else if (!EntityState.Owner.HasValue && !isWhite)
		{
			_iconRenderer.color = Color.white;
			_activationTimerImage.color = new Color(1f, 1f, 1f, 0.7f);

		}
	}

	private void UpdateActivationTimer()
	{
		if (EntityState.Active)
		{
			_activationTimerImage.fillAmount = 1f - (float) EntityState.ActiveTicksRemaining/EntityState.ActiveDuration;
		}
		else
		{
			_activationTimerImage.fillAmount = 0f;
		}
	}

	#endregion


	private void TriggerHint()
	{
		//TODO: reimplement
		//UIManager.instance.DisplayHint(GetComponent<SVGRenderer>().vectorGraphics, _hintText);
	}

	#region player interaction

	private bool PlayerOwnsItem()
	{
		return Director.Player != null
				&& Owner.HasValue
				&& Owner.Value == Director.Player.Id;
	}

	public void OnClick(bool dragging)
	{
		Debug.Log("Item OnClick");

		if (CanActivate())
		{
			if (PlayerOwnsItem())
			{
				if (dragging == false)
				{
					PlayerCommands.DisownItem(this.Id);
				}
			}
			else
			{
				PlayerCommands.PickupItem(Id, EntityState.CurrentNode.Value);
			}
		}
	}

	public bool CanActivate()
	{
		return EntityState.Active == false 
			&& EntityState.CanActivate 
			&& (EntityState.Owner.HasValue == false || EntityState.Owner.Value == Director.Player.Id);
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

	public void DragStart()
	{
		_dragging = true;
	}

	public void DragStop()
	{
		_dragging = false;
		_dragCount = 2;
	}

	#endregion

}


