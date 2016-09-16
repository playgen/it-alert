using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;



// ReSharper disable once CheckNamespace
public class InputHandler : MonoBehaviour
{
	/// <summary>
	/// store the item hit for dragging
	/// </summary>
	private RaycastHit2D? _selectedItemHit;

	private ItemBehaviour _selectedItem;
	private SubsystemBehaviour _selectedSubsystem;

	private float ClickInterval = 1.0f/10;

	private float _lastClick = 0;

	/// <summary>
	/// ?
	/// </summary>
	//private Vector2 _selectedItemClickOffset;

	/// <summary>
	/// 
	/// </summary>
	private int _clickItemFrames;

	#region Initialization 

	private void Start()
	{
			
	}

	private void Awake()
	{

	}

	private void Update()
	{
		HandleInput();
	}

	#endregion

	#region Util

	private bool PlayerHasItem(ItemBehaviour item)
	{
		return Director.Player != null 
			&& Director.Player.InventoryItem.HasValue
			&& Director.Player.InventoryItem.Value == item.Id;
	}

	private bool PlayerOwnsItem(ItemBehaviour item)
	{
		return Director.Player != null
				&& item.Owner.HasValue
				&& item.Owner.Value == Director.Player.Id;
	}

	#endregion

	private void HandleInput()
	{
		//raycast to see if player has clicked/tapped on anything
		var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		var subsystemHits = hits.Where(d => d.collider.tag.Equals(Tags.Subsystem)).ToArray();
		var itemHits = hits.Where(d => d.collider.tag.Equals(Tags.Item)).ToArray();
		var enhancementHits = hits.Where(d => d.collider.tag.Equals(Tags.Enhancement)).ToArray();

		var click = false;

		if (Input.GetMouseButtonDown(0) && _lastClick <= 0)
		{
			if (subsystemHits.Any() && itemHits.Any())
			{
				OnClickItem(subsystemHits.Single(), itemHits.Single());
			}
			else if (subsystemHits.Any() && enhancementHits.Any())
			{
				OnClickEnhancement(subsystemHits.Single(), enhancementHits.Single());
			}
			else if (subsystemHits.Any())
			{
				OnClickSubsystem(subsystemHits.Single());
			}
			click = true;
		}

		if (Input.GetMouseButton(0) && click == false)
		{
			if (_selectedItemHit.HasValue && _clickItemFrames++ > 0)
			{
				DragItem(_selectedItemHit.Value);
			}
			_lastClick = ClickInterval;
		}

		if (Input.GetMouseButtonUp(0) && _lastClick >= 0)
		{
			OnDrop();
		}

		_lastClick = Math.Max(0, _lastClick - Time.deltaTime);
	}

	#region Clicks

	private void OnClickSubsystem(RaycastHit2D subsystemHit)
	{
		var subsystem = subsystemHit.collider.GetComponent<SubsystemBehaviour>();
		PlayerCommands.Move(subsystem);
	}

	private void OnClickItem(RaycastHit2D subsystemHit, RaycastHit2D itemHit)
	{
		var item = itemHit.collider.GetComponent<ItemBehaviour>();
		var subsystem = subsystemHit.collider.GetComponent<SubsystemBehaviour>();

		// if the player can select this item
		if (item.CanActivate())
		{
			// set the item time so we can detect a drag
			_clickItemFrames = 0;
			_selectedItemHit = itemHit;
			_selectedItem = item;
			_selectedSubsystem = subsystem;

			if (PlayerHasItem(item))
			{
				//TODO: the item should be dropped automatically when the player enters the subsystem, so there is nothing to do here
				// if the player already has the item, drop it
				PlayerCommands.DropItem(item);
			}
			else
			{
				// pick it up
				PlayerCommands.PickupItem(item, subsystem);
			}
		}
	}

	private void OnClickEnhancement(RaycastHit2D subsystemHit, RaycastHit2D enhancementHit)
	{
		var subsystem = subsystemHit.collider.GetComponent<SubsystemBehaviour>();
		var enhancement = enhancementHit.collider.GetComponent<EnhancementBehaviour>();
	}

	#endregion

	#region Drag

	private void OnDrop()
	{
		if (_selectedItemHit.HasValue && _selectedItem != null && _selectedSubsystem != null)
		{
			var item = _selectedItem;
			if (PlayerOwnsItem(item))
			{
				var releaseHit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero).SingleOrDefault(d => d.collider.tag.Equals(Tags.Subsystem));

				if (_selectedSubsystem.ConnectionSquareCollider.bounds.Contains(releaseHit.point))
				{
					PlayerCommands.ActivateItem(item);
				}
				else
				{
					//if (Time.time - _clickItemTime < 0.25f)
					//{
					//	PlayerCommands.DropItem(_player, item);
					//}
				}
			}
			_selectedItemHit = null;
			_selectedItem = null;
			_selectedSubsystem = null;
			_clickItemFrames = 0;
		}
	}

	private void DragItem(RaycastHit2D hit)
	{
		var item = hit.collider;
		item.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;// - (Vector3)_selectedItemClickOffset;

		Bounds locationBounds = _selectedSubsystem.DropCollider.bounds;
		if (!locationBounds.Contains(hit.point))
		{
			float clampedX = Mathf.Clamp(item.transform.position.x, locationBounds.min.x, locationBounds.max.x);
			float clampedY = Mathf.Clamp(item.transform.position.y, locationBounds.min.y, locationBounds.max.y);
			item.transform.position = new Vector3(clampedX, clampedY, item.transform.position.z);
		}
	}

	#endregion
}
