using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Debug = UnityEngine.Debug;


// ReSharper disable once CheckNamespace
public class InputHandler : MonoBehaviour
{
	/// <summary>
	/// store the item hit for dragging
	/// </summary>
	private RaycastHit2D? _selectedItemHit;

	private ItemBehaviour _selectedItem;
	private SubsystemBehaviour _selectedSubsystem;

	private const float CligDragThreshold = 0.2f;

	private float _mouseDown = 0;
    private Vector2 _mouseDownPos;
    private Vector2 _minDragBounds;
    private Vector2 _maxDragBounds;
    private Vector2 _defaultDragBounds = new Vector2(-100f, -100f); // we can use a negative number as the lowest value is 0,0
	private bool _dragging;

	public static bool DebugLog { get; set; }


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

	private void Log(string message)
	{
		if (DebugLog)
		{
			Debug.Log(message);
		}
	}

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
		if (Input.GetMouseButtonDown(0))
		{
			//Debug.Log("InputHandler::MouseDown");
			_mouseDown = Time.time;
		    _mouseDownPos = Input.mousePosition;
		    GetDragBounds();
		}

		if (Input.GetMouseButton(0))
		{
		    if (IsMouseOutsideBounds())
			{
				Log("InputHandler::Dragging");

				_dragging = true;
                OnDrag();
                //DragItem();
            }
		}

		if (Input.GetMouseButtonUp(0))
		{
		    if (!_dragging)
		    {
		        Log("InputHandler::Click");

		        OnClick();
		    }
		    else
		    {
		        Log("InputHandler::Drop");

		        _dragging = false;
		        OnDrop();
		    }
		    DragStop();
            ResetBoundValues();

		}
	}

    private void GetDragBounds()
    {
        if (_selectedItem == null)
        {
            var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            var itemHits = hits.Where(d => d.collider.tag.Equals(Tags.Item)).ToArray();
            if (itemHits.Length == 1)
            {
                _selectedItemHit = itemHits.Single();
                _selectedItem = _selectedItemHit.Value.collider.GetComponent<ItemBehaviour>();
                //get the bounds of the selected item
                _minDragBounds = _selectedItemHit.Value.collider.bounds.min;
                _maxDragBounds = _selectedItemHit.Value.collider.bounds.max;
            }
        }   
    }

    private bool IsMouseOutsideBounds()
    {
        //if (_minDragBounds == _defaultDragBounds || _maxDragBounds == _defaultDragBounds)
        //    return false;
        var inputPosition = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position);
        return inputPosition.x < _minDragBounds.x || inputPosition.y < _minDragBounds.y ||
               inputPosition.x > _maxDragBounds.x || inputPosition.y > _maxDragBounds.y;
    }

    #region Clicks

	private void OnClick()
	{
		var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		var subsystemHits = hits.Where(d => d.collider.tag.Equals(Tags.Subsystem)).ToArray();
		var itemHits = hits.Where(d => d.collider.tag.Equals(Tags.Item)).ToArray();
		var enhancementHits = hits.Where(d => d.collider.tag.Equals(Tags.Enhancement)).ToArray();

		if (subsystemHits.Any() && itemHits.Any())
		{
			OnClickItem(subsystemHits.Single(), itemHits.Single());
		}
		else if (subsystemHits.Any() && enhancementHits.Any())
		{
			//OnClickEnhancement(subsystemHits.Single(), enhancementHits.Single());
		}
		else if (subsystemHits.Any())
		{
			OnClickSubsystem(subsystemHits.Single());
		}
	}

	private void OnClickSubsystem(RaycastHit2D subsystemHit)
	{
		var subsystem = subsystemHit.collider.GetComponent<SubsystemBehaviour>();
		PlayerCommands.Move(subsystem.Id);
	}

	private void OnClickItem(RaycastHit2D subsystemHit, RaycastHit2D itemHit)
	{
		var item = itemHit.collider.GetComponent<ItemBehaviour>();
		item.OnClick(false);
	}

	private void OnClickEnhancement(RaycastHit2D subsystemHit, RaycastHit2D enhancementHit)
	{
		//var subsystem = subsystemHit.collider.GetComponent<SubsystemBehaviour>();
		//var enhancement = enhancementHit.collider.GetComponent<EnhancementBehaviour>();
	}

	#endregion

	#region Drag

	private void OnDrag()
	{
		if (_selectedSubsystem == null && _selectedItem != null)
		{
            _selectedItem.OnClick(true);
            var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			var subsystemHits = hits.Where(d => d.collider.tag.Equals(Tags.Subsystem)).ToArray();
			//var itemHits = hits.Where(d => d.collider.tag.Equals(Tags.Item)).ToArray();
			////var enhancementHits = hits.Where(d => d.collider.tag.Equals(Tags.Enhancement)).ToArray();

			Log("InputHandler::OnDrag [" + subsystemHits.Length + ", ]");

			if (subsystemHits.Length == 1)
			{
				_selectedSubsystem = subsystemHits.Single().collider.GetComponent<SubsystemBehaviour>();
				//_selectedItemHit = itemHits.Single();
				//_selectedItem = _selectedItemHit.Value.collider.GetComponent<ItemBehaviour>();
				//_selectedItem.OnClick(true);
			}
		}
       // Debug.Log(string.Format("mouse input: {0} bound: min {1}, max {2}", Camera.main.ScreenToWorldPoint(Input.mousePosition), _minDragBounds, _maxDragBounds));
        DragItem();
	}

	private void DragItem()
	{
		if (_selectedItemHit.HasValue
			&& _selectedItem != null
			&& _selectedSubsystem != null
			&& _selectedSubsystem.HasActiveItem == false)
		{
			var item = _selectedItem;

			item.DragStart();
			item.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
			// - (Vector3)_selectedItemClickOffset;

			Log("Item::Drag [" + item.transform.position + "]");

			Bounds locationBounds = _selectedSubsystem.DropCollider.bounds;
			if (!locationBounds.Contains(item.transform.position))
			{
				Log("Item::Drag [Clamping]");

				float clampedX = Mathf.Clamp(item.transform.position.x, locationBounds.min.x, locationBounds.max.x);
				float clampedY = Mathf.Clamp(item.transform.position.y, locationBounds.min.y, locationBounds.max.y);
				item.transform.position = new Vector3(clampedX, clampedY, item.transform.position.z);
			}
		}
		else
		{
			DragStop();
		}
	}

	private void OnDrop()
	{
		if (_selectedItemHit.HasValue && _selectedItem != null && _selectedSubsystem != null)
		{
			var item = _selectedItem;
			item.DragStop();
			if (PlayerOwnsItem(item))
			{
				var releaseHit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero).SingleOrDefault(d => d.collider.tag.Equals(Tags.Subsystem));

				if (_selectedSubsystem.ConnectionSquareCollider.bounds.Contains(releaseHit.point))
				{
					PlayerCommands.ActivateItem(item.Id);
                }
                PlayerCommands.DisownItem(item.Id);
            }
			DragStop();
		}

	}

	private void DragStop()
	{
		_selectedItemHit = null;
		_selectedItem = null;
		_selectedSubsystem = null;
		_dragging = false;
	    //ResetBoundValues();
	}

    private void ResetBoundValues()
    {
        _maxDragBounds = _defaultDragBounds;
        _minDragBounds = _defaultDragBounds;
    }
    #endregion
}
