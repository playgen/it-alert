using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PlayGen.ITAlert.Unity.Simulation
{
	public class PlayerInputHandler : MonoBehaviour
	{
		/// <summary>
		/// store the item hit for dragging
		/// </summary>
		//private RaycastHit2D? _selectedItemHit;

		//private ItemBehaviour _selectedItem;
		//private SubsystemBehaviour _selectedSubsystem;

		//private const float CligDragThreshold = 0.2f;

		//private float _mouseDown = 0;
		//private Vector2 _mouseDownPos;
		//private Vector2 _minDragBounds;
		//private Vector2 _maxDragBounds;

		//private Vector2 _defaultDragBounds = new Vector2(-100f, -100f); // we can use a negative number as the lowest value is 0,0

		//private bool _dragging;


		#region Initialization 

		private void Update()
		{
			HandleInput();
		}

		#endregion

		#region Util

		#endregion

		private void HandleInput()
		{
			//if (Input.GetMouseButtonDown(0))
			//{
			//	_mouseDown = Time.time;
			//	_mouseDownPos = Input.mousePosition;
			//	GetDragBounds();
			//}

			//if (Input.GetMouseButton(0))
			//{
			//	if (IsMouseOutsideBounds())
			//	{
			//		Log("InputHandler::Dragging");

			//		_dragging = true;
			//		OnDrag();
			//		//DragItem();
			//	}
			//}

			if (Input.GetMouseButtonUp(0))
			{
				//if (!_dragging)
				//{
					OnClick();
				//}
				//else
				//{
				//	Log("InputHandler::Drop");

				//	_dragging = false;
				//	OnDrop();
				//}
				//DragStop();
				//ResetBoundValues();

			}
		}

		//private void GetDragBounds()
		//{
		//if (_selectedItem == null)
		//{
		//    var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		//    var itemHits = hits.Where(d => d.collider.tag.Equals(Tags.Item)).ToArray();
		//    if (itemHits.Length == 1)
		//    {
		//        _selectedItemHit = itemHits.Single();
		//        _selectedItem = _selectedItemHit.Value.collider.GetComponent<ItemBehaviour>();
		//        //get the bounds of the selected item
		//        _minDragBounds = _selectedItemHit.Value.collider.bounds.min;
		//        _maxDragBounds = _selectedItemHit.Value.collider.bounds.max;
		//    }
		//}   
		//}

		//private bool IsMouseOutsideBounds()
		//{
		//	//if (_minDragBounds == _defaultDragBounds || _maxDragBounds == _defaultDragBounds)
		//	//    return false;
		//	var inputPosition = (Vector2) (Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position);
		//	return inputPosition.x < _minDragBounds.x || inputPosition.y < _minDragBounds.y ||
		//			inputPosition.x > _maxDragBounds.x || inputPosition.y > _maxDragBounds.y;
		//}

		#region Clicks

		private void OnClick()
		{
			var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			var subsystemHits = hits.Where(d => d.collider.tag.Equals(Tags.Subsystem)).ToArray();
			var itemHits = hits.Where(d => d.collider.tag.Equals(Tags.Item)).ToArray();
			var itemContainerHits = hits.Where(d => d.collider.tag.Equals(Tags.ItemContainer)).ToArray();

			if (subsystemHits.Any())
			{
				OnClickSubsystem(subsystemHits.Single());
			}
			else if (itemHits.Any() && itemContainerHits.Any())
			{
				OnClickItemInContainer(itemHits.Single(), itemContainerHits.Single());
			}
			else if (itemContainerHits.Any())
			{
				OnClickItemContainer(itemContainerHits.Single());
			}
			else if (itemHits.Any())
			{
				OnClickItem(itemHits.Single());
			}
		}

		private void OnClickSubsystem(RaycastHit2D subsystemHit)
		{
			var subsystem = subsystemHit.collider.GetComponent<SubsystemBehaviour>();
			PlayerCommands.Move(subsystem.Id);
		}

		private void OnClickItem(RaycastHit2D itemHit)
		{
			var item = itemHit.collider.GetComponent<ItemBehaviour>();
			item.OnClick();
		}

		private void OnClickItemInContainer(RaycastHit2D itemHit, RaycastHit2D containerHit)
		{
			var item = itemHit.collider.GetComponent<ItemBehaviour>();
			var container = containerHit.collider.GetComponent<ItemContainerBehaviour>();
			container.OnClick();
		}

		private void OnClickItemContainer(RaycastHit2D containerHit)
		{
			var container = containerHit.collider.GetComponent<ItemContainerBehaviour>();
			container.OnClick();
		}

		#endregion

		//#region Drag

		//private void OnDrag()
		//{
		//	if (_selectedSubsystem == null && _selectedItem != null)
		//	{
		//		_selectedItem.OnClick(true);
		//		var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		//		var subsystemHits = hits.Where(d => d.collider.tag.Equals(Tags.Subsystem)).ToArray();
		//		//var itemHits = hits.Where(d => d.collider.tag.Equals(Tags.Item)).ToArray();
		//		////var enhancementHits = hits.Where(d => d.collider.tag.Equals(Tags.Enhancement)).ToArray();

		//		Log("InputHandler::OnDrag [" + subsystemHits.Length + ", ]");

		//		if (subsystemHits.Length == 1)
		//		{
		//			_selectedSubsystem = subsystemHits.Single().collider.GetComponent<SubsystemBehaviour>();
		//			//_selectedItemHit = itemHits.Single();
		//			//_selectedItem = _selectedItemHit.Value.collider.GetComponent<ItemBehaviour>();
		//			//_selectedItem.OnClick(true);
		//		}
		//	}
		//	// Debug.Log(string.Format("mouse input: {0} bound: min {1}, max {2}", Camera.main.ScreenToWorldPoint(Input.mousePosition), _minDragBounds, _maxDragBounds));
		//	DragItem();
		//}

		//private void DragItem()
		//{
		//	//if (_selectedItemHit.HasValue
		//	//	&& _selectedItem != null
		//	//	&& _selectedSubsystem != null
		//	//	&& _selectedSubsystem.HasActiveItem == false)
		//	//{
		//	//	var item = _selectedItem;

		//	//	item.DragStart();
		//	//	item.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
		//	//	// - (Vector3)_selectedItemClickOffset;

		//	//	Log("Item::Drag [" + item.transform.position + "]");

		//	//	Bounds locationBounds = _selectedSubsystem.DropCollider.bounds;
		//	//	if (!locationBounds.Contains(item.transform.position))
		//	//	{
		//	//		Log("Item::Drag [Clamping]");

		//	//		float clampedX = Mathf.Clamp(item.transform.position.x, locationBounds.min.x, locationBounds.max.x);
		//	//		float clampedY = Mathf.Clamp(item.transform.position.y, locationBounds.min.y, locationBounds.max.y);
		//	//		item.transform.position = new Vector3(clampedX, clampedY, item.transform.position.z);
		//	//	}
		//	//}
		//	//else
		//	//{
		//	//	DragStop();
		//	//}
		//}

		//private void OnDrop()
		//{
		//	if (_selectedItemHit.HasValue && _selectedItem != null && _selectedSubsystem != null)
		//	{
		//		var item = _selectedItem;
		//		item.DragStop();
		//		if (PlayerOwnsItem(item))
		//		{
		//			var releaseHit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero).SingleOrDefault(d => d.collider.tag.Equals(Tags.Subsystem));

		//			if (_selectedSubsystem.ConnectionSquareCollider.bounds.Contains(releaseHit.point))
		//			{
		//				PlayerCommands.ActivateItem(item.Id);
		//			}
		//			PlayerCommands.DisownItem(item.Id);
		//		}
		//		DragStop();
		//	}

		//}

		//private void DragStop()
		//{
		//	_selectedItemHit = null;
		//	_selectedItem = null;
		//	_selectedSubsystem = null;
		//	_dragging = false;
		//	//ResetBoundValues();
		//}

		//private void ResetBoundValues()
		//{
		//	_maxDragBounds = _defaultDragBounds;
		//	_minDragBounds = _defaultDragBounds;
		//}

		//#endregion
	}
}