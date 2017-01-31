using System.Collections.Generic;
using PlayGen.ITAlert.Unity.GameStates.Game.Room.Feedback;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	public class FeedbackDragBehaviour : MonoBehaviour
	{

		private bool _beingDragged;
		private string _currentList;
		private Transform _parent;
		private Vector2 _dragPosition;
		private FeedbackStateInput _input;

		private void Start()
		{
			_parent = transform.parent;
			var trigger = GetComponent<EventTrigger>();
			trigger.triggers.Clear();
			var drag = new EventTrigger.Entry {eventID = EventTriggerType.PointerDown};
			drag.callback.AddListener(data => { BeginDrag(); });
			trigger.triggers.Add(drag);
			var drop = new EventTrigger.Entry {eventID = EventTriggerType.PointerUp};
			drop.callback.AddListener(data => { EndDrag(); });
			trigger.triggers.Add(drop);
		}

		public void SetInterface(FeedbackStateInput inter)
		{
			_input = inter;
		}

		private void BeginDrag()
		{
			_beingDragged = true;
			_dragPosition = Input.mousePosition - transform.position;
			_parent = transform.parent;
			transform.SetParent(transform.parent.parent.parent.parent, false);
			transform.position = (Vector2) Input.mousePosition - _dragPosition;
			transform.SetAsLastSibling();
		}

		private void Update()
		{
			if (_beingDragged)
			{
				transform.position = (Vector2) Input.mousePosition - _dragPosition;
			}
		}

		private void EndDrag()
		{
			_beingDragged = false;
			_dragPosition = Vector2.zero;
			var raycastResults = new List<RaycastResult>();
			//gets all UI objects below the cursor
			EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) {position = Input.mousePosition},
				raycastResults);
			foreach (var result in raycastResults)
			{
				if (result.gameObject.GetComponent<FeedbackSlotBehaviour>())
				{
					var slot = result.gameObject.GetComponent<FeedbackSlotBehaviour>();
					if (_currentList == null || _currentList == slot.CurrentList)
					{
						transform.SetParent(_parent, false);
						if (_input.RearrangeOrder(_currentList, slot.CurrentList, result.gameObject.transform.GetSiblingIndex() - 1,
							GetComponent<Text>().text, slot, this))
						{
							_currentList = slot.CurrentList;
							_parent = transform.parent;
						}
					}
				}
			}
			transform.SetParent(_parent, false);
		}
	}
}